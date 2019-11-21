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
        static Complex[,] A;    
        static Complex[] E;                
        List<WireMesh> geo;
        static Complex[] X;
        WireCurrent I;
        TextBox info;
        double C_4pi = 4 * Math.PI;
        public static int DegreeOfParallelism { get; set; }

        double theta;
        double phi;
        double polar;
        int counter = 0;

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
        public ThinWireAprox(List<WireMesh> g, NearField incField, double freq, double _wire, TextBox tx)
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
            info = tx;
                        
            A = LeftPartCalculation2(DegreeOfParallelism);  
            info.AppendText(counter + Environment.NewLine);
            E = RightPartCalculation(A, incField, DegreeOfParallelism);

            info.AppendText("Решение матрицы " + Environment.NewLine);
            X = MatrixSolution.Solve(A);
            
            //тест
            A = LeftPartCalculation2(DegreeOfParallelism);//   
            Complex[] b = MultiplyMatrix(A, X);

            //E = RightPartCalculation(incField, DegreeOfParallelism);
            //Complex[] X_test = MatrixSolution.Solve(A, E);

            double accr = Compare(X, b);
            info.AppendText("Отклонение равно" + accr + Environment.NewLine);
            //X = MatrixAlg.Gauss(A, E);
            //X = MatrixAlg.GaussOriginal(A, E);                     
            I = ReturnCurrentsSegments();            
        }
        void InsertRightPart(Complex[,] A, Complex[] b)
        {
            int size = A.GetLength(0);
            for (int i = 0; i < size; i++)
            {
                A[i, size] = b[i];
            }
        }
        Complex[] MultiplyMatrix(Complex[,] A, Complex[] X)
        {
            int rows_number = X.Length;
            Complex[] b = new Complex[rows_number];
            
            for (int i = 0; i < rows_number; i++)
            {
                for (int j = 0; j < rows_number; j++)
                {
                    b[i] += A[i, j] * b[j];    
                }                
            }
            return b;
        }
        private double Compare(Complex[] X, Complex[] X_test)
        {
            double result = 0;
            for (int i = 0; i < X.Length; i++)
            {
                Complex value = X[i] - X_test[i];
                result += value.Magnitude;
            }
            return result;
        }

        private void KirchhoffLowProof()
        {            
            int count = geo[0].junctions.Count;
            for (int i = 1; i <= count; i++)
            {
                Complex val = new Complex(0, 0);

                for (int k = 0; k < geo[0].BaseFunctionsCount; k++)
                {
                    var bf = geo[0].BaseFunctions[k];

                    if (bf.Junction == i)
                    {
                        val += X[k];
                    }                              
                }
                info.AppendText(val.Real + "\n " + val.Magnitude + Environment.NewLine);                
            }
        }               
        
        private Complex[] RightPartCalculation(Complex[,] matrix, NearField nearField, int p)
        {
            info.AppendText("Составление элементов правой части матрицы " + Environment.NewLine);

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
            Complex[] vectorE = new Complex[baseFunctionNumber];

            ParallelOptions opt = new ParallelOptions();
            opt.MaxDegreeOfParallelism = p;
            Parallel.For(0, baseFunctionNumber, opt, i =>
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
                vectorE[i] = iwE0 * (scal1 * delta1 / 2f + scal2 * delta2 / 2f);
            });

            InsertRightPart(matrix, vectorE);
            return vectorE;
        }
         
        private Complex[] RightPartCalculation(NearField nearField)
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
            Complex[] _E = new Complex[baseFunctionNumber];


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
                _E[i] = iwE0 * (scal1 * delta1 / 2f + scal2 * delta2 / 2f);
            }

            return _E;
        }

        private Complex[,] LeftPartCalculation2(int p)
        {
            info.AppendText("Составление элементов левой части матрицы " + Environment.NewLine);

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
            Complex[,] _A = new Complex[baseFunctionNumber, baseFunctionNumber + 1];
            var options = new ParallelOptions() { MaxDegreeOfParallelism = 4 };
            Parallel.For(0, baseFunctionNumber, options, i =>
            {
                WireBasisElement bacefunction1 = baseFunctions[i];
                Line l1 = bacefunction1.L_Left;
                Line l2 = bacefunction1.L_Right;

                //Точки обзора на середине двух соседних интервалов
                Point3D P_i1 = l1.Center;
                Point3D P_i2 = l2.Center;
                double delta_i1 = l1.Length;
                double delta_i2 = l2.Length;

                Point3D P_center = bacefunction1.P;
                Point3D P_left = bacefunction1.P_Left;
                Point3D P_right = bacefunction1.P_Right;

                DVector v_i1 = DVector.GetDirection(P_left, P_center);
                DVector v_i2 = DVector.GetDirection(P_center, P_right);


                for (int j = 0; j < baseFunctionNumber; j++)
                {
                    Complex Coeff = new Complex();
                    //Точки интегрирования на краях и в центре базисной функции
                    WireBasisElement bacefunction2 = baseFunctions[j];
                    Line lj_1 = bacefunction2.L_Left;
                    Line lj_2 = bacefunction2.L_Right;

                    Point3D P_j1 = bacefunction2.P_Left;
                    Point3D P_j2 = bacefunction2.P;
                    Point3D P_j3 = bacefunction2.P_Right;

                    double delta_j1 = lj_1.Length;
                    double delta_j2 = lj_2.Length;

                    DVector v_j1 = DVector.GetDirection(P_j1, P_j2);
                    DVector v_j2 = DVector.GetDirection(P_j2, P_j3);


                    Complex G_LL = GreenFunctionIntegral(P_i1, P_j1, P_j2);
                    Complex G_LR = GreenFunctionIntegral(P_i1, P_j2, P_j3);
                    Complex G_RL = GreenFunctionIntegral(P_i2, P_j1, P_j2);
                    Complex G_RR = GreenFunctionIntegral(P_i2, P_j2, P_j3);


                    double sc11 = DVector.Scal(v_i1, v_j1);
                    double sc12 = DVector.Scal(v_i1, v_j2);
                    double sc21 = DVector.Scal(v_i2, v_j1);
                    double sc22 = DVector.Scal(v_i2, v_j2);


                    Complex C1 = K2 * ((sc11 * G_LL + sc12 * G_LR) * delta_i1 / 4 + (sc21 * G_RL + sc22 * G_RR) * delta_i2 / 4);
                    Complex C2 = ((G_LL - G_RL) / delta_j1 - (G_LR - G_RR) / delta_j2);
                    Coeff = (C1 - C2);
                    _A[i, j] = Coeff;
                }
            });

            return _A;
        }
        private Complex[,] LeftPartCalculation2()
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
            Complex[,] _A = new Complex[baseFunctionNumber, baseFunctionNumber];

            for (int i = 0; i < baseFunctionNumber; i++)
            {
                WireBasisElement basefunction1 = baseFunctions[i];
                Line l1 = basefunction1.L_Left;
                Line l2 = basefunction1.L_Right;

                //Точки обзора на середине двух соседних интервалов
                Point3D P_i1 = l1.Center;
                Point3D P_i2 = l2.Center;
                double delta_i1 = l1.Length;
                double delta_i2 = l2.Length;

                Point3D P_center = basefunction1.P;
                Point3D P_left = basefunction1.P_Left;
                Point3D P_right = basefunction1.P_Right;

                DVector v_i1 = DVector.GetDirection(P_left, P_center);
                DVector v_i2 = DVector.GetDirection(P_center, P_right);


                for (int j = 0; j < baseFunctionNumber; j++)
                {
                    Complex Coeff = new Complex();
                    //Точки интегрирования на краях и в центре базисной функции
                    WireBasisElement bacefunction2 = baseFunctions[j];
                    Line lj_1 = bacefunction2.L_Left;
                    Line lj_2 = bacefunction2.L_Right;

                    Point3D P_j1 = bacefunction2.P_Left;
                    Point3D P_j2 = bacefunction2.P;
                    Point3D P_j3 = bacefunction2.P_Right;

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
                    _A[i, j] = Coeff;
                }
            }

            return _A;
        }
        /// <summary>
        /// Интеграл функции Грина из точки наблюдения i по отрезку [j1,j2]
        /// </summary>
        /// <param name="p_i">Точка наблюдения</param>
        /// <param name="p_j_left">Левая граница интегрирования</param>
        /// <param name="p_j_right">Правая граница интегрирования</param>
        /// <returns></returns>
        private Complex GreenFunctionIntegral(Point3D p_i, Point3D p_j_left, Point3D p_j_right)
        {
            Complex funG = new Complex();

            //double delta = Point3D.Distance(p_j1, p_j2);            
            
            Point3D p_j_center = Point3D.GetMiddlePoint(p_j_left, p_j_right);
            double delta_j = Point3D.Distance(p_j_left, p_j_right);
            double d = Point3D.Distance(p_i, p_j_center);
            if (d < delta_j / 6)// // 
            {
                Complex integral = SimpsonInt_G2(p_i, p_i, p_j_right, 1);
                double d_2w = delta_j / (2 * wire);
                Complex A = 2 * integral;
                Complex B = 1 / (2 * pi) * Math.Log(d_2w + Math.Sqrt(1 + d_2w * d_2w));
                funG = A + B;
                counter++;
            }
            else
            {
                funG = SimpsonInt_G(p_i, p_j_left, p_j_right, 1);
            }
            //else
            //{
            //    funG = SimpsonInt_G(p_i, p_j_left, p_j_right);                
            //}
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
        private Complex SimpsonInt_G2(Point3D p_i, Point3D p_j1, Point3D p_j2, int N)
        {
            double delta_j = Point3D.Distance(p_j1, p_j2);
            Complex Ga = G_wire2(p_i, p_j1);
            Complex Gb = G_wire2(p_i, p_j2);
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
                    sum2 += G_wire2(p_i, p_j1 + i * v);//Значения с чётными индексами, которые нужно умножить на 2.
                }
                else
                {
                    sum4 += G_wire2(p_i, p_j1 + i * v);//Значения с нечётными индексами, которые нужно умножить на 4.
                }
            }

            sum = Ga + 4 * sum4 + 2 * sum2 + Gb;//Добавлем значение f(b) так как ранее его исключили. 

            I = (h / 3) * sum;

            return I;
        }


        void WriteMatrixA(Complex[,] Amatrix)
        {
            string name = @"C:\Users\Denis\Documents\Documents 2018\Расчет рассеянного поля по ИУ\Apperture\matrixA.txt";

            StreamWriter sw = new StreamWriter(name);
            int size = Convert.ToInt32(Math.Sqrt(Amatrix.Length));
            for (int i = 0; i < size; i++)
			{
                for (int j = 0; j < size; j++)
                {
                    sw.Write(Amatrix[i, j].Real.ToString().Replace(",", ".") + "," + Amatrix[i, j].Imaginary.ToString().Replace(",", ".") + "\t");    
                }
                sw.Write(Environment.NewLine);
			}
            sw.Close();
        }
        void WriteMatrixB(Complex[] Bmatrix)
        {
            string name = @"C:\Users\Denis\Documents\Documents 2018\Расчет рассеянного поля по ИУ\Apperture\matrixB.txt";

            StreamWriter sw = new StreamWriter(name);
            int size = Bmatrix.Length;
            for (int i = 0; i < size; i++)
            {
                sw.WriteLine(Bmatrix[i].Real.ToString().Replace(",", ".") + "," + Bmatrix[i].Imaginary.ToString().Replace(",", "."));                                
            }
            sw.Close();
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
            return (exp / (C_4pi * r));
        }
        private Complex G_wire2(Point3D i, Point3D j)
        {
            double r = Math.Sqrt(Math.Pow(Point3D.Distance(i, j), 2) + wire2);
            return (new Complex(Math.Cos(K_0 * r) - 1f, -Math.Sin(K_0 * r)) / (C_4pi * r));
        }       
        
        
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Свойства
        private WireCurrent ReturnCurrentsSegments()
        {
            info.AppendText("Нахождение проволочных токов " + Environment.NewLine);

            int countLines = 0;
            for (int k = 0; k < geo.Count; k++)
            {
                countLines += geo[k].CountLines;
            }           

            Complex[] currentLineValues = new Complex[countLines];
            Line[] segment = new Line[countLines];

            for (int k = 0; k < geo.Count; k++)
            {
                for (int i = 0; i < geo[k].BaseFunctionsCount; i++)
                {
                    WireBasisElement element = geo[k].BaseFunctions[i];
                    int l1 = element.L_Left.Index;
                    int l2 = element.L_Right.Index;
                    Complex value = new Complex(X[i].Real, X[i].Imaginary) / 2;
                    currentLineValues[l1] += value;
                    currentLineValues[l2] += value;
                    
                    int shift = 0;
                    for (int s = 0; s < k; s++)
                    {
                        shift += geo[s].CountLines;
                    }
                    segment[shift + l1] = element.L_Left;
                    segment[shift + l2] = element.L_Right;
                }
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

        public WireCurrent GetCurrent()
        {
            return I;
        }
    }
}



