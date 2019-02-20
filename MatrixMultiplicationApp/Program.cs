using MatrixMultiplicationApp.Models;
using MatrixMultiplicationApp.Service;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MatrixMultiplicationApp
{
    class Program
    {
        static void Main(string[] args)
        {

            IApiService api = new ApiService();

            int rowCount = 1000;
            
            Int32[][] matrixA = new Int32[rowCount][];
            Int32[][] matrixB = new Int32[rowCount][];

            Task task = Task.Run(async () =>
            {

                Boolean initialized = await api.Initialize(rowCount);
                if(initialized)
                {
                    for(int i = 0; i < rowCount; i++)
                    {

                        MatrixRowResponse matrixARow = await api.GetRow('A', i);
                        matrixA[i] = matrixARow.Value;
                        MatrixRowResponse matrixBRow = await api.GetRow('B', i);
                        matrixB[i] = matrixBRow.Value;
                    }

                    var watch = System.Diagnostics.Stopwatch.StartNew();

                    Int32[][] matrixC = MultiplyMatrices(matrixA, matrixB);
                    watch.Stop();
                    int elapsed = (int)watch.ElapsedMilliseconds;
                    Console.WriteLine(String.Format("Multiplication took {0} milliseconds", elapsed));

                    Boolean validated = await api.Validate(GetMatrixMD5Hash(matrixC));

                    Console.WriteLine(String.Format("Result is validate: {0} ", validated));

                }
                
            });

            task.Wait();
            Console.ReadLine();
        }


        static Int32[][] MultiplyMatrices(Int32[][] matrixA, Int32[][] matrixB)
        {
            int rowCount = matrixA.Length;
            Int32[][] matrixC = new Int32[matrixA.Length][];

            for (int i = 0; i < rowCount; i++)
            {
                matrixC[i] = new Int32[matrixA.Length];
                for (int j = 0; j < rowCount; j++)
                {
                    matrixC[i][j] = 0;
                    for (int k = 0; k < rowCount; k++)
                    {
                        matrixC[i][j] += matrixA[i][k] * matrixB[k][j];
                    }
                }
            }

            return matrixC;
        }

        static string GetMatrixMD5Hash(Int32[][] matrix)
        {
            StringBuilder concatenatedValue = new StringBuilder();

            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix.Length; j++)
                {
                    concatenatedValue.Append(matrix[i][j]);
                }
            }

            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(concatenatedValue.ToString());
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
