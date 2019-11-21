using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Integral
{
    public class Ball : Renderable
    {
        Effect effect;
        EffectTechnique technique;
        EffectPass pass;
        ShaderSignature inputSignature;

        InputLayout layout;
        EffectMatrixVariable tmat;
        int vertexStride = 0;
        int indexStride = 0;
        int numVertices = 0;
        int numIndices = 0;

        int vertexBufferSizeInBytes = 0;
        int indexBufferSizeInBytes = 0;

        
        DataStream vertices;
        DataStream indices;
        SlimDX.Direct3D11.Buffer vertexBuffer;
        SlimDX.Direct3D11.Buffer indexBuffer;
        int color = Color.Olive.ToArgb();
        
        const double pi = Math.PI;
        public struct Vertex
        {
            public Vector3 Position;
            public int Color;

            public Vertex(Vector3 position, int color)
            {
                this.Position = position;
                this.Color = color;
            }
        }
        public Ball(int size, double X, double Y, double Z)
        {
            try
            {
                using (ShaderBytecode effectByteCode = ShaderBytecode.CompileFromFile(
                    "Shaders/colorEffect.fx",
                    "Render",
                    "fx_5_0",
                    ShaderFlags.EnableStrictness,
                    EffectFlags.None))
                {
                    effect = new Effect(DeviceManager.Instance.device, effectByteCode);
                    technique = effect.GetTechniqueByIndex(0);
                    pass = technique.GetPassByIndex(0);
                    inputSignature = pass.Description.Signature;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            var elements = new[] { 
                new InputElement("POSITION", 0, Format.R32G32B32_Float, 0),
                new InputElement("COLOR", 0, Format.B8G8R8A8_UNorm, 12, 0) 
            };
            layout = new InputLayout(DeviceManager.Instance.device, inputSignature, elements);

            int increas = 15;
            tmat = effect.GetVariableByName("gWVP").AsMatrix();

            numVertices = 360 / increas * 3 + 3;
            vertexStride = Marshal.SizeOf(typeof(Vertex));
            vertexBufferSizeInBytes = vertexStride * numVertices;

            vertices = new DataStream(vertexBufferSizeInBytes, true, true);

            float x = (float)X;
            float y = (float)Y;
            float z = (float)Z;
            size = size * 10;
            //vertices.Write(new Vertex(new Vector3(x, y, z), color));
            
            for (int i = 0; i <= 360; i += increas)
            {
                vertices.Write(new Vertex(new Vector3(x + size * (float)Math.Sin(i * pi / 180), y + size * (float)Math.Cos(i * pi / 180), z), color));
            }

            for (int i = 0; i <= 360; i += increas)
            {
                vertices.Write(new Vertex(new Vector3(x + size * (float)Math.Sin(i * pi / 180), y, z + size * (float)Math.Cos(i * pi / 180)), color));
            }

            for (int i = 0; i <= 360; i += increas)
            {
                vertices.Write(new Vertex(new Vector3(x, y + size * (float)Math.Sin(i * pi / 180), z + size * (float)Math.Cos(i * pi / 180)), color));
            }
            
            
            vertices.Position = 0;

            vertexBuffer = new SlimDX.Direct3D11.Buffer(
               DeviceManager.Instance.device,
               vertices,
               vertexBufferSizeInBytes,
               ResourceUsage.Default,
               BindFlags.VertexBuffer,
               CpuAccessFlags.None,
               ResourceOptionFlags.None,
               0);

            int round = 360 / increas;
            numIndices = round * 6 + 6;
            
            indexStride = Marshal.SizeOf(typeof(short)); // 2 bytes
            indexBufferSizeInBytes = numIndices * indexStride;

            indices = new DataStream(indexBufferSizeInBytes, true, true);

            // Cube has 6 sides: top, bottom, left, right, front, back

            

            int k = 0;
            for (int j = 0; j <= 2; j++)
            {
                for (int i = 0; i < round; i++)
                {
                    indices.WriteRange(new short[] { (short)(k), (short)(k+1) });
                    k++;
                }
                k++;
            }
            

            indices.Position = 0;

            indexBuffer = new SlimDX.Direct3D11.Buffer(
                DeviceManager.Instance.device,
                indices,
                indexBufferSizeInBytes,
                ResourceUsage.Default,
                BindFlags.IndexBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                0);
        }

        public override void render()
        {
            Matrix ViewPerspective = CameraManager.Instance.ViewPerspective;
            tmat.SetMatrix(ViewPerspective);

            DeviceManager.Instance.context.InputAssembler.InputLayout = layout;
            DeviceManager.Instance.context.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
            DeviceManager.Instance.context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, vertexStride, 0));
            DeviceManager.Instance.context.InputAssembler.SetIndexBuffer(indexBuffer, Format.R16_UInt, 0);

            technique = effect.GetTechniqueByName("Render");

            EffectTechniqueDescription techDesc;
            techDesc = technique.Description;

            for (int p = 0; p < techDesc.PassCount; ++p)
            {
                technique.GetPassByIndex(p).Apply(DeviceManager.Instance.context);
                DeviceManager.Instance.context.DrawIndexed(numIndices, 0, 0);
            }
        }
        public override void dispose()
        {
            effect.Dispose();
            inputSignature.Dispose();
            vertexBuffer.Dispose();
            layout.Dispose();
        }
    }
}
