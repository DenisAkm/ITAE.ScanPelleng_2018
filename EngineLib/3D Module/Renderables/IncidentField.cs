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
    public class IncidentField : Renderable
    {
        Effect effect;
        EffectTechnique technique;
        EffectPass pass;
        ShaderSignature inputSignature;

        InputLayout layout;
        EffectMatrixVariable tmat;
        int numVertices = 0;
        int vertexBufferSizeInBytes = 0;
        int vertexStride = Marshal.SizeOf(typeof(Vertex));
        DataStream vertices;
        SlimDX.Direct3D11.Buffer vertexBuffer;
        int color = Color.FromArgb(120, 170, 50).ToArgb();
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
        public IncidentField(int size, double theta, double phi, double polariz)
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

            numVertices = 4;
            vertexBufferSizeInBytes = vertexStride * numVertices;

            vertices = new DataStream(vertexBufferSizeInBytes, true, true);

            
            double p1x = Math.Sin(theta * pi / 180) * Math.Cos(phi * pi / 180);
            double p1y = Math.Sin(theta * pi / 180) * Math.Sin(phi * pi / 180);
            double p1z = Math.Cos(theta * pi / 180);
            DVector k = new DVector(p1x, p1y, p1z);
            k.Normalize();
            
            int length = size * 20;            

            Point3D P0 = new Point3D(size * 200 * k);
            Point3D P1 = new Point3D(size * 180 * k);
            
            

            DVector v_phi = new DVector(Math.Sin(phi * pi / 180), -Math.Cos(phi * pi / 180), 0);
            DVector v_theta = new DVector(-Math.Cos(theta * pi / 180) * Math.Cos(phi * pi / 180), -Math.Cos(theta * pi / 180) * Math.Sin(phi * pi / 180), Math.Sin(theta * pi / 180));
            DVector v = -Math.Sin(polariz * pi / 180) * v_phi + Math.Cos(polariz * pi / 180) * v_theta;
            v.Normalize();
            Point3D P2 = new Point3D(length*v/2);
            P2 = P2 + P0;

            vertices.Write(new Vertex(new Vector3((float)P0.Y, (float)P0.X, (float)P0.Z), color));
            vertices.Write(new Vertex(new Vector3((float)P1.Y, (float)P1.X, (float)P1.Z), color));

            vertices.Write(new Vertex(new Vector3((float)P0.Y, (float)P0.X, (float)P0.Z), color));
            vertices.Write(new Vertex(new Vector3((float)P2.Y, (float)P2.X, (float)P2.Z), color));
            
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
        }

        public override void render()
        {
            Matrix ViewPerspective = CameraManager.Instance.ViewPerspective;
            tmat.SetMatrix(ViewPerspective);

            DeviceManager.Instance.context.InputAssembler.InputLayout = layout;
            DeviceManager.Instance.context.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
            DeviceManager.Instance.context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, vertexStride, 0));
            

            technique = effect.GetTechniqueByName("Render");

            EffectTechniqueDescription techDesc;
            techDesc = technique.Description;

            for (int p = 0; p < techDesc.PassCount; ++p)
            {
                technique.GetPassByIndex(p).Apply(DeviceManager.Instance.context);
                DeviceManager.Instance.context.Draw(numVertices, 0);
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
