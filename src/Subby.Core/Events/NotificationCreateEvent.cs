using Subby.Core.Entities;
using Subby.Utilities.DomainEvents;

namespace Subby.Core.Events
{
    public class NotificationCreateEvent : DomainEventBase
    {
        public User User { get; set; }
        public string Type { get; set; }

        public NotificationCreateEvent(User user, string type)
        {
            User = user;
            Type = type;
        }
    }
}