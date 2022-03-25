using LastContent.ServiceBus;

namespace Subby.Web.Messages
{
    public class AdzunaMessage : BaseMessage
    {
        public string Name { get; } = "Adzuna Message Message";
        public string Description { get; } = "Daily Adzuna Import";
    }
}