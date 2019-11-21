using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Integral
{
    /// <summary>
    /// Распределение электрических и магнитных токов
    /// </summary>
    public class SurfaceCurrent
    {
        //Константы класса
        const double Mu_0 = Constant.Mu_0;       // 1.0 / (c * c * E_0) Гн/м     magnetic constant    магнитная постоянная
        const double E_0 = Constant.E_0;            // 8.85e-12 Ф/м        electric constant     электрическая постоянная
        public double Z_0 = Constant.Z_0;                    // sqrt(Mu_0/E_0) = 120pi Ом    impedance of free space   волновое сопротивление свободного пространства


        private SurfaceCurrentElement[] currentElement;

        public SurfaceCurrentElement this[int index]
        {
            get 
            {
                return currentElement[index];
            }
            set
            {
                currentElement[index] = value;
            }
        }
        //Конструкторы
        /// <summary>
        /// Создаёт экземпляр класса Current
        /// </summary>
        public SurfaceCurrent(Complex[] Current, DVector[] Vector, Triangle[] Segment)
        {
            currentElement = new SurfaceCurrentElement[Current.Length];
            
            for (int i = 0; i < Current.Length; i++)
            {
                currentElement[i] = new SurfaceCurrentElement(Current[i], Vector[i], Segment[i]);
            }
        }
        

        public int Count {
            get
            {
                return currentElement.Length;
            }
        }
    }
    public class SurfaceCurrentElement
    {

        public Complex X { get; set; }
        public Complex Y { get; set; }
        public Complex Z { get; set; }

        public double Real { get; set; }
        public double Imaginary { get; set; }
        public double Phase {
            get
            {
                return Math.Atan2(Imaginary, Real) * 180 / Math.PI;
            }            
        }
        public double Magnitude {
            get
            {
                return Math.Sqrt(Real * Real + Imaginary * Imaginary);
            }
        }
        public Triangle Segment { get; set; }

        public Point3D Position
        {
            get
            {
                return Segment.Center;
            }
        }
        public CVector CVector
        {
            get
            {
                return new CVector(X, Y, Z);
            }
        }
        public DVector DVector
        {
            get
            {
                DVector vector = new DVector(X.Magnitude, Y.Magnitude, Z.Magnitude);
                vector.Normalize();
                return vector;
            }
        }

        public Complex Value
        {
            get 
            {
                return new Complex(Real, Imaginary);
            }
            set 
            {
                this.Real = value.Real;
                this.Imaginary = value.Imaginary;
                DVector dir = new DVector(X.Magnitude, Y.Magnitude, Z.Magnitude);
                dir.Normalize();
                CVector v = value * dir;
                this.X = v.X;
                this.Y = v.Y;
                this.Z = v.Z;
            }
        }
        //public void Negative()
        //{
        //    this.Real *= -1;
        //    this.Imaginary *= -1;

        //    this.X *= -1;
        //    this.Y *= -1;
        //    this.Z *= -1;
        //}
        
        public SurfaceCurrentElement(Complex value, DVector vector, Triangle tr)
        {
            this.Real = value.Real;
            this.Imaginary = value.Imaginary;

            CVector i = value * vector;
            this.X = i.X;
            this.Y = i.Y;
            this.Z = i.Z;

            Segment = tr;
        }
    }
}
