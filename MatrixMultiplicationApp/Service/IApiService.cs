using MatrixMultiplicationApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MatrixMultiplicationApp.Service
{
    public interface IApiService
    {
        /// <summary>
        /// Initialize matrix A and B by calling the api
        /// </summary>
        /// <param name="matrixSize">Matrix size to initialize</param>
        /// <returns>Success</returns>
        Task<Boolean> Initialize(int matrixSize);

        Task<MatrixRowResponse> GetRow(Char matrix, int rowNum);

        Task<Boolean> Validate(String hash);
    }
}
