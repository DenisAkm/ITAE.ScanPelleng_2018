using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integral
{
    public class WireMesh
    {
        List<Point3D> points;
        List<Index2> indeces;
        List<WireBasisElement> baseElements = new List<WireBasisElement>();
        List<Line> lines;
        public List<Junction> junctions = new List<Junction>();
        public string Name {get; set;}
        public WireMesh(List<double> x, List<double> y, List<double> z, List<int> i1, List<int> i2, string name)
        {
            Name = name;
            int pointsCount = x.Count;
            points = new List<Point3D>(pointsCount);
            for (int i = 0; i < pointsCount; i++)
            {
                points.Add(new Point3D(x[i], y[i], z[i]));
            }

            int linesCount = i1.Count;
            linesCount = i1.Count;
            indeces = new List<Index2>(linesCount);
            for (int i = 0; i < linesCount; i++)
            {
                indeces.Add(new Index2(i1[i], i2[i]));
            }

            lines = new List<Line>(linesCount);

            for (int i = 0; i < linesCount; i++)
            {
                Point3D p1 = points[i1[i] - 1];
                Point3D p2 = points[i2[i] - 1];

                lines.Add(new Line(p1, p2, i));
            }

            //TODO: Find Bace Elements
            int basisIndex = 0;
            int junctionIndex = 1;
            List<Line> list;
            for (int i = 0; i < pointsCount; i++)
            {
                list = new List<Line>();
                Point3D p = points[i];

                for (int j = 0; j < lines.Count; j++)
                {
                    Line l = lines[j];
                    if (l.Include(p))
                    {
                        list.Add(l);
                    }
                }

                if (list.Count > 2)
                {
                    for (int n = 0; n < list.Count - 1; n++)
                    {
                        baseElements.Add(new WireBasisElement(list[n], list[n + 1], basisIndex, junctionIndex));
                        basisIndex++;
                    }
                    Junction J = new Junction(p, list);
                    junctions.Add(J);
                    junctionIndex++;
                }
                else if (list.Count == 2)
                {
                    baseElements.Add(new WireBasisElement(list[0], list[1], basisIndex));
                    basisIndex++;
                }
            }            
        }
               
        

        
        public int BaseFunctionsCount
        {
            get 
            {
                return baseElements.Count;
            }
        }
        public int CountPoints
        {
            get 
            {
                return points.Count;
            }
        }
        public int CountLines
        {
            get 
            {
                return indeces.Count;
            }
        }
        
        public List<Point3D> Points
        {
            get
            {
                //int num = points.Count;
                //List<Point3D> res = new List<Point3D>(num);
                //for (int i = 0; i < num; i++)
                //{
                //    res.Add(points[i]);
                //}
                return points;
            }
        }

        public List<Line> Lines
        {
            get 
            {
                //int num = lines.Count;
                //List<Line> res = new List<Line>(num);
                //for (int i = 0; i < num; i++)
                //{
                //    res.Add(lines[i]);
                //}
                //return res;
                return lines;
            }
        }
        public List<Index2> Indeces
        {
            get 
            {
                //int num = indeces.Count;
                //List<Index2> res = new List<Index2>(num);
                //for (int i = 0; i < num; i++)
                //{
                //    res.Add(indeces[i]);
                //}
                return indeces;
            }
        }

        public List<WireBasisElement> BaseFunctions
        {
            get
            {
                //int num = baseElements.Count;
                //List<WireBasisElement> res = new List<WireBasisElement>(num);
                //for (int i = 0; i < num; i++)
                //{
                //    res.Add(baseElements[i]);
                //}
                return baseElements;
            }
        }

        
    }

    public class WireBasisElement
    {
        public Line L_Left { get; set; }
        public Line L_Right { get; set; }
        public Point3D P { get; set; }
        public Point3D P_Left { get; set; }
        public Point3D P_Right { get; set; }

        public int Index { get; set; }
        
        public int Junction { get; set; }

        public WireBasisElement(Line a, Line b, int i, int j = 0)
        {
            int i1 = a.V1.Index;
            int i2 = a.V2.Index;
            int i3 = b.V1.Index;
            int i4 = b.V2.Index;
            this.Index = i;
            this.Junction = j;

            if (i1 == i3 || i1 == i4 )
            {
                P = a.V1;
            }
            else if (i2 == i3 || i2 == i4)
            {
                P = a.V2;
            }

            Point3D p1 = a.FindOther(P.Index);
            Point3D p2 = b.FindOther(P.Index);
            
            P_Left = Point3D.GetLeft(p1, p2);
            P_Right = Point3D.GetRight(p1, p2);

            if (P_Left == p1)
            {
                L_Left = a;
                L_Right = b;
            }
            else if (P_Left == p2)
            {
                L_Left = b;
                L_Right = a;
            }              
        }
    }
    public struct Index2
    {
        public int I1;
        public int I2;

        public Index2(int i1, int i2)
        {
            I1 = i1;
            I2 = i2;
        }
    }

    public class Junction
    {
        public Point3D P;
        public List<Line> Arms;
        public Junction(Point3D p, List<Line> arms)
        {
            P = p;
            Arms = arms;
        }
        public int Count
        {
            get
            {
                return Arms.Count;
            }
        }
    }
}
