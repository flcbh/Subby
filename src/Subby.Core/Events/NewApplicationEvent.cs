using Subby.Core.Entities;
using Subby.Utilities.DomainEvents;

namespace Subby.Core.Events
{
    public class NewApplicationEvent : DomainEventBase
    {
        public User User { get; set; }

        public NewApplicationEvent(User user)
        {
            User = user;
        }
    }
}