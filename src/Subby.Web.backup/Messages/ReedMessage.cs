using LastContent.ServiceBus;

namespace Subby.Web.Messages
{
    public class ReedMessage : BaseMessage
    {
        public string Name { get; } = "Reed Message Message";
        public string Description { get; } = "Daily Reed Import";
    }
}