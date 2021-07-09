using System;
using System.Collections.Generic;
using System.Text;

namespace RestSharp_Polly.Models
{
    public class BadRequestReturn
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
    }
}
