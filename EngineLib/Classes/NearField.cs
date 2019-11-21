using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Integral
{
    public class NearField
    {
        double Dimention = 1;
        double pi = Math.PI;
        readonly Complex imOne = new Complex(0, 1);      // мнимая единица        
        readonly Complex imOneMin = new Complex(0, -1);      // минус мнимая единица  
        readonly double Frequency;
        readonly Complex Ekoeff;                                     // 1/i*omega*E_0
        readonly Complex Mukoeff;                                    // -1/i*omega*E_0
        readonly Complex iOmega;                                            //2 * pi * frequency;
        readonly double K_0;
        readonly double K2;

        private NearFieldElement[] nearfieldElement;

        public NearFieldElement this[int index]
        {
            get
            {
                return nearfieldElement[index];
            }
            set
            {
                nearfieldElement[index] = value;
            }
        }
        public CVector FindField(Line l)
        {
            int number = CountElements;
            CVector answer = new CVector();
            for (int i = 0; i < number; i++)
            {
                Line segment = nearfieldElement[i].Element;
                if (l.Index == segment.Index)
                {
                    answer = nearfieldElement[i].E;
                    break;
                }
            }
            return answer;
        }
        public int CountElements
        {
            get 
            {
                return nearfieldElement.Length;
            }
        }
        public NearField(Apperture antenna, List<WireMesh> wires, double freq)
        {         
            Complex ex, ey, ez, hx, hy, hz;            

            Frequency = freq;
            K_0 = 2 * pi * freq / Constant.c;      // волновое число 2pi/lambda
            K2 = K_0 * K_0;
            iOmega = 2 * pi * imOne * freq;
            Ekoeff = (-1.0) / (iOmega * Constant.E_0);          // 1/i*omega*E_0
            Mukoeff = (-1.0) / (iOmega * Constant.Mu_0);         // -1/i*omega*Mu_0

            int numberOfFinalElements = 0;
            for (int i = 0; i < wires.Count; i++)
            {
                numberOfFinalElements += wires[i].CountLines;
            }
             
            nearfieldElement = new NearFieldElement[numberOfFinalElements];

            int el = 0;
            for (int k = 0; k < wires.Count; k++)
            {
                for (int j = 0; j < wires[k].CountLines; j++)
                {
                    ex = new Complex(0, 0);
                    ey = new Complex(0, 0);
                    ez = new Complex(0, 0);

                    hx = new Complex(0, 0);
                    hy = new Complex(0, 0);
                    hz = new Complex(0, 0);


                    NearFieldElement Element = new NearFieldElement();
                    int numberOfAntennaTriangles = antenna.Count;


                    for (int i = 0; i < numberOfAntennaTriangles; i++)
                    {
                        CVector icur = new CVector(antenna.ElectricCurrent[i].X, antenna.ElectricCurrent[i].Y, antenna.ElectricCurrent[i].Z);
                        CVector mcur = new CVector(antenna.MagneticCurrent[i].X, antenna.MagneticCurrent[i].Y, antenna.MagneticCurrent[i].Z);

                        NearFieldElement FieldC = ElementFieldCalcP(icur, mcur, antenna.Triangles[i], wires[k].Lines[j]);

                        Element += FieldC;
                    }

                    nearfieldElement[el] = Element;
                    el++;
                }
            }         
        }

        public NearField(Apperture antenna, List<WireMesh> wires, double freq, int proc, TextBox info)
        {
            info.AppendText("Расчет ближнего поля" + Environment.NewLine);
            Complex ex, ey, ez, hx, hy, hz;

            Frequency = freq;
            K_0 = 2 * pi * freq / Constant.c;      // волновое число 2pi/lambda
            K2 = K_0 * K_0;
            iOmega = 2 * pi * imOne * freq;
            Ekoeff = (-1.0) / (iOmega * Constant.E_0);          // 1/i*omega*E_0
            Mukoeff = (-1.0) / (iOmega * Constant.Mu_0);         // -1/i*omega*Mu_0

            int numberOfFinalElements = 0;
            for (int i = 0; i < wires.Count; i++)
            {
                numberOfFinalElements += wires[i].CountLines;
            }

            List<Line> sumWires = new List<Line>();

            for (int k = 0; k < wires.Count; k++)
            {
                for (int s = 0; s < wires[k].CountLines; s++)
			    {
                    sumWires.Add(wires[k].Lines[s]);
                }                
            }

            nearfieldElement = new NearFieldElement[numberOfFinalElements];

            ParallelOptions op = new ParallelOptions() { MaxDegreeOfParallelism = proc };
            Parallel.For(0, sumWires.Count, op, j =>
            {
                ex = new Complex(0, 0);
                ey = new Complex(0, 0);
                ez = new Complex(0, 0);

                hx = new Complex(0, 0);
                hy = new Complex(0, 0);
                hz = new Complex(0, 0);


                NearFieldElement Element = new NearFieldElement();
                int numberOfAntennaTriangles = antenna.Count;


                for (int i = 0; i < numberOfAntennaTriangles; i++)
                {
                    CVector icur = new CVector(antenna.ElectricCurrent[i].X, antenna.ElectricCurrent[i].Y, antenna.ElectricCurrent[i].Z);
                    CVector mcur = new CVector(antenna.MagneticCurrent[i].X, antenna.MagneticCurrent[i].Y, antenna.MagneticCurrent[i].Z);

                    NearFieldElement FieldC = ElementFieldCalcP(icur, mcur, antenna.Triangles[i], sumWires[j]);

                    Element += FieldC;
                }

                nearfieldElement[j] = Element;

            });
            
        }

        private NearFieldElement ElementFieldCalcP(CVector i, CVector m, Triangle triangle, Line line)
        {
            Point3D P1 = triangle.Center;
            Point3D P2 = line.Center;
            double square = triangle.Square;
            //
            //  Переменные
            //          
            double xprimeC = P1.X * Dimention;
            double yprimeC = P1.Y * Dimention;
            double zprimeC = P1.Z * Dimention;

            double xsecondC = P2.X * Dimention;
            double ysecondC = P2.Y * Dimention;
            double zsecondC = P2.Z * Dimention;

            double x_x0 = xprimeC - xsecondC;
            double y_y0 = yprimeC - ysecondC;
            double z_z0 = zprimeC - zsecondC;
            double x2 = x_x0 * x_x0;
            double y2 = y_y0 * y_y0;
            double z2 = z_z0 * z_z0;

            double r = Math.Sqrt(x2 + y2 + z2);
            Complex exp_ikr = new Complex(Math.Cos(K_0 * r), -Math.Sin(K_0 * r));

            Complex funG = exp_ikr / (4 * pi * r);
            double r2 = r * r;
            double r3 = r * r2;
            double r4 = r2 * r2;

            //
            //НАЧАЛО КОСТИНЫ ФОРМУЛЫ
            //
            Complex coeffA = imOneMin * K_0 / r - 1.0 / r2;
            Complex coeffB = (3.0 + 3.0 * imOne * K_0 * r - K2 * r2) / r4;

            Complex dx = x_x0 * coeffA * funG;
            Complex dy = y_y0 * coeffA * funG;
            Complex dz = z_z0 * coeffA * funG;

            Complex dxx = (x2 * coeffB + coeffA) * funG;
            Complex dyy = (y2 * coeffB + coeffA) * funG;
            Complex dzz = (z2 * coeffB + coeffA) * funG;

            Complex dxy = (x_x0 * y_y0 * coeffB) * funG;
            Complex dxz = (x_x0 * z_z0 * coeffB) * funG;
            Complex dyz = (y_y0 * z_z0 * coeffB) * funG;
            //
            // КОНЕЦ
            //

            Complex exC = Ekoeff * (K2 * i.X * funG + i.X * dxx + i.Y * dxy + i.Z * dxz) * square - (m.Z * dy - m.Y * dz) * square; //-
            Complex eyC = Ekoeff * (K2 * i.Y * funG + i.Y * dyy + i.Z * dyz + i.X * dxy) * square - (m.X * dz - m.Z * dx) * square; //-
            Complex ezC = Ekoeff * (K2 * i.Z * funG + i.Z * dzz + i.X * dxz + i.Y * dyz) * square - (m.Y * dx - m.X * dy) * square; //-

            Complex hxC = Mukoeff * (K2 * m.X * funG + m.X * dxx + m.Y * dxy + m.Z * dxz) * square + (i.Z * dy - i.Y * dz) * square; //+
            Complex hyC = Mukoeff * (K2 * m.Y * funG + m.X * dxy + m.Y * dyy + m.Z * dyz) * square + (i.X * dz - i.Z * dx) * square; //+
            Complex hzC = Mukoeff * (K2 * m.Z * funG + m.X * dxz + m.Y * dyz + m.Z * dzz) * square + (i.Y * dx - i.X * dy) * square; //+


            return new NearFieldElement(new CVector(exC, eyC, ezC), new CVector(hxC, hyC, hzC), line);
        }
    }

    public class NearFieldElement
    {

        public CVector E { get; set; }
        public CVector H { get; set; }
        public Line Element { get; set; }

        public NearFieldElement()
        {
            E = new CVector(0, 0, 0);
            H = new CVector(0, 0, 0);            
        }

        public NearFieldElement(CVector e, CVector h, Line l)
        {
            E = e;
            H = h;
            Element = l;
        }
        
        public static NearFieldElement operator + (NearFieldElement a, NearFieldElement b)
        {
            NearFieldElement answer = new NearFieldElement();
            answer.E = a.E + b.E;
            answer.H = a.H + b.H;
            answer.Element = b.Element;
            return answer;
        }
    }
}
