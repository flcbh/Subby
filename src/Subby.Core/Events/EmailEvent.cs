using Subby.Core.Entities;
using Subby.Utilities.DomainEvents;

namespace Subby.Core.Events
{
    public class EmailEvent : DomainEventBase
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Body { get; set; }
        
        public string Subject { get; set; }

        public EmailEvent(string email, string body, string subject, string name)
        {
            Email = email;
            Body = body;
            Subject = subject;
            Name = name;
        }
    }
}