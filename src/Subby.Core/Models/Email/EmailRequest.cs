using Newtonsoft.Json;

namespace Subby.Core.Models.Email
{
    public class EmailRequest
    {
        [JsonProperty("sender")]
        public Sender Sender { get; set; }

        [JsonProperty("to")]
        public Sender[] To { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("htmlContent")]
        public string HtmlContent { get; set; }
    }
    
    public class Sender
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}