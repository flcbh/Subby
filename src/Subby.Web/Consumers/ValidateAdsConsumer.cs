using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LastContent.Utilities.Extensions;
using MassTransit;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using Subby.Core.Entities;
using Subby.Utilities.Extensions;
using Subby.Utilities.Interfaces;
using Subby.Web.Messages;
using Subby.Workers.Models;

namespace Subby.Web.Consumers
{
    public class ValidateAdsConsumer : IConsumer<ValidateAdsMessage>
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;

        public ValidateAdsConsumer(IRepository repository, ILogger<ValidateAdsConsumer> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public Task Consume(ConsumeContext<ValidateAdsMessage> context)
        {

            try
            {
            
                var sproc = _repository.BuildStoredProcedure();
                sproc.ExecuteNonQuery("[dbo].[ClearOldJobs]");
                
                _logger.LogInformation("Clear jobs executed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Clear job failed");
            }

            return Task.CompletedTask;
        }
    }
}