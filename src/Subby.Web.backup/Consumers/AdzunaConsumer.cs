using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LastContent.Utilities.Extensions;
using MassTransit;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using RestSharp;
using Subby.Core.Entities;
using Subby.Utilities.Extensions;
using Subby.Utilities.Interfaces;
using Subby.Web.Messages;
using Subby.Workers.Models;

namespace Subby.Web.Consumers
{
    public class AdzunaConsumer : IConsumer<AdzunaMessage>
    {
        private readonly IRepository _repository;

        public AdzunaConsumer(IRepository repository)
        {
            _repository = repository;
        }

        public Task Consume(ConsumeContext<AdzunaMessage> context)
        {
            
            var apiUrl = "http://api.adzuna.com/v1/api";
            var appId = "8bbc8ae1";
            var appKey = "fc5a75ceb75ee34feab837fc0c70ac38";
            
            var listOfKeywords = new List<string>()
            {
                "plumber", "electrician", "bricklayer", "joiner", "carpenter", "roofer ", "tiler ", "plasterer ",
                "tenderer ", "groundwork ", "plant machinery driver", "machine driver", "cctv & telco",
                "heating engineer", "gas engineer", "fencers", "extension builders ", "driveway pavers ", "demolition ",
                "architects ", "Surveyors ", "landscape gardeners ", "new home builders ", "painter", "decorator ",
                "labourer ", "handyman", "site foreman ", "site manager", "quantity surveyor"
            };

            var tempJobs = new List<Result>();
            var jobs = 0;

            foreach (var k in listOfKeywords)
            {
                var client =
                    new RestClient(
                        $"{apiUrl}/jobs/gb/search/1?app_id={appId}&app_key={appKey}&results_per_page=100&what={k}&where=uk&content-type=application/json");
                var request = new RestRequest(Method.GET);
                var response = client.Execute<AdzunaResponse>(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var data = JsonConvert.DeserializeObject<AdzunaResponse>(response.Content);
                    
                    foreach (var item in data.Results)
                    {
                        tempJobs.Add(item);
                    }
                }
            }

            if (tempJobs.Any())
            {
                tempJobs.Shuffle();
                foreach (var item in tempJobs)
                {
                    if (item.Description.ToLower().Contains("checkatrade"))
                    {
                        continue;
                    }
                    
                    var apiJobExist = _repository.Linq<Job>().FirstOrDefault(x => x.Reference == item.Id.ToString() && x.Source == "Adzuna");
                    if (apiJobExist != null) continue;
                    
                    var trade = _repository.Linq<Trade>().FirstOrDefault(x => x.Name == item.Category.Label);
                    if (trade == null) continue;
                        
                    var slug = (item.Title + " " + item.Location.DisplayName + " " + item.Id).ToSlug();
                    var jobExists = _repository.Linq<Job>().FirstOrDefault(x => x.Slug == slug);
                    if (jobExists != null) continue;

                    if (string.IsNullOrEmpty(item.Latitude) || string.IsNullOrEmpty(item.Longitude) || item.Description.Contains("checkatrade"))
                    {
                        continue;
                    }
                    
                    if (item.Description.StripHTML().Contains("checkatrade"))
                    {
                        continue;
                    }
                        
                    var job = new Job
                    {
                        Latitude = Convert.ToDouble(item.Latitude),
                        Longitude = Convert.ToDouble(item.Longitude),
                        Title = item.Title.StripHTML(),
                        Description = item.Description.StripHTML(),
                        Location = item.Location.DisplayName,
                        ExternalLink = item.RedirectUrl.ToString(),
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Budget = "£" + item.SalaryMax.ToString(CultureInfo.InvariantCulture),
                        Slug = slug,
                        DeadLine = DateTime.Now.AddMonths(1).ToString("dd/MM/yyyy"),
                        IsExternal = true,
                        Source = "Adzuna",
                        Reference = item.Id.ToString(),
                        Trade = trade
                    };

                    var contractType = "Contract";

                    if (!string.IsNullOrEmpty(item.ContractTime))
                    {
                        if (item.ContractTime == "full_time")
                        {
                            contractType = "Full Time";
                        }
                    }

                    job.ContractType = contractType;

                    _repository.Add(job);

                    jobs += 1;
                }
            }
            
            Console.WriteLine($"Total {jobs} added");
            return Task.CompletedTask;
        }
    }
}