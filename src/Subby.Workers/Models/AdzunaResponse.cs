using System;
using Newtonsoft.Json;

namespace Subby.Workers.Models
{
    public class AdzunaResponse
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("mean")]
        public double Mean { get; set; }

        [JsonProperty("results")]
        public Result[] Results { get; set; }
    }

    public class Result
    {
        [JsonProperty("salary_max")]
        public double SalaryMax { get; set; }

        [JsonProperty("salary_min")]
        public double SalaryMin { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("longitude", NullValueHandling = NullValueHandling.Ignore)]
        public string Longitude { get; set; }

        [JsonProperty("redirect_url")]
        public Uri RedirectUrl { get; set; }

        [JsonProperty("category")]
        public Category Category { get; set; }

        [JsonProperty("salary_is_predicted")]
        public long SalaryIsPredicted { get; set; }

        [JsonProperty("latitude", NullValueHandling = NullValueHandling.Ignore)]
        public string Latitude { get; set; }
        
        [JsonProperty("contract_time")]
        public string ContractTime { get; set; }
        
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("created")]
        public DateTimeOffset Created { get; set; }

        [JsonProperty("adref")]
        public string Adref { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("company")]
        public Company Company { get; set; }
    }

    public partial class Category
    {

        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }
    }

    public class Company
    {

        [JsonProperty("display_name", NullValueHandling = NullValueHandling.Ignore)]
        public string DisplayName { get; set; }
    }

    public class Location
    {
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("area")]
        public string[] Area { get; set; }
    }
}