using Subby.Core.Entities;
using Subby.Utilities.DomainEvents;

namespace Subby.Core.Events
{
    public class JobViewEvent : DomainEventBase
    {
        public Job Job { get; set; }

        public JobViewEvent(Job job)
        {
            Job = job;
        }
    }
}