using System;
using System.Collections.Generic;
using System.Text;

namespace RestSharp_Polly.Models
{
    public class InternalServerErrorReturn
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }
}
