using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LastContent.Utilities.Extensions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using Subby.Core.Entities;
using Subby.Utilities.Interfaces;
using Subby.Web.Messages;
using Subby.Workers.Models;

namespace Subby.Web.Consumers
{
    public class ReedConsumer : IConsumer<ReedMessage>
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;

        public ReedConsumer(IRepository repository, ILogger<ReedConsumer> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public Task Consume(ConsumeContext<ReedMessage> context)
        {
            var listOfKeywords = new List<string>()
            {
                "plumber", "electrician", "bricklayer", "joiner", "carpenter", "roofer ", "tiler ", "plasterer ",
                "tenderer ", "groundwork ", "plant machinery driver", "machine driver", "cctv & telco",
                "heating engineer", "gas engineer", "fencers", "extension builders ", "driveway pavers ", "demolition ",
                "architects ", "Surveyors ", "landscape gardeners ", "new home builders ", "painter", "decorator ",
                "labourer ", "handyman", "site foreman ", "site manager", "quantity surveyor"
            };

            foreach (var keyword in listOfKeywords)
            {
                var client = new RestClient($"https://www.reed.co.uk/api/1.0/search?keywords={keyword}&location=uk&permanent=false&contract=true");
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", "Basic MGZkMWIzZmEtYzY2Yi00ZWZjLTliYjMtY2Q3ZjVmOWJjYTFmOg==");

                var response = client.Execute<ReedResponse>(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var data = JsonConvert.DeserializeObject<ReedResponse>(response.Content);
                    foreach (var item in data.Results)
                    {
                        if (item.JobDescription.ToLower().Contains("checkatrade"))
                        {
                            continue;
                        }

                        if (item.MinimumSalary <= 0)
                        {
                            continue;
                        }
                        
                        var trade = _repository.Linq<Trade>().FirstOrDefault(x => x.Name.Contains(keyword));
                        if (trade == null) continue;
                        
                        var slug = (item.JobTitle + " Reed " + item.JobId).ToSlug();
                        var jobExists = _repository.Linq<Job>().FirstOrDefault(x => x.Reference == item.JobId.ToString() && x.Source == "Reed");
                        if (jobExists != null) continue;

                        try
                        {
                            AddJob(item, slug, trade);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed Reed Job Importer" + ex.Message);
                        }
                        
                    }
                }
            }
            
            // Console.WriteLine($"Total Reed {jobs} added");
            return Task.CompletedTask;
        }

        private void AddJob(ReedResult item, string slug, Trade trade)
        {
            var datetime = DateTime.Now.AddMonths(1);
            if (DateTime.TryParse(item.ExpirationDate, out DateTime result))
            {
                // Use result.
                datetime = result;
            }
            
            var job = new Job
            {
                Title = item.JobTitle.StripHTML(),
                Description = item.JobDescription.StripHTML(),
                Location = item.LocationName,
                ExternalLink = item.JobUrl.ToString(),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Budget = "£" + item.MinimumSalary,
                Slug = slug,
                DeadLine = datetime.ToString("dd/MM/yyyy"),
                IsExternal = true,
                Source = "Reed",
                Reference = item.JobId.ToString(),
                ContractType = "Contract",
                Trade = trade
            };

            _repository.Add(job);

        }
    }
}