using Subby.Core.Entities;
using Subby.Utilities.DomainEvents;

namespace Subby.Core.Events
{
    public class JobInterestEvent : DomainEventBase
    {
        public Job Job { get; set; }
        public bool IsInterested { get; set; }
        
        public User User { get; set; }

        public JobInterestEvent(Job job, bool isInterested, User user)
        {
            Job = job;
            IsInterested = isInterested;
            User = user;
        }
    }
}