//public ThinWireAprox(List<WireMesh> g, double freq, double _phi, double _theta, double _polar, double _wire)
//        {            
//            polar = _polar;
//            geo = g;
//            phi = _phi;
//            theta = _theta;
//            frequency = freq;
//            lambda = c / freq;
//            Omega = 2 * pi * freq;
//            K_0 = 2 * pi / lambda;      // волновое число 2pi/lambda
//            K2 = K_0 * K_0;
//            wire = _wire;
//            wire2 = wire * wire;


//            A = LeftPartCalculation2();                        
//            E = RightPartCalculation();
//            X = MatrixAlg.Gauss(A, E);
              
          
//            I = ReturnCurrentsSegments();
//         }



//private MatrixAlg RightPartCalculation()
//        {
//            int baseFunctionNumber = 0;
//            for (int k = 0; k < geo.Count; k++)
//            {
//                baseFunctionNumber += geo[k].BaseFunctionsCount;    
//            }

//            List<WireBasisElement> baseFunctions = geo[0].BaseFunctions;
//            for (int k = 1; k < geo.Count; k++)
//            {
//                for (int s = 0; s < geo[k].BaseFunctionsCount; s++)
//                {
//                    baseFunctions.Add(geo[k].BaseFunctions[s]);    
//                }                
//            }
            
