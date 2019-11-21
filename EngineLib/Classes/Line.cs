using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integral
{
    public class Line
    {
        public Point3D V1 { get; set; }
        public Point3D V2 { get; set; }
                
        public int Index { get; set; }
        public Line(Point3D p1, Point3D p2, int index = 0)
        {
            V1 = p1;
            V2 = p2;
            Index = index;            
        }

        public Point3D Center
        {
            get
            {
                return Point3D.GetMiddlePoint(V1, V2);
            }
        }

        public Point3D FindOther(int index)
        {
            if (V1.Index == index)
            {
                return V2;
            }
            else if( V2.Index == index)
            {
                return V1;
            }
            else
            {
                return new Point3D(0, 0, 0);
            }
        }
        public double Length
        {
            get 
            {
                return Point3D.Distance(V1, V2);
            }
        }
        public static bool operator ==(Line a, Line b)
        {
            int v1 = a.V1.Index;
            int v2 = a.V2.Index;
            int v3 = b.V1.Index;
            int v4 = b.V2.Index;

            if (v1 == v3 && v2 == v4)
            {
                return true;
            }
            else if (v1 == v4 && v2 == v3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool operator !=(Line a, Line b)
        {
            int v1 = a.V1.Index;
            int v2 = a.V2.Index;
            int v3 = b.V1.Index;
            int v4 = b.V2.Index;

            if (v1 == v3 && v2 == v4)
            {
                return false;
            }
            else if (v1 == v4 && v2 == v3)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool Adjusted(Line a, Line b)
        {
            int i1 = a.V1.Index;
            int i2 = a.V2.Index;
            int i3 = b.V1.Index;
            int i4 = b.V2.Index;
            if (i1 == i3 || i1 == i4 || i2 == i3 || i2 == i4)
            {
                return true;    
            }
            else
            {
                return false;
            }

            
        }

        public bool Include(Point3D p)
        {
            if (this.V1.Index == p.Index || this.V2.Index == p.Index)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
