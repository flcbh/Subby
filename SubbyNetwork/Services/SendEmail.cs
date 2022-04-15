using LastContent.Utilities.Email;
using Newtonsoft.Json;
using RestSharp;
using SubbyNetwork.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubbyNetwork.Models;

namespace SubbyNetwork.Services
{
    public class SendEmail : ISendEmail
    {

        public void Send(string to, string toName, string subject, string body)
        {
            var client = new RestClient("https://api.sendinblue.com/v3/smtp/email");
            var request = new RestRequest("https://api.sendinblue.com/v3/smtp/email", Method.Post);
            request.AddHeader("api-key", "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");
            request.AddHeader("Content-Type", "application/json");

            var requestBody = new EmailRequest
            {
                Subject = subject,
                Sender = new Sender
                {
                    Name = "Subby Network",
                    Email = "nigel@subbynetwork.com"
                },
                To = new[]
                {
                    new Sender
                    {
                        Name = toName,
                        Email = to
                    }
                },
                HtmlContent = body
            };

            request.AddParameter("application/json", JsonConvert.SerializeObject(requestBody), ParameterType.RequestBody);
            client.ExecuteAsync(request);
        }
    }
}
