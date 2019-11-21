using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integral;
using System.Windows.Forms;

namespace Integral
{
    public class TriangleMesh
    {
        readonly public List<Point3D> points;
        readonly public List<Index3> indexes;
                
        List<Triangle> triangles;
        List<TriangleBaseElement> baseElements = new List<TriangleBaseElement>();
        public TriangleMesh(List<double> x, List<double> y, List<double> z, List<int> i1, List<int> i2, List<int> i3)
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


            for (int i = 0; i < count; i++)
            {
                Triangle origin = triangles[i];
                for (int j = i; j < count; j++)
                {
                    if (i != j)
                    {
                        Triangle triangle = triangles[j];
                        if (Triangle.Adjucted(origin, triangle))
                        {
                            baseElements.Add(new TriangleBaseElement(origin, triangle));
                        }
                    }
                }
            }
            
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
    }
    public struct Index3
    {
        readonly public int i1;
        readonly public int i2;
        readonly public int i3;
        public Index3(int i1, int i2, int i3)
        {
            this.i1 = i1;
            this.i2 = i2;
            this.i3 = i3;
        }
    }

    public class TriangleBaseElement
    {
        Triangle tr1;
        Triangle tr2;
        Line line;
        public TriangleBaseElement(Triangle triangele1, Triangle triangele2)
        {
            tr1 = triangele1;
            tr2 = triangele2;

            Line l11 = new Line(tr1.V2, tr1.V3);
            Line l12 = new Line(tr1.V1, tr1.V3);
            Line l13 = new Line(tr1.V1, tr1.V2);

            Line l21 = new Line(tr2.V2, tr2.V3);
            Line l22 = new Line(tr2.V1, tr2.V3);
            Line l23 = new Line(tr2.V1, tr2.V2);
         
            
            if (l11 == l21)
            {
                line = l11;
            }
            else if (l11 == l22)
            {
                line = l11;
            }
            else if (l11 == l23)
            {
                line = l11;
            }
            else if(l12 == l21)
            {
                line = l12;
            }
            else if (l12 == l22)
            {
                line = l12;
            }
            else if (l12 == l23)
            {
                line = l12;
            }
            else if(l13 == l21)
            {
                line = l13;
            }
            else if (l13 == l22)
            {
                line = l13;
            }
            else if (l13 == l23)
            {
                line = l13;
            }
            else
            {
                MessageBox.Show("");
            }
        }
    }
}
