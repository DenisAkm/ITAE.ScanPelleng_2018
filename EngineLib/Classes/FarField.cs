using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Integral
{
    public class FarField
    {
        Complex imOne = Complex.ImaginaryOne;
        const double pi = Math.PI;
        const double E_0 = Constant.E_0;
        const double c0 = Constant.c;
        
        
        public string Name {get; set;}
        FarFieldElement[] fieldElement;
        public FarFieldElement this[int index]
        {
            get
            {
                return fieldElement[index];
            }
            set
            {
                fieldElement[index] = value;
            }
        }


        public FarField(int count)
        {
            Name = "no name";
            fieldElement = new FarFieldElement[count];
        }        
                
        public FarField(List<FarFieldElement> elements)
        {
            int count = elements.Count;
            this.fieldElement = new FarFieldElement[count];
            for (int i = 0; i < count; i++)
            {
                this.fieldElement[i] = elements[i];
            }
        }

        /// <summary>
        /// Трехмерная диаграмма направленности создаваемая аппертурой
        /// </summary>
        /// <param name="app"></param>
        /// <param name="f"></param>
        /// <param name="phiStart"></param>
        /// <param name="phiFinish"></param>
        /// <param name="thetaStart"></param>
        /// <param name="thetaFinish"></param>
        /// <param name="step"></param>
        public FarField(Apperture app, double f, double phiStart, double phiFinish, double thetaStart, double thetaFinish, double step, int systemOfCoord, bool local = false)
        {
            int numberPhi = Convert.ToInt32((phiFinish - phiStart) / step) + 1;
            int numberTheta = Convert.ToInt32((thetaFinish - thetaStart) / step) + 1;

            fieldElement = new FarFieldElement[numberPhi * numberTheta];
            int k = 0;
            for (int i = 0; i < numberPhi; i++)
            {
                double phi = phiStart + i * step; 
                for (int j = 0; j < numberTheta; j++)
                {
                    double theta = thetaStart + j * step;         
                    double thetaGlobal = MainForm.GetThetaGlobal(phi, theta, systemOfCoord);
                    double phiGlobal = MainForm.GetPhiGlobal(phi, theta, systemOfCoord);

                    fieldElement[k] = new FarFieldElement(app, f, thetaGlobal, phiGlobal);
                    if (local)
                    {
                        fieldElement[k].LocalPhi = phi;
                        fieldElement[k].LocalTheta = theta;
                    }
                    k++;
                }
            }
        }
        public FarField(int proc, Apperture app, double f, double phiStart, double phiFinish, double thetaStart, double thetaFinish, double step, int systemOfCoord, bool local = false)
        {
            int numberPhi = Convert.ToInt32((phiFinish - phiStart) / step) + 1;
            int numberTheta = Convert.ToInt32((thetaFinish - thetaStart) / step) + 1;

            fieldElement = new FarFieldElement[numberPhi * numberTheta];
            var options = new ParallelOptions() { MaxDegreeOfParallelism = proc };
            Parallel.For(0, numberTheta, options, j =>
                {
                    double theta = thetaStart + j * step;
                    for (int i = 0; i < numberPhi; i++)
                    {
                        double phi = phiStart + i * step;
                        double thetaGlobal = MainForm.GetThetaGlobal(phi, theta, systemOfCoord);
                        double phiGlobal = MainForm.GetPhiGlobal(phi, theta, systemOfCoord);

                        fieldElement[j + i * numberTheta] = new FarFieldElement(app, f, thetaGlobal, phiGlobal);
                        if (local)
                        {
                            fieldElement[j + i * numberTheta].LocalPhi = phi;
                            fieldElement[j + i * numberTheta].LocalTheta = theta;
                        }                        
                    }
                });
        }

        /// <summary>
        /// Трехмерная диаграмма направленности создаваемая проволочным источником
        /// </summary>
        /// <param name="source"></param>
        /// <param name="f"></param>
        /// <param name="phiStart"></param>
        /// <param name="phiFinish"></param>
        /// <param name="thetaStart"></param>
        /// <param name="thetaFinish"></param>
        /// <param name="step"></param>
        public FarField(WireCurrent source, double f, double phiStart, double phiFinish, double thetaStart, double thetaFinish, double step, int systemOfCoord, bool local = false)
        {
            int numberPhi = Convert.ToInt32((phiFinish - phiStart) / step) + 1;
            int numberTheta = Convert.ToInt32((thetaFinish - thetaStart) / step) + 1;

            fieldElement = new FarFieldElement[numberPhi * numberTheta];
            int k = 0;
            for (int i = 0; i < numberPhi; i++)
            {
                double phi = phiStart + i * step;
                for (int j = 0; j < numberTheta; j++)
                {                    
                    double theta = thetaStart + j * step;
                    if (theta == 90)
                    {
                        theta = 90;
                    }
                    double thetaGlobal = MainForm.GetThetaGlobal(phi, theta, systemOfCoord);
                    double phiGlobal = MainForm.GetPhiGlobal(phi, theta, systemOfCoord);
                    fieldElement[k] = new FarFieldElement(source, f, thetaGlobal, phiGlobal);
                    if (local)
                    {
                        fieldElement[k].LocalPhi = phi;
                        fieldElement[k].LocalTheta = theta;   
                    }
                    k++;
                }
            }
        }
        public FarField(int proc, WireCurrent source, double f, double phiStart, double phiFinish, double thetaStart, double thetaFinish, double step, int systemOfCoord, bool local = false)
        {
            int numberPhi = Convert.ToInt32((phiFinish - phiStart) / step) + 1;
            int numberTheta = Convert.ToInt32((thetaFinish - thetaStart) / step) + 1;

            fieldElement = new FarFieldElement[numberPhi * numberTheta];
            var options = new ParallelOptions() { MaxDegreeOfParallelism = proc };
            Parallel.For(0, numberTheta, options, j =>
            {
                double theta = thetaStart + j * step;
                for (int i = 0; i < numberPhi; i++)
                {
                    double phi = phiStart + i * step;

                    double thetaGlobal = MainForm.GetThetaGlobal(phi, theta, systemOfCoord);
                    double phiGlobal = MainForm.GetPhiGlobal(phi, theta, systemOfCoord);
                    fieldElement[j + i * numberTheta] = new FarFieldElement(source, f, thetaGlobal, phiGlobal);
                    if (local)
                    {
                        fieldElement[j + i * numberTheta].LocalPhi = phi;
                        fieldElement[j + i * numberTheta].LocalTheta = theta;
                    }
                }
            });
        }


        public static FarField operator +(FarField ff1, FarField ff2)
        {
            if (ff1.Count == ff2.Count)
            {
                FarField res = new FarField(ff1.Count);
                int Count = ff1.Count;
                for (int i = 0; i < Count; i++)
                {
                    res[i] = ff1[i] + ff2[i];
                }
                return res;
            }
            else
            {
                return new FarField(0);
            }
        }
        public int Count
        {
            get { return this.fieldElement.Length; }
        }

        
    }
    public class FarFieldElement
    {
        Complex imOne = Complex.ImaginaryOne;
        const double pi = Math.PI;
        const double E_0 = Constant.E_0;
        const double c0 = Constant.c;

        public double LocalPhi = 0;
        public double LocalTheta = 0;
        public Complex Ex { get; set; }
        public Complex Ey { get; set; }
        public Complex Ez { get; set; }

        public readonly double Phi;
        public readonly double Theta;
        public double Frequency { get; set; }
        
        public double Ephi
        {
            get
            {
                double phi = Phi * pi / 180;
                double theta = Theta * pi / 180;

                DVector v_phi = new DVector(-Math.Sin(phi), Math.Cos(phi), 0);
                v_phi.Normalize();
                Complex phiProjectionE = Projection(v_phi);
                return 20 * Math.Log10(phiProjectionE.Magnitude);
            }
        }
        public double Etheta
        {
            get
            {
                double phi = Phi * pi / 180;
                double theta = Theta * pi / 180;

                DVector v_theta = new DVector(-Math.Cos(theta) * Math.Cos(phi), -Math.Cos(theta) * Math.Sin(phi), Math.Sin(theta));
                v_theta.Normalize();
                Complex thetaProjectionE = Projection(v_theta);                
                return 20 * Math.Log10(thetaProjectionE.Magnitude);
            }
        }
        public double Etotal
        {
            get
            {
                double phi = Phi * pi / 180;
                double theta = Theta * pi / 180;

                DVector v_phi = new DVector(-Math.Sin(phi), Math.Cos(phi), 0);
                DVector v_theta = new DVector(-Math.Cos(theta) * Math.Cos(phi), -Math.Cos(theta) * Math.Sin(phi), Math.Sin(theta));
                v_phi.Normalize();
                v_theta.Normalize();
                Complex phiProjectionE = Projection(v_phi);
                Complex thetaProjectionE = Projection(v_theta);
                Complex totalProjectionE = Math.Sqrt(thetaProjectionE.Magnitude * thetaProjectionE.Magnitude + phiProjectionE.Magnitude * phiProjectionE.Magnitude);

                return 20 * Math.Log10(totalProjectionE.Magnitude);
            }
        }

        private Complex Projection(DVector vector)
        {
            Complex projVectorE = (this.Ex * vector.X + this.Ey * vector.Y + this.Ez * vector.Z);
            if (projVectorE.Magnitude <= 0.00001)
            {
                projVectorE = 0.00001;
            }
            return projVectorE;
        }

        public FarFieldElement(WireCurrent cur, double freq, double theta, double phi)
        {
            double Omega = 2 * pi * freq;
            double K_0 = Omega / c0;
            double K2 = K_0 * K_0;
            Complex ix, iy, iz;
            Frequency = freq;
            Complex Ekoeff = (1.0) / (4 * pi * imOne * Omega * E_0); //fourComx*piComx*imOne*omegaComx*E_0Comx
            Theta = theta;
            Phi = phi;

            int segmentNumber = cur.Count;


            Complex ex = new Complex(0, 0);
            Complex ey = new Complex(0, 0);
            Complex ez = new Complex(0, 0);

            double a = K_0 * Math.Sin(theta * pi / 180) * Math.Cos(phi * pi / 180);
            double b = K_0 * Math.Sin(theta * pi / 180) * Math.Sin(phi * pi / 180);
            double c = K_0 * Math.Cos(theta * pi / 180);

            double ab = a * b;
            double bc = b * c;
            double ac = a * c;

            //List<Line> lines = geom.Lines;
            for (int j = 0; j < segmentNumber; j++)
            {
                Line l = cur[j].Segment;
                Point3D p = l.Center;
                double length = l.Length;

                double arg = a * p.X + b * p.Y + c * p.Z;
                Complex cexp = new Complex(Math.Cos(arg), Math.Sin(arg));

                ix = cur[j].X;
                iy = cur[j].Y;
                iz = cur[j].Z;

                ex += Ekoeff * (K2 * ix - a * a * ix - ab * iy - ac * iz) * cexp * length;
                ey += Ekoeff * (K2 * iy - b * b * iy - bc * iz - ab * ix) * cexp * length;
                ez += Ekoeff * (K2 * iz - c * c * iz - ac * ix - bc * iy) * cexp * length;
            }

            Ex = ex;
            Ey = ey;
            Ez = ez;
        }

        public FarFieldElement(Apperture app, double freq, double theta, double phi)
        {
            double Omega = 2 * pi * freq;
            double K_0 = Omega / c0;
            double K2 = K_0 * K_0;
            Complex ix, iy, iz, mx, my, mz;
            this.Frequency = freq;
            Complex Ekoeff = (1.0) / (4 * pi * imOne * Omega * E_0); //fourComx*piComx*imOne*omegaComx*E_0Comx
            this.Theta = theta;
            this.Phi = phi;

            int segmentNumber = app.Count;

            Complex ex = new Complex(0, 0);
            Complex ey = new Complex(0, 0);
            Complex ez = new Complex(0, 0);
                        
            double a = K_0 * Math.Sin(theta * pi / 180) * Math.Cos(phi * pi / 180);
            double b = K_0 * Math.Sin(theta * pi / 180) * Math.Sin(phi * pi / 180);
            double c = K_0 * Math.Cos(theta * pi / 180);

            double ab = a * b;
            double bc = b * c;
            double ac = a * c;

            List<Triangle> triangles = app.Triangles;
            SurfaceCurrent i = app.ElectricCurrent;
            SurfaceCurrent m = app.MagneticCurrent;

            for (int j = 0; j < segmentNumber; j++)
            {                
                Triangle tr = triangles[j];
                Point3D p = tr.Center;
                double square = tr.Square;

                double arg = a * p.X + b * p.Y + c * p.Z;
                Complex cexp = new Complex(Math.Cos(arg), Math.Sin(arg));

                ix = i[j].X;
                iy = i[j].Y;
                iz = i[j].Z;

                mx = m[j].X;
                my = m[j].Y;
                mz = m[j].Z;

                ex += Ekoeff * (K2 * ix - a * a * ix - ab * iy - ac * iz) * cexp * square + imOne * (b * mz - c * my) * cexp * square / (4 * pi);//-
                ey += Ekoeff * (K2 * iy - b * b * iy - bc * iz - ab * ix) * cexp * square + imOne * (c * mx - a * mz) * cexp * square / (4 * pi);//-
                ez += Ekoeff * (K2 * iz - c * c * iz - ac * ix - bc * iy) * cexp * square + imOne * (a * my - b * mx) * cexp * square / (4 * pi);//-
                
            }

            Ex = ex;
            Ey = ey;
            Ez = ez;
        }
        
        public FarFieldElement(FarFieldElement f)
        {
            this.Ex = new Complex(f.Ex.Real,f.Ex.Imaginary);
            this.Ey = new Complex(f.Ey.Real,f.Ey.Imaginary);
            this.Ez = new Complex(f.Ez.Real,f.Ez.Imaginary);            
            this.Frequency = f.Frequency;
            this.Phi = f.Phi;
            this.Theta = f.Theta;
            this.LocalTheta = f.LocalTheta;
            this.LocalPhi = f.LocalPhi;
        }
        public static FarFieldElement operator +(FarFieldElement ffe1, FarFieldElement ffe2)
        {
            FarFieldElement res = new FarFieldElement(ffe2);
            res.Ex += ffe1.Ex;
            res.Ey += ffe1.Ey;
            res.Ez += ffe1.Ez;
            return res;
        }        

    }   
}
