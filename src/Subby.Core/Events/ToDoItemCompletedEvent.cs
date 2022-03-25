using Subby.Core.Entities;
using Subby.Utilities;
using Subby.Utilities.DomainEvents;

namespace Subby.Core.Events
{
    public class ToDoItemCompletedEvent : DomainEventBase
    {
        public ToDoItem CompletedItem { get; set; }

        public ToDoItemCompletedEvent(ToDoItem completedItem)
        {
            CompletedItem = completedItem;
        }
    }
}