using System;
using System.Linq;
using Ardalis.GuardClauses;
using Subby.Core.Entities;
using Subby.Core.Events;
using Subby.Utilities.DomainEvents;
using Subby.Utilities.Interfaces;

namespace Subby.Core.Handlers
{
    public class NotificationCreateEventHandler : IHandle<NotificationCreateEvent>
    {
        private readonly IRepository _repository;
        public NotificationCreateEventHandler(IRepository repository)
        {
            _repository = repository;
        }
        
        public void Handle(NotificationCreateEvent domainEvent)
        {
            Guard.Against.Null(domainEvent, nameof(domainEvent));

            var getActiveNotification = _repository.Linq<Notification>()
                .FirstOrDefault(x => x.User == domainEvent.User && !x.Read && x.Type == domainEvent.Type);
            if (getActiveNotification == null)
            {
                _repository.Add(new Notification()
                {
                    User = domainEvent.User,
                    Type = domainEvent.Type,
                    CreatedAt = DateTime.Now,
                    Read = false
                });

            } 
        }
    }
}