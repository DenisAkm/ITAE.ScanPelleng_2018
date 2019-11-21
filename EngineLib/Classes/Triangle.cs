using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Integral
{
    public class Triangle
    {
        public Point3D V1;
        public Point3D V2;
        public Point3D V3;

        int index;        

        public Triangle(Point3D p1, Point3D p2, Point3D p3, int i = 0)
        {
            V1 = p1;
            V2 = p2;
            V3 = p3;
            index = i;
        }
        public static bool Adjucted(Triangle tr1, Triangle tr2)
        {
            int tr1_v1 = tr1.V1.Index;
            int tr1_v2 = tr1.V2.Index;
            int tr1_v3 = tr1.V3.Index;

            int tr2_v1 = tr2.V1.Index;
            int tr2_v2 = tr2.V2.Index;
            int tr2_v3 = tr2.V3.Index;

            int count = 0;
            if (tr1_v1 == tr2_v1 || tr1_v1 == tr2_v2 || tr1_v1 == tr2_v3)
            {
                count++;
            }
            if (tr1_v2 == tr2_v1 || tr1_v2 == tr2_v2 || tr1_v2 == tr2_v3)
            {
                count++;
            }
            if (tr1_v3 == tr2_v1 || tr1_v3 == tr2_v2 || tr1_v3 == tr2_v3)
            {
                count++;
            }

            if (count == 2)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public Point3D Center
        {
            get 
            {
                return new Point3D((V1.X + V2.X + V3.X) / 3, (V1.Y + V2.Y + V3.Y) / 3, (V1.Z + V2.Z + V3.Z) / 3);
            }
        }

        public double Square
        {
            get
            {
                double a = Math.Sqrt(Math.Pow(V2.X - V1.X, 2) + Math.Pow(V2.Y - V1.Y, 2) + Math.Pow(V2.Z - V1.Z, 2));
                double b = Math.Sqrt(Math.Pow(V3.X - V2.X, 2) + Math.Pow(V3.Y - V2.Y, 2) + Math.Pow(V3.Z - V2.Z, 2));
                double c = Math.Sqrt(Math.Pow(V1.X - V3.X, 2) + Math.Pow(V1.Y - V3.Y, 2) + Math.Pow(V1.Z - V3.Z, 2));
                double p = (a + b + c) / 2;

                return Math.Sqrt((p - a) * (p - b) * (p - c) * p);
            }
        }
        private DVector Norma    //  Вычисление нормали треуголька
        {
            get
            {
                //double a = (V2.Y - V1.Y) * (V3.Z - V1.Z) - (V3.Y - V1.Y) * (V2.Z - V1.Z);                                                                  // коэффициенты плоскости
                //double b = (V3.X - V1.X) * (V2.Z - V1.Z) - (V2.X - V1.X) * (V3.Z - V1.Z);                                                                  // Ax+By+Cz+D=0
                //double c = (V2.X - V1.X) * (V3.Y - V1.Y) - (V2.Y - V1.Y) * (V3.X - V1.X);
                //double d = (V3.X - V1.X) * (V2.Y * V1.Z - V1.Y * V2.Z) + (V3.Y - V1.Y) * (V1.X * V2.Z - V2.X * V1.Z) + (V3.Z - V1.Z) * (V2.X * V1.Y - V1.X * V2.Y);
                //double length = Math.Sqrt(a * a + b * b + c * c);
                //double var = a * Center.X + b * Center.Y + c * Center.Z + d;

                DVector norma = new DVector();

                //if (d >= 0) //Math.Round(var, 1) >= 0 //a * center.X + b * center.Y + c * center.Z + 
                //{
                //    norma.X = a / length;
                //    norma.Y = b / length;
                //    norma.Z = c / length;
                //}
                //else
                //{
                //    norma.X = (-1) * a / length;
                //    norma.Y = (-1) * b / length;
                //    norma.Z = (-1) * c / length;
                //}
                return norma; 
            }
        }
    }
    
}
