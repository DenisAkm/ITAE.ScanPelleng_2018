using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using SlimDX.D3DCompiler;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;

namespace Integral
{
    public class TriangleSurface: Renderable
    {
        ShaderSignature inputSignature;
        EffectTechnique technique;
        EffectPass pass;

        Effect effect;

        InputLayout layout;
        SlimDX.Direct3D11.Buffer vertexBuffer;
        SlimDX.Direct3D11.Buffer indexBuffer;
        DataStream vertices;
        DataStream indices;

        int vertexStride = Marshal.SizeOf(typeof(Vertex));
        int indexStride = Marshal.SizeOf(typeof(short));        
        int numVertices = 0;        
        int numIndices = 0;

        int vertexBufferSizeInBytes = 0;
        int indexBufferSizeInBytes = 0;

        
        EffectMatrixVariable tmat;

        [StructLayout(LayoutKind.Sequential)]
        private struct Vertex
        {
            public Vector3 Position;
            public int Color;

            public Vertex(Vector3 position, int color)
            {
                this.Position = position;
                this.Color = color;
            }
        }

        public TriangleSurface(List<double> x, List<double> y, List<double> z, List<int> P1, List<int> P2, List<int> P3, int color)
        {
            try
            {
                string shadersPath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Environment.CurrentDirectory)), "Shaders/colorEffect.fx");
                using (ShaderBytecode effectByteCode = ShaderBytecode.CompileFromFile(
                    shadersPath,       
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


            tmat = effect.GetVariableByName("gWVP").AsMatrix();


            vertexStride = Marshal.SizeOf(typeof(Vertex)); // 16 bytes

            numVertices = x.Count;
            vertexBufferSizeInBytes = vertexStride * numVertices;

            vertices = new DataStream(vertexBufferSizeInBytes, true, true);
            
            float a, b, c;
            int mmfactor = 1000;
            for (int i = 0; i < x.Count; i++)
            {
                a = Convert.ToSingle(x[i]);
                b = Convert.ToSingle(y[i]);
                c = Convert.ToSingle(z[i]);
                vertices.Write(new Vertex(new Vector3(mmfactor * b, mmfactor * a, mmfactor * c), color));           
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

            numIndices = 2 * P1.Count * 6;
            indexStride = Marshal.SizeOf(typeof(short)); // 2 bytes
            indexBufferSizeInBytes = numIndices * indexStride;

            indices = new DataStream(indexBufferSizeInBytes, true, true);

            for (int i = 0; i < P1.Count; i++)
            {
                
                short d1 = (short)(Convert.ToDouble(P1[i]) - 1);
                short d2 = (short)(Convert.ToDouble(P2[i]) - 1);
                short d3 = (short)(Convert.ToDouble(P3[i]) - 1);
                
                //прямая сторона
                indices.WriteRange(new short[] { d1, d2, d3 });
                //обратная сторона
                indices.WriteRange(new short[] { d1, d3, d2 });
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
