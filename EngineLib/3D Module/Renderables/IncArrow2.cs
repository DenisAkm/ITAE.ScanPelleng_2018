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
    public class IncArrow2 : Renderable
    {
        Effect effect;
        EffectTechnique technique;
        EffectPass pass;
        ShaderSignature inputSignature;

        InputLayout layout;
        EffectMatrixVariable tmat;
        int numVertices = 0;
        int numIndices = 0;
        int vertexBufferSizeInBytes = 0;
        int indexBufferSizeInBytes = 0;
        int vertexStride = Marshal.SizeOf(typeof(Vertex));
        int indexStride = Marshal.SizeOf(typeof(short));
        DataStream vertices;
        DataStream indices;
        SlimDX.Direct3D11.Buffer vertexBuffer;
        SlimDX.Direct3D11.Buffer indexBuffer;
        int color1 = Color.FromArgb(120, 170, 50).ToArgb();
        int color2 = Color.FromArgb(120, 170, 50).ToArgb();
        const double pi = Math.PI;
        int increas = 60;
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
        public IncArrow2(int size, double theta, double phi, double polariz)
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

            numVertices = 2 * (2 * (360 / increas + 1) + (360 / increas + 1) + 1);
            vertexBufferSizeInBytes = vertexStride * numVertices;

            vertices = new DataStream(vertexBufferSizeInBytes, true, true);

            numIndices = 2 * 2 * 3 * 3 * 360 / increas;

            indexBufferSizeInBytes = numIndices * indexStride;

            indices = new DataStream(indexBufferSizeInBytes, true, true);


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
            Point3D P2 = new Point3D(length / 2 * v);
            P2 = P2 + P0;

            float radius = size * 1.1f;

            DVector cross = DVector.Cross(new DVector(1, 0, 0), k);
            if (cross.Module < 0.05)
            {
                cross = DVector.Cross(new DVector(0, 1, 0), k);
            }
            cross.Normalize();
            DVector radiusVector = radius * cross;
            Point3D r0_up = P0 + new Point3D(radiusVector);
            Point3D r0_dawn = P1 + new Point3D(radiusVector);

            Point3D a0 = P1 + new Point3D(2.5 * radiusVector);
            Point3D a = P1 + (-1) * new Point3D(size * 10 * k);
            Matrix rotM = Matrix.RotationAxis(new Vector3((float)k.X, (float)k.Y, (float)k.Z), (float)(increas * pi / 180));
            

            vertices.Write(new Vertex(new Vector3((float)r0_up.Y, (float)r0_up.X, (float)r0_up.Z), color1));
            vertices.Write(new Vertex(new Vector3((float)r0_dawn.Y, (float)r0_dawn.X, (float)r0_dawn.Z), color1));
            int count = 2;
            for (int i = 0; i < 360 / increas; i++)
            {

                Point3D r1_up = rotM * r0_up;
                Point3D r1_dawn = rotM * r0_dawn;
                                               
                vertices.Write(new Vertex(new Vector3((float)r1_up.Y, (float)r1_up.X, (float)r1_up.Z), color1));
                vertices.Write(new Vertex(new Vector3((float)r1_dawn.Y, (float)r1_dawn.X, (float)r1_dawn.Z), color1));
                                
                short offset = (short)(2 * i);
                indices.WriteRange(new short[] { (short)(offset + 0), (short)(offset + 1), (short)(offset + 2) });
                indices.WriteRange(new short[] { (short)(offset + 1), (short)(offset + 2), (short)(offset + 3) });

                indices.WriteRange(new short[] { (short)(offset + 2), (short)(offset + 1), (short)(offset + 0) });
                indices.WriteRange(new short[] { (short)(offset + 3), (short)(offset + 2), (short)(offset + 1) });
                
                r0_up = new Point3D(r1_up);
                r0_dawn = new Point3D(r1_dawn);
                count += 2;
            }            

            vertices.Write(new Vertex(new Vector3((float)a.Y, (float)a.X, (float)a.Z), color1));
            short indexCoan = (short)(count);
            count++;
            vertices.Write(new Vertex(new Vector3((float)a0.Y, (float)a0.X, (float)a0.Z), color1));
            count++;
            int stocount = count;
            for (int i = 0; i < 360 / increas; i++)
            {                
                Point3D a1 = rotM * a0;

                short offset = (short)(i + stocount);

                vertices.Write(new Vertex(new Vector3((float)a1.Y, (float)a1.X, (float)a1.Z), color1));

                
                indices.WriteRange(new short[] { (short)(offset + 0), (short)(offset - 1), (short)(indexCoan) });
                indices.WriteRange(new short[] { (short)(indexCoan), (short)(offset - 1), (short)(offset + 0) });
                a0 = new Point3D(a1);
                count++;
            }



            cross = DVector.Cross(new DVector(1, 0, 0), v);
            if (cross.Module < 0.05)
            {
                cross = DVector.Cross(new DVector(0, 1, 0), v);
            }
            cross.Normalize();
            radiusVector = radius * cross;
            r0_up = P2 + new Point3D(radiusVector);
            r0_dawn = P0 + new Point3D(radiusVector);

            a0 = P2 + new Point3D(2.5 * radiusVector);
            a = P2 + new Point3D(size * 10 * v);
            rotM = Matrix.RotationAxis(new Vector3((float)v.X, (float)v.Y, (float)v.Z), (float)(increas * pi / 180));


            vertices.Write(new Vertex(new Vector3((float)r0_up.Y, (float)r0_up.X, (float)r0_up.Z), color1));
            vertices.Write(new Vertex(new Vector3((float)r0_dawn.Y, (float)r0_dawn.X, (float)r0_dawn.Z), color1));
            
            stocount = count;
            for (int i = 0; i < 360 / increas; i++)
            {

                Point3D r1_up = rotM * (r0_up - P2) + P2;
                Point3D r1_dawn = rotM * (r0_dawn - P2) + P2;
                


                vertices.Write(new Vertex(new Vector3((float)r1_up.Y, (float)r1_up.X, (float)r1_up.Z), color1));
                vertices.Write(new Vertex(new Vector3((float)r1_dawn.Y, (float)r1_dawn.X, (float)r1_dawn.Z), color1));

                              
                short offset = (short)(2 * i + stocount);

                indices.WriteRange(new short[] { (short)(offset + 0), (short)(offset + 1), (short)(offset + 2) });
                indices.WriteRange(new short[] { (short)(offset + 2), (short)(offset + 1), (short)(offset + 3) });

                indices.WriteRange(new short[] { (short)(offset + 2), (short)(offset + 1), (short)(offset + 0) });
                indices.WriteRange(new short[] { (short)(offset + 3), (short)(offset + 1), (short)(offset + 2) });

                
                r0_up = new Point3D(r1_up);
                r0_dawn = new Point3D(r1_dawn);
                count += 2;
            }

            count += 2;
            vertices.Write(new Vertex(new Vector3((float)a.Y, (float)a.X, (float)a.Z), color1));
            indexCoan = (short)(count);
            count++;
            vertices.Write(new Vertex(new Vector3((float)a0.Y, (float)a0.X, (float)a0.Z), color1));
            stocount = count;
            for (int i = 0; i < 360 / increas; i++)
            {
                Point3D a1 = rotM * (a0 - P2) + P2;

                vertices.Write(new Vertex(new Vector3((float)a1.Y, (float)a1.X, (float)a1.Z), color1));

                short offset = (short)(stocount + i);
                indices.WriteRange(new short[] { (short)(offset + 0), (short)(offset + 1), (short)(indexCoan) });
                indices.WriteRange(new short[] { (short)(indexCoan), (short)(offset + 1), (short)(offset + 0) });
                a0 = new Point3D(a1);
                count++;
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
            DeviceManager.Instance.context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
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
