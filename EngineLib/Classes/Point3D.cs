using SlimDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integral
{
    //
    // Точка в трёхмерном пространстве
    //
    
    public class Point3D
    {
        public int Index { get; set; }
        public static int GIndex = 0;
        public double X;
        public double Y;
        public double Z;             
        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
            Index = Point3D.GIndex;
            Point3D.GIndex++;
        }
        public Point3D(Point3D _p)
        {
            this.X = _p.X;
            this.Y = _p.Y;
            this.Z = _p.Z;            
        }

        public Point3D(DVector k)
        {
            X = k.X;
            Y = k.Y;
            Z = k.Z;
        }
        public static double Distance(Point3D a, Point3D b)
        {
            return Math.Sqrt(Math.Pow((a.X - b.X), 2) + Math.Pow((a.Y - b.Y), 2) + Math.Pow((a.Z - b.Z), 2));
        }
        public void Move(DVector vector)
        {
            this.X += vector.X;
            this.Y += vector.Y;
            this.Z += vector.Z;
        }

        public static bool operator > (Point3D a, Point3D b)
        {
            if (a.X + a.Y + a.Z > b.X + b.Y + b.Z)
            {
                return true;
            }
            else
            {
                return false;
            }            
        }
        public static bool operator <(Point3D a, Point3D b)
        {
            if (a.X + a.Y + a.Z < b.X + b.Y + b.Z)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Scale(double factor)
        {
            this.X = this.X * factor;
            this.Y = this.Y * factor;
            this.Z = this.Z * factor;
        }
        public static Point3D operator *(double a, Point3D P)
        {
            return new Point3D(P.X * a, P.Y * a, P.Z * a);
        }
        public static Point3D operator +(Point3D a, Point3D b)
        {
            return new Point3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        public static Point3D operator -(Point3D a, Point3D b)
        {
            return new Point3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
        public static Point3D operator +(Point3D a, DVector v)
        {
            return new Point3D(a.X + v.X, a.Y + v.Y, a.Z + v.Z);
        }


        public static Point3D operator /(Point3D P, double a)
        {
            return new Point3D(P.X / a, P.Y / a, P.Z / a);
        }
        
        public static Point3D GetMiddlePoint(Point3D a, Point3D b)
        {
            Point3D c = (a + b) / 2;
            return c;
        }
        public static Point3D operator *(Matrix M, Point3D p)
        {
            double x = p.X * M.M11 + p.Y * M.M12 + p.Z * M.M13;
            double y = p.X * M.M21 + p.Y * M.M22 + p.Z * M.M23;
            double z = p.X * M.M31 + p.Y * M.M32 + p.Z * M.M33;
            return new Point3D(x, y, z);
        }
        /// <summary>
        /// Получить точку с меньшим x (при равных x, сравнивать y, z)
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Point3D GetLeft(Point3D p1, Point3D p2)
        {
            if (p1.X < p2.X)
            {
                return p1;
            }
            else if (p1.X > p2.X)
            {
                return p2;
            }
            else
            {
                if (p1.Y < p2.Y)
                {
                    return p1;    
                }
                else if (p1.Y > p2.Y)
                {
                    return p2;
                }
                else
                {
                    if (p1.Z < p2.Z)
                    {
                        return p1;
                    }
                    else
                    {
                        return p2;
                    }
                }
            }                 
        }
        /// <summary>
        /// Получить точку с большим x (при равных x, сравнивать y, z)
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Point3D GetRight(Point3D p1, Point3D p2)
        {
            if (p1.X < p2.X)
            {
                return p2;
            }
            else if (p1.X > p2.X)
            {
                return p1;
            }
            else
            {
                if (p1.Y < p2.Y)
                {
                    return p2;
                }
                else if (p1.Y > p2.Y)
                {
                    return p1;
                }
                else
                {
                    if (p1.Z < p2.Z)
                    {
                        return p2;
                    }
                    else
                    {
                        return p1;
                    }
                }
            }     
        }
    }
}
