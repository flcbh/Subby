using System.Collections.Generic;
using Newtonsoft.Json;

namespace Subby.Utilities.Models
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public List<string> Errors {get; set;} = new List<string>();

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}