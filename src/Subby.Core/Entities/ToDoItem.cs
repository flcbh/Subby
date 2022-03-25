using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class ToDoItem : BaseEntity
    {
        public virtual string Title { get; set; } = string.Empty;
        public virtual string Description { get; set; }
        // public bool IsDone { get; private set; }

        // public void MarkComplete()
        // {
        //     // IsDone = true;
        //
        //     // Events.Add(new ToDoItemCompletedEvent(this));
        // }

        // public override string ToString()
        // {
        //     string status = IsDone ? "Done!" : "Not done.";
        //     return $"{Id}: Status: {status} - {Title} - {Description}";
        // }
    }
}
