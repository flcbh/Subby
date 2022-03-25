using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using Subby.Core.Entities;
using Subby.Utilities.Interfaces;
using Subby.Web.Messages;
using Subby.Web.Models.FIrebaseViewModels;

namespace Subby.Web.Consumers
{
    public class NewJobConsumer : IConsumer<NewJobMessage>
    {
        private readonly IRepository _repository;

        public NewJobConsumer(IRepository repository)
        {
            _repository = repository;
        }
        
        public Task Consume(ConsumeContext<NewJobMessage> context)
        {
            var totalNewJobs = _repository.Linq<Job>().Include(x => x.Trade).Where(x => x.CreatedAt.Date >= DateTime.Now.AddDays(-7).Date).ToList();
            if (totalNewJobs.Count > 0)
            {
                var users = _repository.Linq<User>().Include(x => x.Trades).Include(x => x.Tokens).Where(x => !x.PushToken.Equals(string.Empty)).ToList();
                

                var model = new NewJobViewModel
                {
                    CollapseKey = "New Message",
                    Priority = "high"
                };

            
                foreach (var user in users)
                {
                    var trades = user.Trades.Select(x => x.Trade.Id).ToList();
                    if (!trades.Any())
                    {
                        continue;
                    }
                    
                    var userNewJobs = totalNewJobs.Count(x => trades.Contains(x.Trade.Id));
                    if (userNewJobs == 0) continue;

                    if (user.Tokens.Any())
                    {
                        foreach (var token in user.Tokens)
                        {
                            model.To = token.Token;
                            model.Notification = new Subby.Web.Models.FIrebaseViewModels.Notification
                            {
                                Title = $"Hi {user.FirstName}",
                                Body = $"{userNewJobs} jobs have been added to the Sustainability Yard app in your area this week."
                            };
                    
                            var client = new RestClient("https://fcm.googleapis.com/fcm/send");
                            var request = new RestRequest(Method.POST);
                            request.AddHeader("Authorization", "key=AAAAmmNT_wQ:APA91bHEe3rCs0EF2KD6XhP2jGHj01q4U3bcFu9aOggnxcX-mKSNNaRKQ-W8flECC3bNgzEdA69ke6J6KY1oY1ybFF8V0DPg7pabYa99N3PD7FNbQoSo4rstIld0AvseqEPVwFlVowl9AGrTPQQoX-ywTHWlS9Qk2A");
                            request.AddHeader("Content-Type", "application/json");
                            request.AddJsonBody(JsonConvert.SerializeObject(model));
                            var response = client.Execute(request);
                        }
                    }
                    else
                    {
                        model.To = user.PushToken;
                        model.Notification = new Subby.Web.Models.FIrebaseViewModels.Notification
                        {
                            Title = $"Hi {user.FirstName}",
                            Body = $"{userNewJobs} jobs have been added to the Sustainability Yard app in your area this week."
                        };
                    
                        var client = new RestClient("https://fcm.googleapis.com/fcm/send");
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("Authorization", "key=AAAAmmNT_wQ:APA91bHEe3rCs0EF2KD6XhP2jGHj01q4U3bcFu9aOggnxcX-mKSNNaRKQ-W8flECC3bNgzEdA69ke6J6KY1oY1ybFF8V0DPg7pabYa99N3PD7FNbQoSo4rstIld0AvseqEPVwFlVowl9AGrTPQQoX-ywTHWlS9Qk2A");
                        request.AddHeader("Content-Type", "application/json");
                        request.AddJsonBody(JsonConvert.SerializeObject(model));
                        var response = client.Execute(request);
                    }
                }
            }

            Console.WriteLine($"Push notification sent");
            return Task.CompletedTask;
        }
    }
}