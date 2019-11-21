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
    public class Arc3d : Renderable
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

        //int color = Color.Cyan.ToArgb();

        public string Title { get; set; }
        float Rho;
        float a, b, c;
        double theta, phi;
        const double pi = 3.14159265358979323846;

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

        public Arc3d(int size, double thetaStart, double thetaFinish, double phiStart, double phiFinish, double step, int SoC, int color, string title = "")
        {
            Title = title;
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


            int theta_points = Convert.ToInt32((thetaFinish - thetaStart) / step) + 1;
            int phi_points = Convert.ToInt32((phiFinish - phiStart) / step) + 1;


           
            numVertices = theta_points * phi_points;
            vertexBufferSizeInBytes = vertexStride * numVertices;

            vertices = new DataStream(vertexBufferSizeInBytes, true, true);

            Rho = 250 * size;


            for (int i_phi = 0; i_phi < phi_points; i_phi++)
            {
                phi = (phiStart + i_phi * step) * pi / 180;
                for (int i_theta = 0; i_theta < theta_points; i_theta++)
                {
                    theta = (thetaStart + i_theta * step) * pi / 180;

                    if (SoC == 1)
                    {
                        c = Rho * (float)(Math.Sin(theta) * Math.Cos(phi));
                        a = Rho * (float)(Math.Sin(theta) * Math.Sin(phi));
                        b = Rho * (float)(Math.Cos(theta));    
                    }
                    else if (SoC == 2)
                    {
                        b = Rho * (float)(Math.Sin(theta) * Math.Cos(phi));
                        c = Rho * (float)(Math.Sin(theta) * Math.Sin(phi));
                        a = Rho * (float)(Math.Cos(theta));    
                    }
                    else
                    {
                        a = Rho * (float)(Math.Sin(theta) * Math.Cos(phi));
                        b = Rho * (float)(Math.Sin(theta) * Math.Sin(phi));
                        c = Rho * (float)(Math.Cos(theta));    
                    }
                    

                    vertices.Write(new Vertex(new Vector3(b, a, c), color));
                }
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

            numIndices = numVertices;
            indexBufferSizeInBytes = numIndices * indexStride;

            indices = new DataStream(indexBufferSizeInBytes, true, true);

            for (int i = 0; i < numVertices; i++)
            {
                indices.Write((short)i);
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
            DeviceManager.Instance.context.InputAssembler.PrimitiveTopology = PrimitiveTopology.PointList;
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


