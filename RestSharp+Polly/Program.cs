using Polly;
using RestSharp;
using RestSharp_Polly.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestSharp_Polly
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            string baseUrl = "https://localhost:5001/api";

            try
            {
                var client = new RestClient(baseUrl);

                var loginRequest = new RestRequest("/account/login", Method.POST);
                loginRequest.AddJsonBody(new { Email = "admin@test.com", Password = "Pa$$w0rd" });

                var retryPolicy =
                    Policy
                        .HandleResult<IRestResponse>(r => r.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        .OrResult(r => r.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        .OrResult(r => r.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                        .OrResult(r => r.ResponseStatus == ResponseStatus.Error)
                        .WaitAndRetryAsync(
                            retryCount: 3,
                             _ => TimeSpan.FromSeconds(1),
                             (result, timeSpan, retryCount, context) => {
                                 Console.WriteLine("Attempt: " + retryCount);
                             });

                var response = await retryPolicy.ExecuteAsync(async () => await client.ExecuteAsync(loginRequest));
                var user = response.HandleResponse<User>();

                if (user != null)
                    client.AddDefaultHeader("Authorization", "Bearer " + user.Token);


                var productsRequest = new RestRequest("/products", Method.GET);
                productsRequest.AddParameter("pageSize", 10);
                productsRequest.AddParameter("pageIndex", 1);
                response = await retryPolicy.ExecuteAsync(async () => await client.ExecuteAsync(productsRequest));
                var products = response.HandleResponse<Pagination<ProductToReturnDto>>();


                //Fail Registering
                var registerRequest = new RestRequest("/account/register", Method.POST);
                registerRequest.AddJsonBody(new { DisplayName = "Fernando" });
                response = await retryPolicy.ExecuteAsync(async () => await client.ExecuteAsync(registerRequest));
                response.HandleResponse();
                Console.WriteLine("Fim");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.ReadLine();
            }
        }
    }
}
