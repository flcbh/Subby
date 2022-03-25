using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using RestSharp;
using System.Xml.Serialization;
using LastContent.Utilities.Extensions;
using Subby.Core.Entities;
using Subby.Utilities.Interfaces;
using Subby.Web.Messages;
using Subby.Workers.Models;

namespace Subby.Web.Consumers
{
    public class CvLibraryConsumer : IConsumer<CvLibraryMessage>
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;

        public CvLibraryConsumer(IRepository repository, ILogger<CvLibraryConsumer> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public Task Consume(ConsumeContext<CvLibraryMessage> context)
        {
            
            var apiUrl = "http://www.cv-library.co.uk/cgi-bin/feed.xml?affid=104422";
            
            var client = new RestClient(apiUrl);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/xml");

            var response = client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"CvLibrary Import Failed ${response.Content} status: {response.StatusCode}");
                return Task.CompletedTask;
            }

            var result = new CvLibraryResponse();
            
            XmlSerializer serializer = new XmlSerializer(typeof(CvLibraryResponse));
            using (StringReader reader = new StringReader(response.Content))
            {
                result = (CvLibraryResponse)(serializer.Deserialize(reader));
                
            }

            var jobs = 0;
            if (result.Job.Count > 0)
            {
                foreach (var item in result.Job)
                {
                    if (item.Description.ToLower().Contains("checkatrade"))
                    {
                        continue;
                    }

                    var trade = _repository.Linq<Trade>().FirstOrDefault(x => x.Name.Contains(item.Category));
                    if (trade == null) continue;
                        
                    var slug = (item.Title + " " + item.Company + " " + item.Jobref).ToSlug();
                    var jobExists = _repository.Linq<Job>().FirstOrDefault(x => x.Reference == item.Jobref && x.Source == "CvLibrary");
                    if (jobExists != null) continue;
                    
                    var job = new Job
                    {
                        Title = item.Title.StripHTML(),
                        Description = item.Description.StripHTML(),
                        Location = item.Location,
                        ExternalLink = item.Url,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Budget = "£" + item.Salarymax.ToString(CultureInfo.InvariantCulture) + "/" + item.Salary_per.ToLower(),
                        Slug = slug,
                        DeadLine = DateTime.Now.AddMonths(1).ToString("dd/MM/yyyy"),
                        IsExternal = true,
                        Source = "CvLibrary",
                        Reference = item.Jobref,
                        ContractType = item.Full_part,
                        Trade =  trade
                    };
                    
                    _repository.Add(job);
                    jobs += 1;
                }
            }

            Console.WriteLine($"Total CvLibrary {jobs} added");
            return Task.CompletedTask;
        }
    }
}