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


namespace Integral
{
    public class Arrows : Renderable
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
        int length = 0;
        int ColorRed = Color.FromArgb(220, 40, 20).ToArgb();
        int ColorBlue = Color.FromArgb(63, 72, 204).ToArgb();
        int ColorGreen = Color.FromArgb(20, 170, 50).ToArgb();

        EffectMatrixVariable tmat;

        [StructLayout(LayoutKind.Sequential)]
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

        public Arrows(int size)
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


            tmat = effect.GetVariableByName("gWVP").AsMatrix();

            numVertices = 6;
            vertexBufferSizeInBytes = vertexStride * numVertices;

            vertices = new DataStream(vertexBufferSizeInBytes, true, true);

            length = size * 100;
            vertices.Write(new Vertex(new Vector3(0, 0, 0), ColorRed));     
            vertices.Write(new Vertex(new Vector3(0, 0, length), ColorRed));  //красная ось Z
            vertices.Write(new Vertex(new Vector3(0, 0, 0), ColorGreen));
            vertices.Write(new Vertex(new Vector3(length, 0, 0), ColorGreen));  //зеленая ось Y
            vertices.Write(new Vertex(new Vector3(0, 0, 0), ColorBlue));
            vertices.Write(new Vertex(new Vector3(0, length, 0), ColorBlue));  //синяя ось Х
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

            numIndices = 2 * numVertices;
            indexBufferSizeInBytes = numIndices * indexStride;

            indices = new DataStream(indexBufferSizeInBytes, true, true);

            //прямая сторона
            indices.WriteRange(new short[] { (short)0, (short)1 });
            indices.WriteRange(new short[] { (short)2, (short)3 });
            indices.WriteRange(new short[] { (short)4, (short)5 });

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

   
