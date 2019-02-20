using MatrixMultiplicationApp.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Formatting;

namespace MatrixMultiplicationApp.Service
{
    public class ApiService : IApiService
    {
        private static HttpClient client = new HttpClient();
        private static readonly int MAX_RETRY_COUNT = 5;
        private static readonly int RETRY_TIMEOUT = 1000;
        private static readonly string URL = "https://recruitment-test.investcloud.com/api/numbers";

        public async Task<Boolean> Initialize(int matrixSize)
        {
            return await WithRetry(async () =>
            {
                var response = await SendHttpRequest(HttpMethod.Get, String.Format("{0}/init/{1}", URL, matrixSize), null);
                response.EnsureSuccessStatusCode();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ApiResponse res = await response.Content.ReadAsAsync<ApiResponse>();
                    return res.Success;
                }
                return false;
            });
        }

        public async Task<MatrixRowResponse> GetRow(Char matrix, int rowNum)
        {
            return await WithRetry(async () =>
            {
                MatrixRowResponse row = null;
                var response = await SendHttpRequest(HttpMethod.Get, String.Format("{0}/{1}/row/{2}", URL, matrix, rowNum), null);
                response.EnsureSuccessStatusCode();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    row = await response.Content.ReadAsAsync<MatrixRowResponse>();
                }
                return row;
            });
        }

        public async Task<Boolean> Validate(String hash)
        {
            return await WithRetry(async () =>
            {
                var response = await SendHttpRequest(HttpMethod.Post, String.Format("{0}/validate", URL), new StringContent(hash, UnicodeEncoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ApiResponse res = await response.Content.ReadAsAsync<ApiResponse>();
                    return res.Success;
                }
                return false;
            });
        }

        private async Task<HttpResponseMessage> SendHttpRequest(HttpMethod method, string url, HttpContent content)
        {
            var request = new HttpRequestMessage(method, url);
            if (content != null)
            {
                request.Content = content;
            }
            var response = await client.SendAsync(request);
            return response;
        }

        private async Task<T> WithRetry<T>(Func<Task<T>> func)
        {
            T result = default(T);
            int retryCount = MAX_RETRY_COUNT;
            while (retryCount >= 0)
            {
                try
                {
                    result = await func();
                    break;
                }
                catch (Exception exception)
                {
                    if (retryCount > 0)
                    {
                        retryCount--;
                        Thread.Sleep(RETRY_TIMEOUT * (MAX_RETRY_COUNT - retryCount));
                    }
                    else
                    {
                        throw exception;
                    }
                }
            }

            return result;
        }



    }
}
