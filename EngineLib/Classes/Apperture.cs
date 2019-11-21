using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Integral
{
    public class Apperture
    {
        readonly public List<Point3D> points;
        readonly public List<Index3> indexes;

        SurfaceCurrent electricCurrent;
        SurfaceCurrent magneticCurrent;

        public string workPlane = "";
        Point3D Center { get; set; }
        double R { get; set; }
        List<Triangle> triangles;

        
        public Apperture(List<double> x, List<double> y, List<double> z, List<int> i1, List<int> i2, List<int> i3)
        {
            int count = x.Count;
            points = new List<Point3D>(count);
            for (int i = 0; i < count; i++)
            {
                points.Add(new Point3D(x[i], y[i], z[i]));
            }

            count = i1.Count;
            indexes = new List<Index3>(count);
            for (int i = 0; i < count; i++)
            {
                indexes.Add(new Index3(i1[i], i2[i], i3[i]));
            }

            triangles = new List<Triangle>(count);

            for (int i = 0; i < count; i++)
            {
                Point3D p1 = points[i1[i] - 1];
                Point3D p2 = points[i2[i] - 1];
                Point3D p3 = points[i3[i] - 1];

                triangles.Add(new Triangle(p1, p2, p3, i));
            }
            FindParametersForRoundApperture();
        }

        
        public Apperture(Apperture original)
        {
            this.points = new List<Point3D>(original.points.Count);
            for (int i = 0; i < original.points.Count; i++)
            {
                this.points.Add(original.points[i]);
            }
            this.indexes = new List<Index3>(original.points.Count);
            for (int i = 0; i < original.indexes.Count; i++)
            {
                this.indexes.Add(original.indexes[i]);
            }
            this.triangles = new List<Triangle>(original.triangles.Count);
            for (int i = 0; i < original.triangles.Count; i++)
            {
                this.triangles.Add(original.triangles[i]);
            }

            this.Center = original.Center;
            this.R = original.R;
            Complex[] I_Current = new Complex[original.electricCurrent.Count];
            Complex[] M_Current = new Complex[original.electricCurrent.Count];
            DVector[] I_Vector = new DVector[original.electricCurrent.Count];
            DVector[] M_Vector = new DVector[original.electricCurrent.Count];
            Triangle[] Triangles = new Triangle[original.electricCurrent.Count];

            for (int i = 0; i < original.electricCurrent.Count; i++)
            {
                I_Current[i] = original.electricCurrent[i].Value;
                M_Current[i] = original.magneticCurrent[i].Value;
                I_Vector[i] = original.electricCurrent[i].DVector;
                M_Vector[i] = original.magneticCurrent[i].DVector;
                Triangles[i] = original.electricCurrent[i].Segment;
            }
            this.electricCurrent = new SurfaceCurrent(I_Current, I_Vector, Triangles);
            this.magneticCurrent = new SurfaceCurrent(M_Current, M_Vector, Triangles);
            this.workPlane = original.workPlane;
        }
        public void GenerateConstantCurrentApperture(DVector e_i, DVector e_m, double factor = 1, double phase = 0)
        {
            Complex[] I = new Complex[triangles.Count];
            Complex[] M = new Complex[triangles.Count];
            DVector[] VectorI = new DVector[triangles.Count];
            DVector[] VectorM = new DVector[triangles.Count];
            Triangle[] Segments = new Triangle[triangles.Count];            

            for (int n = 0; n < triangles.Count; n++)
            {
                Triangle segment = triangles[n];

                Complex i = factor / Constant.Z_0;
                Complex m = factor;

                I[n] = i;
                M[n] = m;
                VectorI[n] = e_i;
                VectorM[n] = e_m;
                Segments[n] = segment;
            }
            electricCurrent = new SurfaceCurrent(I, VectorI, Segments);
            magneticCurrent = new SurfaceCurrent(M, VectorM, Segments);
        }
        public void GenerateCosOnStepApperture(DVector e_i, DVector e_m, double delta, double factor = 1, double phase = 0)
        {            
            Complex[] I = new Complex[triangles.Count];
            Complex[] M = new Complex[triangles.Count];
            DVector[] VectorI = new DVector[triangles.Count];
            DVector[] VectorM = new DVector[triangles.Count];
            Triangle[] Segments = new Triangle[triangles.Count];
            Point3D center = Center;
            double Ra = R;

            for (int n = 0; n < triangles.Count; n++)
            {
                Triangle segment = triangles[n];
                Point3D position = segment.Center;


                double c1 = (1 + delta * Math.Cos(Math.PI * Point3D.Distance(position, center) / Ra)) / (1 + delta);
                Complex i = factor * c1 / Constant.Z_0;
                Complex m = factor * c1;

                I[n] = i;
                M[n] = m;
                VectorI[n] = e_i;
                VectorM[n] = e_m;
                Segments[n] = segment;
            }
            electricCurrent = new SurfaceCurrent(I, VectorI, Segments);
            magneticCurrent = new SurfaceCurrent(M, VectorM, Segments);
        }
        public void DifferenceRadiationPattern(int axisNumber = 1)
        {            
            double c = 1;
            Point3D center = Center;
            Complex[] I = new Complex[triangles.Count];
            Complex[] M = new Complex[triangles.Count];
            DVector[] VectorI = new DVector[triangles.Count];
            DVector[] VectorM = new DVector[triangles.Count];
            Triangle[] Segments = new Triangle[triangles.Count];

            for (int n = 0; n < triangles.Count; n++)
            {
                
                c = 1;                
                Point3D position = triangles[n].Center;
                

                if (axisNumber == 2 && position.X < center.X)
                {
                    c = -1;
                }
                if (axisNumber == 3 && position.Y < center.Y)
                {
                    c = -1;
                }
                if (axisNumber == 1 && position.Z < center.Z)
                {
                    c = -1;
                }
                
                if (c == -1)
                {
                    electricCurrent[n].Value *= -1;
                    magneticCurrent[n].Value *= -1;
                }
                
            }            
        }
        public void ApplyScanning(double f, double scanTheta, double scanPhi)
        {
            double K_0 = 2 * Math.PI * f / Constant.c;      // волновое число 2pi/lambda
            Complex c = new Complex();
            Point3D center = Center;            

            for (int n = 0; n < triangles.Count; n++)
            {
                Point3D p = triangles[n].Center;

                c = exp(K_0 * Math.Sin(scanTheta * Math.PI / 180) * Math.Cos(scanPhi * Math.PI / 180) * p.X) * exp(K_0 * Math.Sin(scanTheta * Math.PI / 180) * Math.Sin(scanPhi * Math.PI / 180) * p.Y) * exp(K_0 * Math.Cos(scanTheta * Math.PI / 180) * p.Z);                        
                //c = exp(K_0 * Math.Sin(scanPhi * Math.PI / 180) * Math.Cos(scanTheta * Math.PI / 180)* c2) * exp(K_0 * Math.Sin(scanTheta * Math.PI / 180) * c1);                
                //c = exp(K_0 * Math.Sin(scanPhi * Math.PI / 180) * coordinate);                

                electricCurrent[n].Value *= c;                
                magneticCurrent[n].Value *= c;                
            }
        }




        public void GenerateCurrentCosOld(double f, DVector e_i, DVector e_m, double scanTheta, double scanPhi, double factor = 1)
        {            
            double K_0 = 2 * Math.PI * f / Constant.c;      // волновое число 2pi/lambda

            double del = 0.5;
            Complex[] I = new Complex[triangles.Count];
            Complex[] M = new Complex[triangles.Count];    
            DVector[] VectorI = new DVector[triangles.Count];
            DVector[] VectorM = new DVector[triangles.Count];
            Triangle[] Segments = new Triangle[triangles.Count];
            double Ra = R;
            Point3D center = Center;

            //Complex c2 = exp(K_0 * Math.Sin(scanPhi * Math.PI / 180) * Math.Cos(scanTheta * Math.PI / 180)) * exp(K_0 * Math.Sin(scanTheta * Math.PI / 180));
            
            for (int n = 0; n < triangles.Count; n++)
            {
                Triangle segment = triangles[n];
                Point3D position = segment.Center;

                
                double c1 = (1 + del * Math.Cos(Math.PI * Point3D.Distance(position, center) / Ra)) / (1 + del);
                Complex c2 = exp(K_0 * Math.Sin(scanPhi * Math.PI / 180) * position.Y);
                Complex i = factor * c1 * c2 / Constant.Z_0;
                Complex m = factor * c1 * c2;

                I[n] = i;
                M[n] = m;
                VectorI[n] = e_i;
                VectorM[n] = e_m;
                Segments[n] = segment;
            }

            electricCurrent = new SurfaceCurrent(I, VectorI, Segments);
            magneticCurrent = new SurfaceCurrent(M, VectorM, Segments);
        }
        public void GenerateAzumutRDNOld(double f, DVector e_i, DVector e_m, double scanTheta, double scanPhi, double factor = 1)
        {
            double K_0 = 2 * Math.PI * f / Constant.c;      // волновое число 2pi/lambda

            double del = 0.5;
            Complex[] I = new Complex[triangles.Count];
            Complex[] M = new Complex[triangles.Count];
            DVector[] VectorI = new DVector[triangles.Count];
            DVector[] VectorM = new DVector[triangles.Count];
            Triangle[] Segments = new Triangle[triangles.Count];
            double Ra = R;
            Point3D center = Center;

            //Complex c2 = exp(K_0 * Math.Sin(scanPhi * Math.PI / 180) * Math.Cos(scanTheta * Math.PI / 180)) * exp(K_0 * Math.Sin(scanTheta * Math.PI / 180));

            for (int n = 0; n < triangles.Count; n++)
            {
                double c3 = 1;
                Triangle segment = triangles[n];
                Point3D position = segment.Center;

                if (position.Y < center.Y)
                {
                    c3 = -1;
                }
                double c1 = (1 + del * Math.Cos(Math.PI * Point3D.Distance(position, center) / Ra)) / (1 + del);
                Complex c2 = exp(K_0 * Math.Sin(scanPhi * Math.PI / 180) * position.Y);
                Complex i = factor * c1 * c2 * c3 / Constant.Z_0;
                Complex m = factor * c1 * c2 * c3;

                I[n] = i;
                M[n] = m;
                VectorI[n] = e_i;
                VectorM[n] = e_m;
                Segments[n] = segment;
            }

            electricCurrent = new SurfaceCurrent(I, VectorI, Segments);
            magneticCurrent = new SurfaceCurrent(M, VectorM, Segments);
        }
        public Complex exp(double x)
        {
            return new Complex(Math.Cos(x), -Math.Sin(x));
        }
        public void FindParametersForRoundApperture()
        {
           
                //double answer = 0;
                List<double> max = new List<double>() { points[0].X, points[0].Y, points[0].Z };
                List<double> min = new List<double>() { points[0].X, points[0].Y, points[0].Z };
               

                for (int i = 1; i < points.Count; i++)
                {
                    List<double> value = new List<double>() { points[i].X, points[i].Y, points[i].Z };
                    
                    for (int k = 0; k < 3; k++)
                    {
                        if (value[k] > max[k])
                        {
                            max[k] = value[k];
                        }
                        if (value[k] < min[k])
                        {
                            min[k] = value[k];
                        }                         
                    }                    
                }
                Center = new Point3D((max[0] + min[0]) / 2, (max[1] + min[1]) / 2, (max[2] + min[2]) / 2);

                double Rx = (max[0] - min[0])/2; 
                double Ry = (max[1] - min[1])/2;
                double Rz = (max[2] - min[2])/2;


                if (Rx > Ry && Rx > Rz)
                {
                    R = Rx;
                    if (Ry > Rz)
                    {
                        workPlane = "XY";
                    }
                    else
                    {
                        workPlane = "XZ";
                    }
                }
                else if (Rx > Ry && Rx < Rz)
                {
                    R = Rz;
                    workPlane = "XZ";                    
                }
                else if (Rx < Ry && Ry > Rz)
                {
                    R = Ry;
                    if (Rx > Rz)
                    {
                        workPlane = "XY";
                    }
                    else
                    {
                        workPlane = "YZ";
                    }
                }
                else if (Rx < Ry && Ry < Rz)
                {
                    R = Rz;
                    workPlane = "XZ";                    
                }
                else if (Rx == Ry && Rx > Rz)
                {
                    R = Rx;
                    workPlane = "XY";
                }
                else if (Rx == Ry && Rx < Rz)
                {
                    R = Rz;
                    workPlane = "";
                }


                //if (Rx > Ry)
                //{
                //    if (Rx > Rz)
                //    {
                //        answer = Rx;
                //    }
                //    else
                //    {
                //        answer = Rz;
                //    }
                //}
                //else if (Rx < Ry)
                //{
                //    if (Ry > Rz)
                //    {
                //        answer = Ry;
                //    }
                //    else
                //    {
                //        answer = Rz;
                //    }
                //}
                //else
                //{
                //    if (Rx > Rz)
                //    {
                //        answer = Rx;
                //    }
                //    else if(Rx < Rz)
                //    {
                //        answer = Rz;
                //    }
                //}
                //R = answer;            
        }
        public List<double> ListX
        {
            get
            {
                int count = points.Count;
                List<double> x = new List<double>(count);
                for (int i = 0; i < count; i++)
                {
                    x.Add(points[i].X);
                }
                return x;
            }
        }
        public List<double> ListY
        {
            get
            {
                int count = points.Count;
                List<double> y = new List<double>(count);
                for (int i = 0; i < count; i++)
                {
                    y.Add(points[i].Y);
                }
                return y;
            }
        }
        public List<double> ListZ
        {
            get
            {
                int count = points.Count;
                List<double> z = new List<double>(count);
                for (int i = 0; i < count; i++)
                {
                    z.Add(points[i].Z);
                }
                return z;
            }
        }

        public List<Int32> ListI1
        {
            get
            {
                int count = indexes.Count;
                List<Int32> i1 = new List<Int32>(count);
                for (int i = 0; i < count; i++)
                {
                    i1.Add(indexes[i].i1);
                }
                return i1;
            }
        }
        public List<Int32> ListI2
        {
            get
            {
                int count = indexes.Count;
                List<Int32> i2 = new List<Int32>(count);
                for (int i = 0; i < count; i++)
                {
                    i2.Add(indexes[i].i2);
                }
                return i2;
            }
        }
        public List<Int32> ListI3
        {
            get
            {
                int count = indexes.Count;
                List<Int32> i3 = new List<Int32>(count);
                for (int i = 0; i < count; i++)
                {
                    i3.Add(indexes[i].i3);
                }
                return i3;
            }
        }

        public int Count
        {
            get 
            {
                return triangles.Count;
            }
        }

        public List<Triangle> Triangles
        {
            get 
            {
                return triangles;
            }
        }

        public SurfaceCurrent ElectricCurrent
        {
            get 
            {
                return electricCurrent;
            }
        }
        public SurfaceCurrent MagneticCurrent
        {
            get
            {
                return magneticCurrent;
            }
        }
    }   
}
