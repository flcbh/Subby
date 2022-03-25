using LastContent.ServiceBus;

namespace Subby.Web.Messages
{
    public class CvLibraryMessage : BaseMessage
    {
        public string Name { get; } = "CV Library Message";
        public string Description { get; } = "Daily CV Library Import";
    }
}