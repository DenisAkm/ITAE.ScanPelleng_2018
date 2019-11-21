using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Integral
{
    public class DVector
    {
        //Поля
        public double X;
        public double Y;
        public double Z;

        //Конструкторы
        public DVector()
        {
            X = 0;
            Y = 0;
            Z = 0;   
        }
        public DVector(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;        
        }
        public DVector(DVector vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }
        public DVector(Point3D p)
        {
            X = p.X;
            Y = p.Y;
            Z = p.Z; 
        }




        //Динамическийе методы
        public void Set(double fx, double fy, double fz)
        {
            X = fx;
            Y = fy;
            Z = fz;
        }
        public double Module
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y + Z * Z);
            }
        }
        public void Normalize()
        {
            double length = this.Module;
            X /= length;
            Y /= length;
            Z /= length;
        }





        //Статический методы
        public static DVector Cross(DVector v1, DVector v2)
        {
            double x = v1.Y * v2.Z - v1.Z * v2.Y;
            double y = v1.Z * v2.X - v1.X * v2.Z;
            double z = v1.X * v2.Y - v1.Y * v2.X;
            return new DVector(x, y, z);            
        }
        public static double Scal(DVector v1, DVector v2)
        {
            double ans = 0;
            if (DVector.IsEqual(v1,v2, 9))
            {
                ans = 1;
            }
            else
            {
                ans = v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
            }
            return ans;
        }        
        public static DVector operator +(DVector sum1, DVector sum2)
        {
            return new DVector(sum1.X + sum2.X, sum1.Y + sum2.Y, sum1.Z + sum2.Z);
        }
        public static DVector operator *(double a, DVector mult)
        {
            return new DVector(a * mult.X, a * mult.Y, a * mult.Z);
        }
        public static DVector operator *(DVector mult, double a)
        {
            return new DVector(a * mult.X, a * mult.Y, a * mult.Z);
        }
        public static CVector operator *(Complex a, DVector mult)
        {
            return new CVector(a * mult.X, a * mult.Y, a * mult.Z);
        }
        public static DVector operator -(DVector sum1, DVector sum2)
        {
            return new DVector(sum1.X - sum2.X, sum1.Y - sum2.Y, sum1.Z - sum2.Z);
        }
        public static DVector operator /(DVector mult, double a)
        {
            return new DVector(mult.X / a, mult.Y / a, mult.Z / a);
        }
        public static bool IsEqual(DVector v1, DVector v2, int precision)
        {
            bool ans = false;
            double param = Math.Pow(10, (-1) * precision);
            if (Math.Abs(v1.X - v2.X) < param && Math.Abs(v1.Y - v2.Y) < param && Math.Abs(v1.Z - v2.Z) < param)
            {
                ans = true;
            }

            return ans;
        }
        public static DVector DirectionUp(Point3D a, Point3D b)
        {
            DVector res = new DVector();
            
            if (a > b)
            {
                res = new DVector(a) - new DVector(b);
            }
            else
            {
                res = new DVector(b) - new DVector(a);
            }
            res.Normalize();
            return res;
        }
        
        /// <summary>
        /// Получить направление тока от -x до x между точками а и b
        /// </summary>
        /// <param name="a">Точка а</param>
        /// <param name="b">Точка b</param>
        /// <returns></returns>
        public static DVector GetDirection(Point3D a, Point3D b)
        {            

            DVector v = new DVector(b.X - a.X, b.Y - a.Y, b.Z - a.Z);
            v.Normalize();
            return v;
        }
        
    }
}
