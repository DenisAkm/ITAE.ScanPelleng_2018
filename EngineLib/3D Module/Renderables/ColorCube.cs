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
    public class ColorCube : Renderable
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

        int vertexStride = 0;
        int indexStride = 0;
        int numVertices = 0;        
        int numIndices = 0;

        int vertexBufferSizeInBytes = 0;
        int indexBufferSizeInBytes = 0;

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

        public ColorCube(ArrayList x, ArrayList y, ArrayList z)
        {
            try
            {
                using (ShaderBytecode effectByteCode = ShaderBytecode.CompileFromFile(
                    "colorEffect.fx",
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

            // half length of an edge
            float offset = 0.5f;

            vertexStride = Marshal.SizeOf(typeof(Vertex)); // 16 bytes
            numVertices = x.Count;
            vertexBufferSizeInBytes = vertexStride * numVertices;

            vertices = new DataStream(vertexBufferSizeInBytes, true, true);

            vertices.Write(new Vertex(new Vector3(+offset, +offset, +offset), Color.FromArgb(255, 255, 255).ToArgb())); // 0
            vertices.Write(new Vertex(new Vector3(+offset, +offset, -offset), Color.FromArgb(255, 255, 255).ToArgb())); // 1
            vertices.Write(new Vertex(new Vector3(-offset, +offset, -offset), Color.FromArgb(255, 255, 255).ToArgb())); // 2
            vertices.Write(new Vertex(new Vector3(-offset, +offset, +offset), Color.FromArgb(255, 255, 255).ToArgb())); // 3

            vertices.Write(new Vertex(new Vector3(-offset, -offset, +offset), Color.FromArgb(255, 255, 255).ToArgb())); // 4
            vertices.Write(new Vertex(new Vector3(+offset, -offset, +offset), Color.FromArgb(255, 255, 255).ToArgb())); // 5
            vertices.Write(new Vertex(new Vector3(+offset, -offset, -offset), Color.FromArgb(255, 255, 255).ToArgb())); // 6
            vertices.Write(new Vertex(new Vector3(-offset, -offset, -offset), Color.FromArgb(255, 255, 255).ToArgb())); // 7
            //float a, b, c;
            //for (int i = 0; i < x.Count; i++)
            //{
            //    a = Convert.ToSingle(x[i]);
            //    b = Convert.ToSingle(y[i]);
            //    c = Convert.ToSingle(z[i]);
            //    vertices.Write(new Vertex(new Vector3(a, b, c), Color.FromArgb(255, 255, 255).ToArgb()));
            //}

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

            numIndices = 36;
            indexStride = Marshal.SizeOf(typeof(short)); // 2 bytes
            indexBufferSizeInBytes = numIndices * indexStride;

            indices = new DataStream(indexBufferSizeInBytes, true, true);

            // Cube has 6 sides: top, bottom, left, right, front, back

            // top
            indices.WriteRange(new short[] { 0, 1, 2 });
            indices.WriteRange(new short[] { 2, 1, 0 });
            //indices.WriteRange(new short[] { 2, 3, 0 });

            //// right
            //indices.WriteRange(new short[] { 0, 5, 6 });
            //indices.WriteRange(new short[] { 6, 1, 0 });

            //// left
            //indices.WriteRange(new short[] { 2, 7, 4 });
            //indices.WriteRange(new short[] { 4, 3, 2 });

            //// front
            //indices.WriteRange(new short[] { 1, 6, 7 });
            //indices.WriteRange(new short[] { 7, 2, 1 });

            //// back
            //indices.WriteRange(new short[] { 3, 4, 5 });
            //indices.WriteRange(new short[] { 5, 0, 3 });

            //// bottom
            //indices.WriteRange(new short[] { 6, 5, 4 });
            //indices.WriteRange(new short[] { 4, 7, 6 });

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
