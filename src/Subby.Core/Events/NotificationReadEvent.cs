using Subby.Core.Entities;
using Subby.Utilities.DomainEvents;

namespace Subby.Core.Events
{
    public class NotificationReadEvent : DomainEventBase
    {
        public User User { get; set; }
        public string Type { get; set; }

        public NotificationReadEvent(User user, string type)
        {
            User = user;
            Type = type;
        }
    }
}