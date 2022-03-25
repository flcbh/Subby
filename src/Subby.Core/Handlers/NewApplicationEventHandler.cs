using System;
using System.Linq;
using Ardalis.GuardClauses;
using Subby.Core.Entities;
using Subby.Core.Events;
using Subby.Utilities.DomainEvents;
using Subby.Utilities.Interfaces;

namespace Subby.Core.Handlers
{
    public class NewApplicationEventHandler : IHandle<NewApplicationEvent>
    {
        private readonly IRepository _repository;
        public NewApplicationEventHandler(IRepository repository)
        {
            _repository = repository;
        }
        
        public void Handle(NewApplicationEvent domainEvent)
        {
            Guard.Against.Null(domainEvent, nameof(domainEvent));

            var applicationNotification = _repository
                .Linq<Notification>().FirstOrDefault(x => x.Type == "NEW_APPLICATIONS" && !x.Read && x.User == domainEvent.User);

            if (applicationNotification != null)
            {
                var notificationCount = Convert.ToInt32(applicationNotification.Value);
                applicationNotification.Value = (notificationCount + 1).ToString();
                _repository.Update(applicationNotification);
            }
            else
            {
                _repository.Add(new Notification
                {
                    User = domainEvent.User,
                    Type = "NEW_APPLICATIONS",
                    Value = "1",
                    CreatedAt = DateTime.Now
                });
            }
        }
    }
}