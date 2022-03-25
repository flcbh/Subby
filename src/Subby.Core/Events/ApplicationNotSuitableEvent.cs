using Subby.Core.Entities;
using Subby.Utilities.DomainEvents;

namespace Subby.Core.Events
{
    public class ApplicationNotSuitableEvent : DomainEventBase
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Body { get; set; }

        public ApplicationNotSuitableEvent(string email, string body, string name)
        {
            Email = email;
            Body = body;
            Name = name;
        }
    }
}