using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Integral
{
    struct ReIm
    {
        public double a;
        public double b;
    }
    public static unsafe class MatrixSolution
    {
        [DllImport("gauss_complex_p.dll", CallingConvention = CallingConvention.Cdecl)]
        //static extern ReIm* gauss_complex_p(ref ReIm[,] matrix, ReIm[] b, int n, int proc);
        static extern ReIm* gauss_complex_p_m(ref Complex[,] matrix, int size_a, int size_b, ref int proc);
        //public static int Processes {get; set;}
        //public static int Size { get; set; }

        public static Complex[] Solve(Complex[,] matrix)
        {
            int processes = ThinWireAprox.DegreeOfParallelism;
            int size_A = matrix.GetLength(0);
            int size_b = matrix.GetLength(1) - size_A;


            ReIm* solution = gauss_complex_p_m(ref matrix, size_A, size_b, ref processes);

            Complex[] answer = new Complex[size_A];
            for (int i = 0; i < size_A; i++)
            {
                answer[i] = new Complex(solution[i].a, solution[i].b);
            }
            return answer;
        }
        public static Complex[] SolveOriginal(Complex[,] A, Complex[] B)
        {
            //1 - Инициализация
            int n = B.Length;
            Complex[] X = new Complex[n];
            double max;

            int k = 0, index;
            const double eps = 0.000001;  // точность
            

            while (k < n)
            {
                // 2 - Поиск строки с наибольшим элементом в ведущем столбце
                max = Complex.Abs(A[k, k]);
                index = k;
                for (int i = k + 1; i < n; i++)
                {
                    if (Complex.Abs(A[i, k]) > max)
                    {
                        max = Complex.Abs(A[i, k]);
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
                    Complex temp = A[k, j];
                    A[k, j] = A[index, j];
                    A[index, j] = temp;
                }
                Complex tempVal = B[k];
                B[k] = B[index];
                B[index] = tempVal;


                // 4 - Нормализация уравнений и вычитание
                for (int i = k; i < n; i++)
                {
                    Complex temp = A[i, k];
                    if (Complex.Abs(temp) < eps)
                    {
                        continue; // для нулевого коэффициента пропустить
                    }
                    for (int j = 0; j < n; j++)
                    {
                        A[i, j] = A[i, j] / temp;
                    }
                    B[i] = B[i] / temp;
                    if (i == k)
                    {
                        continue; // уравнение не вычитать само из себя
                    }

                    for (int j = 0; j < n; j++)
                    {
                        A[i, j] = A[i, j] - A[k, j];
                    }
                    B[i] = B[i] - B[k];
                }
                k++;
            }
            // 5 - обратная подстановка
            for (k = n - 1; k >= 0; k--)
            {
                X[k] = B[k];
                for (int i = 0; i < k; i++)
                {
                    B[i] = B[i] - A[i, k] * X[k];
                }
            }
            return X;
        }

        public static Complex[] SolveParallel(Complex[,] A, Complex[] B)
        {
            //1 - 
            var options = new ParallelOptions() { MaxDegreeOfParallelism = ThinWireAprox.DegreeOfParallelism };
            int n = B.Length;
            Complex[] X = new Complex[n];
            double max;         //квадрат элемента    
            int k = 0, index;

            const double eps = 0.000001;  // точность
            

            while (k < n)
            {
                // Поиск строки с максимальным a[i][k]
                //max = A.arrays[k][k].Real * A.arrays[k][k].Real + A.arrays[k][k].Imaginary * A.arrays[k][k].Imaginary;
                max = Complex.Abs(A[k, k]);
                index = k;
                for (int i = k + 1; i < n; i++)
                {
                    //double val = A.arrays[i][k].Real * A.arrays[i][k].Real + A.arrays[i][k].Imaginary * A.arrays[i][k].Imaginary;
                    double val = Complex.Abs(A[i, k]);
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
                    Complex temp = A[k, j];
                    A[k, j] = A[index, j];
                    A[index, j] = temp;
                }
                Complex tempVal = B[k];
                B[k] = B[index];
                B[index] = tempVal;

                Complex firstVal = A[k, k];
                if (!(Complex.Abs(firstVal) < eps))// для нулевого коэффициента пропустить
                {
                    for (int j = 0; j < n; j++)
                    {
                        A[k, j] = A[k, j] / firstVal;
                    }
                    B[k] = B[k] / firstVal;
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
                X[k] = B[k];
                for (int i = 0; i < k; i++)
                {
                    B[i] = B[i] - A[i, k] * X[k];
                }
            }            
            return X;
        }
          
        
        static void SubProc(Complex[,] A, Complex[] B, int i, int k, int n)
        {
            const double eps = 0.000001;  // точность
            Complex temp = A[i, k];
            if (!(Complex.Abs(temp) < eps))// для нулевого коэффициента пропустить
            {
                // Нормализация уравнений
                for (int j = 0; j < n; j++)
                {
                    A[i, j] = A[i, j] / temp;
                }
                B[i] = B[i] / temp;

                for (int j = 0; j < n; j++)
                {
                    A[i, j] = A[i, j] - A[k, j];
                }
                B[i] = B[i] - B[k];
            }
        }
    }
}
