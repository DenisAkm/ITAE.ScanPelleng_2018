using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Integral.Class
{
    unsafe public class MatrixAlg
    {
        public Complex[][] arrays;
        int row, column;
        [DllImport(@"C:\Users\Denis\YandexDisk\Work\ScanPelleng\EngineLib\bin\Debug\gauss_p.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern double* gauss_p(ref double[,] matrix, double[] b, int n, int proc);

        public int RowCount { get { return row; } }
        public int ColumnCount { get { return column; } }

        public MatrixAlg(int row, int colunm)
        {
            this.row = row;
            this.column = colunm;
            arrays = new Complex[row][];
            for (int i = 0; i < arrays.Length; i++)
            {
                arrays[i] = new Complex[colunm];
            }
        }
        public MatrixAlg Transpose()
        {
            MatrixAlg m = new MatrixAlg(column, row);

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    m.arrays[j][i] = arrays[i][j];
                }
            }

            return m;
        }
        public Complex ElementAt(int i, int j)
        {            
            return arrays[i][j];
        }
        
        
        public void TransposeMyself()
        {
            arrays = Transpose().arrays;
        }

        public MatrixAlg Inverse()
        {
            Complex det = Determinant();
            if (det == 0)
            {
                throw new Exception("Матрица вырождена");
            }

            MatrixAlg m = new MatrixAlg(row, column);

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    m.arrays[i][j] = Cofactor(arrays, i, j) / det;
                }
            }

            return m.Transpose();
        }
        public Complex Determinant()
        {
            if (column != row)
            {
                throw new Exception("Расчет определителя невозможен");
            }
            return Determinant(arrays);
        }
        private Complex Determinant(Complex[][] array)
        {
            int n = (int)Math.Sqrt(array.Length);

            if (n == 1)
            {
                return array[0][0];
            }

            Complex det = 0;

            for (int k = 0; k < n; k++)
            {
                det += array[0][k] * Cofactor(array, 0, k);
            }

            return det;
        }
        private Complex Cofactor(Complex[][] array, int row, int column)
        {
            return Convert.ToSingle(Math.Pow(-1, column + row)) * Determinant(Minor(array, row, column));
        }
        private Complex[][] Minor(Complex[][] array, int row, int column)
        {
            int n = (int)Math.Sqrt(array.Length);
            Complex[][] minor = new Complex[n - 1][];

            for (int i = 0; i < minor.Length; i++)
            {
                minor[i] = new Complex[n - 1];
            }

            int _i = 0;
            for (int i = 0; i < n; i++)
            {
                if (i == row)
                {
                    continue;
                }
                int _j = 0;
                for (int j = 0; j < n; j++)
                {
                    if (j == column)
                    {
                        continue;
                    }
                    minor[_i][_j] = array[i][j];
                    _j++;
                }
                _i++;
            }
            return minor;
        }
        public static MatrixAlg operator +(MatrixAlg m1, MatrixAlg m2)
        {
            if (m1.row != m2.row || m1.column != m2.column)
            {
                throw new Exception("Сложение невозможно");
            }

            MatrixAlg m = new MatrixAlg(m1.row, m1.column);

            for (int i = 0; i < m1.row; i++)
            {
                for (int j = 0; j < m1.column; j++)
                {
                    m.arrays[i][j] = m1.arrays[i][j] + m2.arrays[i][j];
                }
            }

            return m;
        }

        public static MatrixAlg operator -(MatrixAlg m1, MatrixAlg m2)
        {
            if (m1.row != m2.row || m1.column != m2.column)
            {
                throw new Exception("Вычитание невозможно");
            }

            MatrixAlg m = new MatrixAlg(m1.row, m1.column);

            for (int i = 0; i < m1.row; i++)
            {
                for (int j = 0; j < m1.column; j++)
                {
                    m.arrays[i][j] = m1.arrays[i][j] - m2.arrays[i][j];
                }
            }

            return m;
        }

        public static MatrixAlg operator *(MatrixAlg m1, MatrixAlg m2)
        {
            if (m1.column != m2.row)
            {
                throw new Exception("Умножение невозможно");
            }

            MatrixAlg m = new MatrixAlg(m1.row, m2.column);

            for (int i = 0; i < m1.row; i++)
            {
                for (int j = 0; j < m2.column; j++)
                {
                    Complex sum = new Complex();

                    for (int k = 0; k < m1.column; k++)
                    {
                        sum += m1.arrays[i][k] * m2.arrays[k][j];
                    }

                    m.arrays[i][j] = sum;
                }
            }

            return m;
        }
        public static MatrixAlg TextMatrix(int n)
        {
            MatrixAlg A = new MatrixAlg(n, n);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j + 1)
                    {
                        A.arrays[i][j] = new Complex(10 * i, 0);
                    }
                    else
                    {
                        A.arrays[i][j] = new Complex((10 * i + j), 0);
                    }                    
                }
            }
            return A;
        }
        public static MatrixAlg TextAttachedMatrix(int n)
        {
            MatrixAlg B = new MatrixAlg(n,1);
            for (int i = 0; i < n; i++)
            {
                B.arrays[i][0] = new Complex(1, 1);
            }
            return B;
        }
        private static object threadLock = new object();
        public static MatrixAlg Gauss(MatrixAlg A, MatrixAlg B)
        {            
            //1 - 
            var options = new ParallelOptions() { MaxDegreeOfParallelism = ThinWireAprox.DegreeOfParallelism };
            int n = B.arrays.GetLength(0);            
            MatrixAlg X;
            double max;         //квадрат элемента    
            int k = 0, index;

            const double eps = 0.000001;  // точность
            X = new MatrixAlg(n, 1);            
                        
            while (k < n)
            {                
                // Поиск строки с максимальным a[i][k]
                //max = A.arrays[k][k].Real * A.arrays[k][k].Real + A.arrays[k][k].Imaginary * A.arrays[k][k].Imaginary;
                max = Complex.Abs(A.arrays[k][k]);
                index = k;
                for (int i = k + 1; i < n; i++)
                {
                    //double val = A.arrays[i][k].Real * A.arrays[i][k].Real + A.arrays[i][k].Imaginary * A.arrays[i][k].Imaginary;
                    double val = Complex.Abs(A.arrays[i][k]);
                    if (val > max)
                    {
                        max = val;
                        index = i;
                    }
                }
                // Перестановка строк
                if (max < eps)
                {                    
                    MessageBox.Show("Нулевые диагональные элементы");                                       
                    break;             
                }

                for (int j = 0; j < n; j++)
                {
                    Complex temp = A.arrays[k][j];
                    A.arrays[k][j] = A.arrays[index][j];
                    A.arrays[index][j] = temp;
                }
                Complex tempVal = B.arrays[k][0];
                B.arrays[k][0] = B.arrays[index][0];
                B.arrays[index][0] = tempVal;

                Complex firstVal = A.arrays[k][k];
                if (!(Complex.Abs(firstVal) < eps))// для нулевого коэффициента пропустить
                {
                    for (int j = 0; j < n; j++)
                    {
                        A.arrays[k][j] = A.arrays[k][j] / firstVal;
                    }
                    B.arrays[k][0] = B.arrays[k][0] / firstVal;
                }
                // Нормализация уравнений
                Parallel.For(k + 1, n, options, i =>
                {
                    SubProc(A, B, i, k, n);
                });
                
                k++;
            }
            
            // обратная подстановка
            for (k = n - 1; k >= 0; k--)
            {
                X.arrays[k][0] = B.arrays[k][0];
                for (int i = 0; i < k; i++)
                {
                    B.arrays[i][0] = B.arrays[i][0] - A.arrays[i][k] * X.arrays[k][0];
                }
            }
            WriteDiagonalElements(A, @"C:\Users\Denis\Documents\Documents 2018\Расчет рассеянного поля по ИУ\Apperture\report.txt");
            return X;
        }

        //public static MatrixAlg GaussOptimized(MatrixAlg A, MatrixAlg B)
        //{
        //    double[,] matrix = MatrixAlg.ConvertToDouble2(A);
        //    double[] b = MatrixAlg.ConvertToDouble1(B);
        //    double* answer;
        //    answer = gauss_p(ref matrix, b, A.ColumnCount, 4);

        //    string text = "";
        //    for (int i = 0; i < answer.Length; i++)
        //    {
        //        text += answer[i] + " ";
        //    }
        //    MessageBox.Show(text);
        //    return MatrixAlg.ConvertToThis(answer);
        //}
        public static MatrixAlg ConvertToThis(double[] matrix)
        {
            return new MatrixAlg(1,2);
        }
        public static double[,] ConvertToDouble2(MatrixAlg A)
        {
            double[,] myArr = new double[3, 3];
            Random ran = new Random();

            // Инициализируем данный массив
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    myArr[i, j] = ran.Next(1, 15);
                    Console.Write("{0}\t", myArr[i, j]);
                }
                Console.WriteLine();
            }
            return myArr;
        }
        public static double[] ConvertToDouble1(MatrixAlg A)
        {
            return new double[] { 1, 1, 0 };
        }
        public static void SubProc(MatrixAlg A, MatrixAlg B, int i, int k, int n)
        {
            const double eps = 0.000001;  // точность
            Complex temp = A.arrays[i][k];
            if (!(Complex.Abs(temp) < eps))// для нулевого коэффициента пропустить
            {
                // Нормализация уравнений
                for (int j = 0; j < n; j++)
                {
                    A.arrays[i][j] = A.arrays[i][j] / temp;
                }
                B.arrays[i][0] = B.arrays[i][0] / temp;

                for (int j = 0; j < n; j++)
                {
                    A.arrays[i][j] = A.arrays[i][j] - A.arrays[k][j];
                }
                B.arrays[i][0] = B.arrays[i][0] - B.arrays[k][0];
            }
        }
        
        public static MatrixAlg GaussOriginal(MatrixAlg A, MatrixAlg B)
        {
            //1 - Инициализация
            int n = B.arrays.GetLength(0);
            MatrixAlg X;
            double max;

            int k = 0, index;
            const double eps = 0.000001;  // точность

            X = new MatrixAlg(n, 1);            

            while (k < n)
            {
                // 2 - Поиск строки с наибольшим элементом в ведущем столбце
                max = Complex.Abs(A.arrays[k][k]);
                index = k;
                for (int i = k + 1; i < n; i++)
                {
                    if (Complex.Abs(A.arrays[i][k]) > max)
                    {
                        max = Complex.Abs(A.arrays[i][k]);
                        index = i;
                    }
                }


                // 3 - Перестановка строки с максимальным значением диагонального элемента
                if (max < eps)
                {
                    MessageBox.Show("нет ненулевых диагональных элементов");
                    break;
                }
                for (int j = 0; j < n; j++)
                {
                    Complex temp = A.arrays[k][j];
                    A.arrays[k][j] = A.arrays[index][j];
                    A.arrays[index][j] = temp;
                }
                Complex tempVal = B.arrays[k][0];
                B.arrays[k][0] = B.arrays[index][0];
                B.arrays[index][0] = tempVal;


                // 4 - Нормализация уравнений и вычитание
                for (int i = k; i < n; i++)
                {
                    Complex temp = A.arrays[i][k];
                    if (Complex.Abs(temp) < eps)
                    {
                        continue; // для нулевого коэффициента пропустить
                    }
                    for (int j = 0; j < n; j++)
                    {
                        A.arrays[i][j] = A.arrays[i][j] / temp;
                    }
                    B.arrays[i][0] = B.arrays[i][0] / temp;
                    if (i == k)
                    {
                        continue; // уравнение не вычитать само из себя
                    }

                    for (int j = 0; j < n; j++)
                    {
                        A.arrays[i][j] = A.arrays[i][j] - A.arrays[k][j];
                    }
                    B.arrays[i][0] = B.arrays[i][0] - B.arrays[k][0];
                }
                k++;
            }
            // 5 - обратная подстановка
            for (k = n - 1; k >= 0; k--)
            {
                X.arrays[k][0] = B.arrays[k][0];
                for (int i = 0; i < k; i++)
                {
                    B.arrays[i][0] = B.arrays[i][0] - A.arrays[i][k] * X.arrays[k][0];
                }
            }
            return X;
        }

        public static MatrixAlg GaussBackUp(MatrixAlg A, MatrixAlg B)
        {
            int n = B.arrays.GetLength(0);
            MatrixAlg X;
            double max;          

            int k, index;

            const double eps = 0.000001;  // точность
            X = new MatrixAlg(n, 1);
            k = 0;

            while (k < n)
            {
                // Поиск строки с максимальным a[i][k]
                max = Complex.Abs(A.arrays[k][k]);
                index = k;
                for (int i = k + 1; i < n; i++)
                {
                    if (Complex.Abs(A.arrays[i][k]) > max)
                    {
                        max = Complex.Abs(A.arrays[i][k]);
                        index = i;
                    }
                }


                // Перестановка строк
                if (max < eps)
                {
                    // нет ненулевых диагональных элементов
                    MessageBox.Show("");
                    break;
                    // cout << "Решение получить невозможно из-за нулевого столбца ";
                    // cout << index << " матрицы A" << endl;

                }
                for (int j = 0; j < n; j++)
                {
                    Complex temp = A.arrays[k][j];
                    A.arrays[k][j] = A.arrays[index][j];
                    A.arrays[index][j] = temp;
                }
                Complex tempVal = B.arrays[k][0];
                B.arrays[k][0] = B.arrays[index][0];
                B.arrays[index][0] = tempVal;
                

                // Нормализация уравнений
                for (int i = k; i < n; i++)
                {
                    Complex temp = A.arrays[i][k];
                    if (Complex.Abs(temp) < eps)
                    {
                        continue; // для нулевого коэффициента пропустить
                    }
                    for (int j = 0; j < n; j++)
                    {
                        A.arrays[i][j] = A.arrays[i][j] / temp;
                    }
                    B.arrays[i][0] = B.arrays[i][0] / temp;
                    if (i == k)
                    {
                        continue; // уравнение не вычитать само из себя
                    }

                    for (int j = 0; j < n; j++)
                    {
                        A.arrays[i][j] = A.arrays[i][j] - A.arrays[k][j];
                    }
                    B.arrays[i][0] = B.arrays[i][0] - B.arrays[k][0];
                }
                k++;
            }
            // обратная подстановка
            for (k = n - 1; k >= 0; k--)
            {
                X.arrays[k][0] = B.arrays[k][0];
                for (int i = 0; i < k; i++)
                {
                    B.arrays[i][0] = B.arrays[i][0] - A.arrays[i][k] * X.arrays[k][0];
                }
            }           
            return X;
        }
        private static void WriteMatrix(MatrixAlg A, string p)
        {
            StreamWriter sw = new StreamWriter(p);

            for (int i = 0; i < A.RowCount; i++)
            {
                for (int j = 0; j < A.ColumnCount; j++)
                {
                    sw.Write("{" + A.arrays[i][j].Real + " ; " + A.arrays[i][j].Imaginary + "} \t");
                }
                sw.WriteLine();
            }

            sw.Close();
        }
        private static void WriteDiagonalElements(MatrixAlg A, string p)
        {
            StreamWriter sw = new StreamWriter(p);

            for (int i = 0; i < A.RowCount; i++)
            {
                for (int j = 0; j < A.ColumnCount; j++)
                {
                    if (i == j)
                    {
                        sw.WriteLine(A.arrays[i][j].Real + "\t\t\t" + A.arrays[i][j].Imaginary);    
                    }                    
                }                
            }

            sw.Close();
        }
        public override string ToString()
        {
            string str = "";

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    str += arrays[i][j] + "\t";
                }
                str += "\n";
            }

            return str;
        }
        public static MatrixAlg TestMatrix3()
        {
            MatrixAlg TM = new MatrixAlg(3, 3);
            TM.arrays[0][0] = 1;
            TM.arrays[0][1] = 1;
            TM.arrays[0][2] = 1;
            TM.arrays[1][0] = 1;
            TM.arrays[1][1] = 1;
            TM.arrays[1][2] = 0;
            TM.arrays[2][0] = 0;
            TM.arrays[2][1] = 1;
            TM.arrays[2][2] = 1;
            return TM;
        }
        public static MatrixAlg TestAttached3()
        {
            MatrixAlg TA = new MatrixAlg(3,1);
            TA.arrays[0][0] = 0;
            TA.arrays[1][0] = new Complex(0, 1);
            TA.arrays[2][0] = 1;
            return TA;
        }
    }
}
