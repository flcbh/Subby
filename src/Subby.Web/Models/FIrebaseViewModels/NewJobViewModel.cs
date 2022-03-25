using Newtonsoft.Json;

namespace Subby.Web.Models.FIrebaseViewModels
{
    public class NewJobViewModel
    {
        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("collapse_key")]
        public string CollapseKey { get; set; }

        [JsonProperty("priority")]
        public string Priority { get; set; }

        [JsonProperty("notification")]
        public Notification Notification { get; set; }
        
        [JsonProperty("data")]
        public Data Data { get; set; }
    }
    
    public class Notification
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }
    }
    
    public class Data
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("channel")]
        public int Channel { get; set; }

        [JsonProperty("click_action")]
        public string ClickAction { get; set; }
    }
}