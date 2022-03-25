using LastContent.ServiceBus;

namespace Subby.Web.Messages
{
    public class NewJobMessage : BaseMessage
    {
        public string Name { get; } = "New Job Message";
        public string Description { get; } = "Weekly New Job Message";
    }
}