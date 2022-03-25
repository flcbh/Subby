using System;
using System.Linq;
using Ardalis.GuardClauses;
using Subby.Core.Entities;
using Subby.Core.Events;
using Subby.Utilities.DomainEvents;
using Subby.Utilities.Interfaces;

namespace Subby.Core.Handlers
{
    public class NotificationReadEventHandler: IHandle<NotificationReadEvent>
    {
        private readonly IRepository _repository;
        public NotificationReadEventHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(NotificationReadEvent domainEvent)
        {
            Guard.Against.Null(domainEvent, nameof(domainEvent));

            var notification = _repository.Linq<Notification>()
                .FirstOrDefault(x => x.Type == domainEvent.Type && x.User == domainEvent.User && !x.Read);

            if (notification != null)
            {
                notification.Read = true;
                notification.ReadAt = DateTime.Now;
                _repository.Update(notification);
            }

        }
    }
}