//            Complex iwE0 = imOne * Omega * E_0;
//            MatrixAlg _E = new MatrixAlg(baseFunctionNumber, 1);

//            double a = K_0 * Math.Sin(theta * pi / 180) * Math.Cos(phi * pi / 180);
//            double b = K_0 * Math.Sin(theta * pi / 180) * Math.Sin(phi * pi / 180);
//            double c = K_0 * Math.Cos(theta * pi / 180);

//            DVector e_phi = new DVector(Math.Sin(phi * pi / 180), (-1) * Math.Cos(phi * pi / 180), 0);
//            DVector e_theta = new DVector((-1) * Math.Cos(theta * pi / 180) * Math.Cos(phi * pi / 180), (-1) * Math.Cos(theta * pi / 180) * Math.Sin(phi * pi / 180), Math.Sin(theta * pi / 180));

//            DVector e = (-1) * e_phi * Math.Sin(polar * pi / 180) + e_theta * Math.Cos(polar * pi / 180);
//            for (int i = 0; i < baseFunctionNumber; i++)
//            {
//                WireBasisElement bacefunction = baseFunctions[i];                
//                Point3D c1 = bacefunction.L_Right.Center;                
//                Point3D c2 = bacefunction.L_Left.Center;

//                double arg1 = a * c1.X + b * c1.Y + c * c1.Z;
//                double arg2 = a * c2.X + b * c2.Y + c * c2.Z;
//                Complex exp1 = new Complex(Math.Cos(arg1), Math.Sin(arg1));
//                Complex exp2 = new Complex(Math.Cos(arg2), Math.Sin(arg2));

//                double delta1 = bacefunction.L_Left.Length;
//                double delta2 = bacefunction.L_Right.Length;
//                Line l1 = bacefunction.L_Left;
//                Line l2 = bacefunction.L_Right;
//                Point3D P_center = bacefunction.P;
//                Point3D P_left = bacefunction.P_Left;
//                Point3D P_right = bacefunction.P_Right;
                
//                DVector v_i1 = DVector.GetDirection(P_left, P_center);
//                DVector v_i2 = DVector.GetDirection(P_center, P_right);                         
                
//                double scal1 = DVector.Scal(v_i1, e);
//                double scal2 = DVector.Scal(v_i2, e);
//                _E.arrays[i][0] = iwE0 * (scal1 * exp1 * delta1 / 2f + scal2 * exp2 * delta2 / 2f);
//            }
            
//            return _E;
//        }