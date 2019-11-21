using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Integral
{
    class ThinWireAprox
    {
        readonly Complex imOne = new Complex(0, 1);      // мнимая единица        
        readonly double K_0;
        readonly double K2;
        readonly double lambda;
        readonly double frequency;
        readonly double Omega;  
        const double pi = Math.PI;
        const double Mu_0 = Constant.Mu_0;                                    // 1.0 / (c * c * E_0) Гн/м     magnetic constant    магнитная постоянная
        const double E_0 = Constant.E_0;                                      // 8.85e-12 Ф/м        electric constant     электрическая постоянная
        readonly static double c = Constant.c;//1 / Math.Sqrt(E_0 * Mu_0);       // м/с скорость света 
        readonly double wire;
        readonly double wire2;
        MatrixAlg A;
        MatrixAlg E;                
        List<WireMesh> geo;
        MatrixAlg X;
        WireCurrent I;
        

        double theta;
        double phi;
        double polar;


        /// <summary>
        /// Расчет рассеянного поля на проволоке
        /// </summary>
        /// <param name="k">Волновой вектор</param>
        /// <param name="L">Сетка разбиения проволоки</param>

        /// <summary>
        /// Расчет рассеянного поля на проволоке
        /// </summary>
        /// <param name="_L">Сетка разбиения проволоки</param>
        /// <param name="freq">Частота</param>
        /// <param name="_phi">Координата phi падпющего поля</param>
        /// <param name="_theta">Координата theta падпющего поля</param>
        /// <param name="_e_phi">Составляющая поля вектор-phi</param>
        /// <param name="_e_theta">Составляющая поля вектор-theta</param>
        /// <param name="_wire">Радиус проволоки</param>
        public ThinWireAprox(List<WireMesh> g, double freq, double _phi, double _theta, double _polar, double _wire)
        {            
            polar = _polar;
            geo = g;
            phi = _phi;
            theta = _theta;
            frequency = freq;
            lambda = c / freq;
            Omega = 2 * pi * freq;
            K_0 = 2 * pi / lambda;      // волновое число 2pi/lambda
            K2 = K_0 * K_0;
            wire = _wire;
            wire2 = wire * wire;


            A = LeftPartCalculation2();                        
            E = RightPartCalculation();
            X = MatrixAlg.Gauss(A, E);
              
          
            I = ReturnCurrentsSegments();
         }
        public ThinWireAprox(List<WireMesh> g, NearField incField, double freq, double _wire)
        {            
            polar = 0;
            geo = g;
            phi = 0;
            theta = 0;
            frequency = freq;
            lambda = c / freq;
            Omega = 2 * pi * freq;
            K_0 = 2 * pi / lambda;      // волновое число 2pi/lambda
            K2 = K_0 * K_0;
            wire = _wire;
            wire2 = wire * wire;

            A = LeftPartCalculation2(4);
            E = RightPartCalculation(incField, 4);
            X = MatrixAlg.Gauss(A, E);


            I = ReturnCurrentsSegments();
        }
                
        private MatrixAlg RightPartCalculation()
        {
            int baseFunctionNumber = 0;
            for (int k = 0; k < geo.Count; k++)
            {
                baseFunctionNumber += geo[k].BaseFunctionsCount;    
            }

            List<WireBasisElement> baseFunctions = geo[0].BaseFunctions;
            for (int k = 1; k < geo.Count; k++)
            {
                for (int s = 0; s < geo[k].BaseFunctionsCount; s++)
                {
                    baseFunctions.Add(geo[k].BaseFunctions[s]);    
                }                
            }
            
            Complex iwE0 = imOne * Omega * E_0;
            MatrixAlg _E = new MatrixAlg(baseFunctionNumber, 1);

            double a = K_0 * Math.Sin(theta * pi / 180) * Math.Cos(phi * pi / 180);
            double b = K_0 * Math.Sin(theta * pi / 180) * Math.Sin(phi * pi / 180);
            double c = K_0 * Math.Cos(theta * pi / 180);

            DVector e_phi = new DVector(Math.Sin(phi * pi / 180), (-1) * Math.Cos(phi * pi / 180), 0);
            DVector e_theta = new DVector((-1) * Math.Cos(theta * pi / 180) * Math.Cos(phi * pi / 180), (-1) * Math.Cos(theta * pi / 180) * Math.Sin(phi * pi / 180), Math.Sin(theta * pi / 180));

            DVector e = (-1) * e_phi * Math.Sin(polar * pi / 180) + e_theta * Math.Cos(polar * pi / 180);
            for (int i = 0; i < baseFunctionNumber; i++)
            {
                WireBasisElement bacefunction = baseFunctions[i];                
                Point3D c1 = bacefunction.L_Right.Center;                
                Point3D c2 = bacefunction.L_Left.Center;

                double arg1 = a * c1.X + b * c1.Y + c * c1.Z;
                double arg2 = a * c2.X + b * c2.Y + c * c2.Z;
                Complex exp1 = new Complex(Math.Cos(arg1), Math.Sin(arg1));
                Complex exp2 = new Complex(Math.Cos(arg2), Math.Sin(arg2));

                double delta1 = bacefunction.L_Left.Length;
                double delta2 = bacefunction.L_Right.Length;
                Line l1 = bacefunction.L_Left;
                Line l2 = bacefunction.L_Right;
                Point3D P_center = bacefunction.P;
                Point3D P_left = bacefunction.P_Left;
                Point3D P_right = bacefunction.P_Right;
                
                DVector v_i1 = DVector.GetDirection(P_left, P_center);
                DVector v_i2 = DVector.GetDirection(P_center, P_right);                         
                
                double scal1 = DVector.Scal(v_i1, e);
                double scal2 = DVector.Scal(v_i2, e);
                _E.arrays[i][0] = iwE0 * (scal1 * exp1 * delta1 / 2f + scal2 * exp2 * delta2 / 2f);
            }
            
            return _E;
        }
        private MatrixAlg RightPartCalculation(NearField nearField, int p)
        {
            int baseFunctionNumber = 0;
            for (int k = 0; k < geo.Count; k++)
            {
                baseFunctionNumber += geo[k].BaseFunctionsCount;
            }

            List<WireBasisElement> baseFunctions = new List<WireBasisElement>();
            for (int k = 0; k < geo.Count; k++)
            {
                for (int s = 0; s < geo[k].BaseFunctionsCount; s++)
                {
                    baseFunctions.Add(geo[k].BaseFunctions[s]);
                }
            }

            Complex iwE0 = imOne * Omega * E_0;
            MatrixAlg _E = new MatrixAlg(baseFunctionNumber, 1);


            Parallel.For(0, baseFunctionNumber, i =>
            {
                WireBasisElement bacefunction = baseFunctions[i];

                double delta1 = bacefunction.L_Left.Length;
                double delta2 = bacefunction.L_Right.Length;

                Line l1 = bacefunction.L_Left;
                Line l2 = bacefunction.L_Right;

                Point3D P_center = bacefunction.P;
                Point3D P_left = bacefunction.P_Left;
                Point3D P_right = bacefunction.P_Right;

                DVector v_i1 = DVector.GetDirection(P_left, P_center);
                DVector v_i2 = DVector.GetDirection(P_center, P_right);
                CVector e1 = nearField.FindField(l1);
                CVector e2 = nearField.FindField(l2);
                Complex scal1 = CVector.Scal(e1, v_i1);
                Complex scal2 = CVector.Scal(e2, v_i2);
                _E.arrays[i][0] = iwE0 * (scal1 * delta1 / 2f + scal2 * delta2 / 2f);
            });

            return _E;
        }
        private MatrixAlg RightPartCalculation(NearField nearField)
        {
            int baseFunctionNumber = 0;
            for (int k = 0; k < geo.Count; k++)
            {
                baseFunctionNumber += geo[k].BaseFunctionsCount;
            }

            List<WireBasisElement> baseFunctions = new List<WireBasisElement>();
            for (int k = 0; k < geo.Count; k++)
            {
                for (int s = 0; s < geo[k].BaseFunctionsCount; s++)
                {
                    baseFunctions.Add(geo[k].BaseFunctions[s]);
                }
            }

            Complex iwE0 = imOne * Omega * E_0;
            MatrixAlg _E = new MatrixAlg(baseFunctionNumber, 1);

            
            for (int i = 0; i < baseFunctionNumber; i++)
            {
                WireBasisElement bacefunction = baseFunctions[i];                

                double delta1 = bacefunction.L_Left.Length;
                double delta2 = bacefunction.L_Right.Length;

                Line l1 = bacefunction.L_Left;
                Line l2 = bacefunction.L_Right;

                Point3D P_center = bacefunction.P;
                Point3D P_left = bacefunction.P_Left;
                Point3D P_right = bacefunction.P_Right;

                DVector v_i1 = DVector.GetDirection(P_left, P_center);
                DVector v_i2 = DVector.GetDirection(P_center, P_right);
                CVector e1 = nearField.FindField(l1);
                CVector e2 = nearField.FindField(l2);
                Complex scal1 = CVector.Scal(e1, v_i1);
                Complex scal2 = CVector.Scal(e2, v_i2);
                _E.arrays[i][0] = iwE0 * (scal1 * delta1 / 2f + scal2 * delta2 / 2f);
            }

            return _E;
        }
        private MatrixAlg LeftPartCalculation2()
        {
            Complex G_LL, G_LR, G_RL, G_RR;
            int baseFunctionNumber = 0;
            for (int k = 0; k < geo.Count; k++)
            {
                baseFunctionNumber += geo[k].BaseFunctionsCount;
            }

            List<WireBasisElement> baseFunctions = new List<WireBasisElement>();

            for (int k = 0; k < geo.Count; k++)
            {
                for (int s = 0; s < geo[k].BaseFunctionsCount; s++)
                {
                    baseFunctions.Add(geo[k].BaseFunctions[s]);
                }
            }

            Complex eps1 = Complex.ImaginaryOne / Omega / E_0;
            MatrixAlg _A = new MatrixAlg(baseFunctionNumber, baseFunctionNumber);

            for (int i = 0; i < baseFunctionNumber; i++)
            {
                WireBasisElement bacefunction1 = baseFunctions[i]; 
                Line l1 = bacefunction1.L_Right;
                Line l2 = bacefunction1.L_Left;

                //Точки обзора на середине двух соседних интервалов
                Point3D P_i1 = l1.Center;
                Point3D P_i2 = l2.Center;
                double delta_i1 = l1.Length;
                double delta_i2 = l2.Length;                

                Point3D P_center = bacefunction1.P;
                Point3D P_left = bacefunction1.P_Left;
                Point3D P_right = bacefunction1.P_Right;

                DVector v_i1 = DVector.GetDirection(P_center, P_left);
                DVector v_i2 = DVector.GetDirection(P_right, P_center);
                

                for (int j = 0; j < baseFunctionNumber; j++)
                {
                    Complex Coeff = new Complex();
                    //Точки интегрирования на краях и в центре базисной функции
                    WireBasisElement bacefunction2 = baseFunctions[j];
                    Line lj_1 = bacefunction2.L_Right;
                    Line lj_2 = bacefunction2.L_Left;

                    Point3D P_j1 = bacefunction2.P_Right;
                    Point3D P_j2 = bacefunction2.P;
                    Point3D P_j3 = bacefunction2.P_Left;

                    double delta_j1 = lj_1.Length;
                    double delta_j2 = lj_2.Length;

                    DVector v_j1 = DVector.GetDirection(P_j1, P_j2);
                    DVector v_j2 = DVector.GetDirection(P_j2, P_j3);

                    
                    G_LL = GreenFunctionIntegral(P_i1, P_j1, P_j2);
                    G_LR = GreenFunctionIntegral(P_i1, P_j2, P_j3);
                    G_RL = GreenFunctionIntegral(P_i2, P_j1, P_j2);
                    G_RR = GreenFunctionIntegral(P_i2, P_j2, P_j3);


                    double sc11 = DVector.Scal(v_i1, v_j1);
                    double sc12 = DVector.Scal(v_i1, v_j2);
                    double sc21 = DVector.Scal(v_i2, v_j1);
                    double sc22 = DVector.Scal(v_i2, v_j2);
                   

                    Complex C1 = K2 * ((sc11 * G_LL + sc12 * G_LR) * delta_i1 / 4 + (sc21 * G_RL + sc22 * G_RR) * delta_i2 / 4);
                    Complex C2 = ((G_LL - G_RL) / delta_j1 - (G_LR - G_RR) / delta_j2);                    
                    Coeff = (C1 - C2);
                    _A.arrays[i][j] = Coeff;
                }
            }

            return _A;
        }

        private MatrixAlg LeftPartCalculation2(int p)
        {
            Complex G_LL, G_LR, G_RL, G_RR;
            int baseFunctionNumber = 0;
            for (int k = 0; k < geo.Count; k++)
            {
                baseFunctionNumber += geo[k].BaseFunctionsCount;
            }

            List<WireBasisElement> baseFunctions = new List<WireBasisElement>();

            for (int k = 0; k < geo.Count; k++)
            {
                for (int s = 0; s < geo[k].BaseFunctionsCount; s++)
                {
                    baseFunctions.Add(geo[k].BaseFunctions[s]);
                }
            }

            Complex eps1 = Complex.ImaginaryOne / Omega / E_0;
            MatrixAlg _A = new MatrixAlg(baseFunctionNumber, baseFunctionNumber);

            Parallel.For(0, baseFunctionNumber, i =>
            {
                WireBasisElement bacefunction1 = baseFunctions[i];
                Line l1 = bacefunction1.L_Right;
                Line l2 = bacefunction1.L_Left;

                //Точки обзора на середине двух соседних интервалов
                Point3D P_i1 = l1.Center;
                Point3D P_i2 = l2.Center;
                double delta_i1 = l1.Length;
                double delta_i2 = l2.Length;

                Point3D P_center = bacefunction1.P;
                Point3D P_left = bacefunction1.P_Left;
                Point3D P_right = bacefunction1.P_Right;

                DVector v_i1 = DVector.GetDirection(P_center, P_left);
                DVector v_i2 = DVector.GetDirection(P_right, P_center);


                for (int j = 0; j < baseFunctionNumber; j++)
                {
                    Complex Coeff = new Complex();
                    //Точки интегрирования на краях и в центре базисной функции
                    WireBasisElement bacefunction2 = baseFunctions[j];
                    Line lj_1 = bacefunction2.L_Right;
                    Line lj_2 = bacefunction2.L_Left;

                    Point3D P_j1 = bacefunction2.P_Right;
                    Point3D P_j2 = bacefunction2.P;
                    Point3D P_j3 = bacefunction2.P_Left;

                    double delta_j1 = lj_1.Length;
                    double delta_j2 = lj_2.Length;

                    DVector v_j1 = DVector.GetDirection(P_j1, P_j2);
                    DVector v_j2 = DVector.GetDirection(P_j2, P_j3);


                    G_LL = GreenFunctionIntegral(P_i1, P_j1, P_j2);
                    G_LR = GreenFunctionIntegral(P_i1, P_j2, P_j3);
                    G_RL = GreenFunctionIntegral(P_i2, P_j1, P_j2);
                    G_RR = GreenFunctionIntegral(P_i2, P_j2, P_j3);


                    double sc11 = DVector.Scal(v_i1, v_j1);
                    double sc12 = DVector.Scal(v_i1, v_j2);
                    double sc21 = DVector.Scal(v_i2, v_j1);
                    double sc22 = DVector.Scal(v_i2, v_j2);


                    Complex C1 = K2 * ((sc11 * G_LL + sc12 * G_LR) * delta_i1 / 4 + (sc21 * G_RL + sc22 * G_RR) * delta_i2 / 4);
                    Complex C2 = ((G_LL - G_RL) / delta_j1 - (G_LR - G_RR) / delta_j2);
                    Coeff = (C1 - C2);
                    _A.arrays[i][j] = Coeff;
                }
            });

            return _A;
        }
                

        /// <summary>
        /// Интеграл функции Грина из точки наблюдения i по отрезку [j1,j2]
        /// </summary>
        /// <param name="p_i">Точка наблюдения</param>
        /// <param name="p_j1">Левая граница интегрирования</param>
        /// <param name="p_j2">Правая граница интегрирования</param>
        /// <returns></returns>
        private Complex GreenFunctionIntegral(Point3D p_i, Point3D p_j1, Point3D p_j2)
        {
            Complex funG = new Complex();

            double delta = Point3D.Distance(p_j1, p_j2);
            

            Point3D p_j = Point3D.GetMiddlePoint(p_j1, p_j2);
            double delta_j = Point3D.Distance(p_j1, p_j2);
            double d = Point3D.Distance(p_i, p_j);
            if (d < delta_j / 6)//
            {
                Complex integral = SimpsonInt_G2(p_i, p_i, p_j2);
                double d_2w = delta / (2 * wire);
                Complex A = 2 * integral;
                Complex B = 1 / (2 * pi) * Math.Log(d_2w + Math.Sqrt(1 + d_2w * d_2w));
                funG = A + B;                
            }
            else if (d < 2 * delta_j)
            {
                funG = SimpsonInt_G(p_i, p_j1, p_j2, 1);
            }
            else
            {
                funG = SimpsonInt_G(p_i, p_j1, p_j2, 1);                
            }
            return funG;
        }
        
        private Complex SimpsonInt_G(Point3D p_i, Point3D p_j1, Point3D p_j2, int N)
        {
            double delta_j = Point3D.Distance(p_j1, p_j2);
            Complex Ga = G_wire(p_i, p_j1);
            Complex Gb = G_wire(p_i, p_j2);
            Complex I = new Complex();
            


            Complex sum2 = new Complex();
            Complex sum4 = new Complex();
            Complex sum = new Complex();
            
            double h = delta_j / (2 * N);//Шаг интегрирования.
            DVector v = DVector.GetDirection(p_j1, p_j2);
            v = h * v;

            
            for (int i = 1; i <= 2 * N - 1; i++)
            {
                if (i % 2 == 0)
                {
                    sum2 += G_wire(p_i, p_j1 + i * v);//Значения с чётными индексами, которые нужно умножить на 2.
                }
                else
                {
                    sum4 += G_wire(p_i, p_j1 + i * v);//Значения с нечётными индексами, которые нужно умножить на 4.
                }
            }

            sum = Ga + 4 * sum4 + 2 * sum2 + Gb;//Добавлем значение f(b) так как ранее его исключили. 
            
            I = (h / 3) * sum;

            return I;
        }

        private Complex SimpsonInt_G2(Point3D p_i, Point3D p_j1, Point3D p_j2)
        {
            double delta_j = Point3D.Distance(p_j1, p_j2);
            Complex Ga = G_wire2(p_i, p_j1);
            Complex Gb = G_wire2(p_i, p_j2);

            Complex I = new Complex(); //I-предыдущее вычисленное значение интеграла, I1-новое, с большим N.
            
            Complex sum4 = new Complex();
            Complex sum = new Complex();
            
            double h = delta_j / 2;//Шаг интегрирования.
            DVector v = DVector.GetDirection(p_j1, p_j2);
            v = h * v;
           
            sum = Ga + 4 * sum4 + Gb;

            I = (h / 3) * sum;

            return I;
        }
        /// <summary>
        /// Функиция Грина от точек i и j с учетом толщицы проволоки
        /// </summary>
        /// <param name="i">Точка обзора</param>
        /// <param name="j">Точка наблюдения</param>
        /// <returns></returns>
        private Complex G_wire(Point3D i, Point3D j)
        {
            double r = Math.Sqrt(Math.Pow(Point3D.Distance(i, j), 2) + wire2);
            Complex exp = new Complex(Math.Cos(K_0 * r), -Math.Sin(K_0 * r));
            return (exp / (4 * Math.PI * r));
        }
        private Complex G_wire2(Point3D i, Point3D j)
        {
            double r = Math.Sqrt(Math.Pow(Point3D.Distance(i, j), 2) + wire2);
            return (new Complex(Math.Cos(K_0 * r) - 1f, -Math.Sin(K_0 * r)) / (4 * Math.PI * r));
        }       
        
        
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Свойства
        private WireCurrent ReturnCurrentsSegments()
        {
            int baseFunctionNumber = 0;
            for (int k = 0; k < geo.Count; k++)
            {
                baseFunctionNumber += geo[k].BaseFunctionsCount;
            }

            List<WireBasisElement> baseFunctions = geo[0].BaseFunctions;
            for (int k = 1; k < geo.Count; k++)
            {
                for (int s = 0; s < geo[k].BaseFunctionsCount; s++)
                {
                    baseFunctions.Add(geo[k].BaseFunctions[s]);
                }
            }

            int countLines = 0;
            for (int k = 0; k < geo.Count; k++)
            {
                countLines += geo[k].CountLines;
            }



            Complex[] currentLineValues = new Complex[countLines];
            Line[] segment = new Line[countLines];

            for (int i = 0; i < baseFunctionNumber; i++)
            {
                WireBasisElement element = baseFunctions[i];
                int l1 = element.L_Left.Index;
                int l2 = element.L_Right.Index;
                Complex value = new Complex(X.arrays[i][0].Real, X.arrays[i][0].Imaginary) / 2;
                currentLineValues[l1] += value;
                currentLineValues[l2] += value;
                segment[l1] = element.L_Left;
                segment[l2] = element.L_Right;
            }
            DVector[] currentVectors = new DVector[countLines];

            List<Line> lines = geo[0].Lines;
            for (int k = 1; k < geo.Count; k++)
            {
                for (int s = 0; s < geo[k].CountLines; s++)
                {
                    lines.Add(geo[k].Lines[s]);
                }             
            }

            for (int i = 0; i < countLines; i++)
            {
                Point3D a = Point3D.GetLeft(lines[i].V2, lines[i].V1);
                Point3D b = Point3D.GetRight(lines[i].V2, lines[i].V1);
                currentVectors[i] = DVector.GetDirection(a, b);
            }


            WireCurrent Cur = new WireCurrent(currentLineValues, currentVectors, segment);
            return Cur;
        }        

        private void WriteMatrixAlg(MatrixAlg A)
        {
            StreamWriter sw = new StreamWriter(Path.Combine(Environment.CurrentDirectory, "MatrixAlg.txt"));
            for (int i = 0; i < A.Column; i++)
            {
                for (int j = 0; j < A.Row; j++)
                {
                    sw.Write(A.arrays[i][j].Magnitude + "\t");
                }
                sw.Write(Environment.NewLine);
            }
            sw.Close();
        }

        public WireCurrent GetCurrent()
        {
            return I;
        }
    }
}

