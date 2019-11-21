using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Integral
{
    public class Polyline : Renderable
    {
        public string Name { get; set; }

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
        int size = 1000;
        int indexBufferSizeInBytes = 0;
        int vertexBufferSizeInBytes = 0;

        int color = Color.FromArgb(255, 255, 255).ToArgb();
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
        public Polyline(WireMesh geom)
        {
            Name = geom.Name;
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

            tmat = effect.GetVariableByName("gWVP").AsMatrix();
                        
            numVertices = geom.CountPoints;
            vertexBufferSizeInBytes = vertexStride * numVertices;

            vertices = new DataStream(vertexBufferSizeInBytes, true, true);

            List<Point3D> ListP = geom.Points;
            int number = ListP.Count;
            for (int i = 0; i < number; i++)
            {
                vertices.Write(new Vertex(new Vector3(size * (float)ListP[i].Y, size * (float)ListP[i].X, size * (float)ListP[i].Z), color));            
            }
            
            vertices.Position = 0;

            // create the vertex layout and buffer
            var elements = new[] { 
                new InputElement("POSITION", 0, Format.R32G32B32_Float, 0),
                new InputElement("COLOR", 0, Format.B8G8R8A8_UNorm, 12, 0) 
            };
            layout = new InputLayout(DeviceManager.Instance.device, inputSignature, elements);
            vertexBuffer = new SlimDX.Direct3D11.Buffer(DeviceManager.Instance.device, vertices, vertexBufferSizeInBytes, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

            numIndices = 2 * geom.CountLines * 2;
            indexStride = Marshal.SizeOf(typeof(short)); // 2 bytes
            indexBufferSizeInBytes = numIndices * indexStride;

            
            indices = new DataStream(indexBufferSizeInBytes, true, true);

            List<Index2> ListI = geom.Indeces;
            for (int i = 0; i < ListI.Count; i++)
            {
                short d1 = (short)(Convert.ToDouble(ListI[i].I1) - 1);
                short d2 = (short)(Convert.ToDouble(ListI[i].I2) - 1);
                

                //прямая сторона
                indices.WriteRange(new short[] { d1, d2});
                //обратная сторона
                indices.WriteRange(new short[] { d1, d2});
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

            // configure the Input Assembler portion of the pipeline with the vertex data
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
