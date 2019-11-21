using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.D3DCompiler;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using System.Runtime.InteropServices;
using System.Drawing;


namespace Integral
{
    public class Grid : Renderable
    {
        SlimDX.Direct3D11.Buffer vertexBuffer;
        DataStream vertices;

        InputLayout layout;

        int numVertices = 0;

        ShaderSignature inputSignature;
        EffectTechnique technique;
        EffectPass pass;

        Effect effect;
        EffectMatrixVariable tmat;


        public Grid(int cellsPerSide, float cellSize)
        {
            try
            {
                using (ShaderBytecode effectByteCode = ShaderBytecode.CompileFromFile(
                    "Shaders/transformEffect.fx",
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


            int numLines = cellsPerSide + 1;
            float lineLength = cellsPerSide * cellSize;

            float xStart = -lineLength / 2.0f;
            float yStart = -lineLength / 2.0f;

            float xCurrent = xStart;
            float yCurrent = yStart;

            numVertices = 2 * 2 * numLines;
            int SizeInBytes = 12 * numVertices;

            vertices = new DataStream(SizeInBytes, true, true);

            for (int y = 0; y < numLines; y++)
            {
                vertices.Write(new Vector3(xCurrent, yStart, 0));
                vertices.Write(new Vector3(xCurrent, yStart + lineLength, 0));
                xCurrent += cellSize;
            }

            for (int x = 0; x < numLines; x++)
            {
                vertices.Write(new Vector3(xStart, yCurrent, 0 ));
                vertices.Write(new Vector3(xStart + lineLength, yCurrent, 0));
                yCurrent += cellSize;
            }

            vertices.Position = 0;

            // create the vertex layout and buffer
            var elements = new[] { new InputElement("POSITION", 0, Format.R32G32B32_Float, 0) };
            layout = new InputLayout(DeviceManager.Instance.device, inputSignature, elements);
            vertexBuffer = new SlimDX.Direct3D11.Buffer(DeviceManager.Instance.device, vertices, SizeInBytes, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);


        }

        public override void render()
        {
            Matrix ViewPerspective = CameraManager.Instance.ViewPerspective;
            tmat.SetMatrix(ViewPerspective);

            // configure the Input Assembler portion of the pipeline with the vertex data
            DeviceManager.Instance.context.InputAssembler.InputLayout = layout;
            DeviceManager.Instance.context.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
            DeviceManager.Instance.context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, 12, 0));

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
            inputSignature.Dispose();
        }
    }
}
