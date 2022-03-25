using System;
using System.Linq;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Http;
using Subby.Core.Entities;
using Subby.Core.Events;
using Subby.Utilities.DomainEvents;
using Subby.Utilities.Interfaces;

namespace Subby.Core.Handlers
{
    public class JobViewEventHandler  : IHandle<JobViewEvent>
    {
        private readonly IRepository _repository;
        public JobViewEventHandler(IRepository repository)
        {
            _repository = repository;
        }
        
        public void Handle(JobViewEvent domainEvent)
        {
            Guard.Against.Null(domainEvent, nameof(domainEvent));

            var currentActivity = _repository.Linq<JobActivity>()
                .FirstOrDefault(x => x.Job == domainEvent.Job && x.CreatedAt.Date == DateTime.UtcNow.Date);

            var jobActivity = new JobActivity
            {
                Job = domainEvent.Job,
                CreatedAt = DateTime.Now,
            };
            if (currentActivity == null)
            {
                jobActivity.Count = 1;
                _repository.Add(jobActivity);
            }
            else
            {
                currentActivity.Count += 1;
                _repository.Update(currentActivity);
            }
        }
    }
}