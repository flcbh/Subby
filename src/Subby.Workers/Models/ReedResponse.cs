using System;
using Newtonsoft.Json;

namespace Subby.Workers.Models
{
    public class ReedResponse
    {
        [JsonProperty("results")]
        public ReedResult[] Results { get; set; }
        
        [JsonProperty("ambiguousLocations")]
        public object[] AmbiguousLocations { get; set; }

        [JsonProperty("totalResults")]
        public long TotalResults { get; set; }
    }
    
    public class ReedResult
    {
        [JsonProperty("jobId")]
        public long JobId { get; set; }

        [JsonProperty("employerId")]
        public long EmployerId { get; set; }

        [JsonProperty("employerName")]
        public string EmployerName { get; set; }

        [JsonProperty("employerProfileId")]
        public object EmployerProfileId { get; set; }

        [JsonProperty("employerProfileName")]
        public object EmployerProfileName { get; set; }

        [JsonProperty("jobTitle")]
        public string JobTitle { get; set; }

        [JsonProperty("locationName")]
        public string LocationName { get; set; }

        [JsonProperty("minimumSalary")]
        public double? MinimumSalary { get; set; }

        [JsonProperty("maximumSalary")]
        public double? MaximumSalary { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("expirationDate")]
        public string ExpirationDate { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("jobDescription")]
        public string JobDescription { get; set; }

        [JsonProperty("applications")]
        public long Applications { get; set; }

        [JsonProperty("jobUrl")]
        public Uri JobUrl { get; set; }
    }
}