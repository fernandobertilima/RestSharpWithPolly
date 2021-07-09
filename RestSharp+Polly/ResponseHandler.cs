using Newtonsoft.Json;
using RestSharp;
using RestSharp_Polly.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestSharp_Polly
{
    public static class ResponseHandler
    {
        public static void HandleResponse(this IRestResponse response)
        {
            HandleResponse<object>(response, false);
        }
        public static T HandleResponse<T>(this IRestResponse response)
        {
            return HandleResponse<T>(response, true);
        }


        public static T HandleResponse<T>(IRestResponse response, bool containsBody)
        {
            var result = default(T);

            switch (response.StatusCode)
            {
                case 0:
                    Console.WriteLine("Exception: " + response.ErrorException.Message);
                    break;
                case System.Net.HttpStatusCode.OK:
                    if(containsBody)
                        result = response.Handle200<T>();
                    break;
                case System.Net.HttpStatusCode.BadRequest:
                    var badRequest = response.Handle400();
                    Console.WriteLine(badRequest.Message);
                    badRequest.Errors?.ForEach(e => Console.WriteLine(e));
                    break;
                case System.Net.HttpStatusCode.Unauthorized:
                    Console.WriteLine("Não autorizado");
                    break;
                case System.Net.HttpStatusCode.InternalServerError:
                    var error = response.Handle500();
                    Console.WriteLine(error.Message);
                    Console.WriteLine(error.Details);
                    break;
            }

            return result;
        }


        private static T Handle200<T>(this IRestResponse response)
        {
            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        private static BadRequestReturn Handle400(this IRestResponse response)
        {
            return JsonConvert.DeserializeObject<BadRequestReturn>(response.Content);
        }

        private static InternalServerErrorReturn Handle500(this IRestResponse response)
        {
            return JsonConvert.DeserializeObject<InternalServerErrorReturn>(response.Content);
        }
    }
}
