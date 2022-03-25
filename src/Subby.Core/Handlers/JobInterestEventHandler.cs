using System;
using System.Linq;
using Ardalis.GuardClauses;
using Subby.Core.Entities;
using Subby.Core.Events;
using Subby.Utilities.DomainEvents;
using Subby.Utilities.Interfaces;

namespace Subby.Core.Handlers
{
    public class JobInterestEventHandler: IHandle<JobInterestEvent>
    {
        private readonly IRepository _repository;
        
        public JobInterestEventHandler(IRepository repository)
        {
            _repository = repository;
        }
        
        public void Handle(JobInterestEvent domainEvent)
        {
            Guard.Against.Null(domainEvent, nameof(domainEvent));

            
            var jobInterest = _repository.Linq<JobInterest>()
                .FirstOrDefault(x => x.Job == domainEvent.Job && x.User == domainEvent.User);

         
            if (jobInterest == null)
            {
                var newInterest = new JobInterest
                {
                    Job = domainEvent.Job,
                    CreatedAt = DateTime.Now,
                    IsInterested = domainEvent.IsInterested,
                    User = domainEvent.User
                };
                _repository.Add(newInterest);
            }
            else
            {
                jobInterest.IsInterested = domainEvent.IsInterested;
                _repository.Update(jobInterest);
            }
        }
    }
}