using LastContent.ServiceBus;

namespace Subby.Web.Messages
{
    public class ValidateAdsMessage : BaseMessage
    {
        public string Name { get; } = "Validate Message";
        public string Description { get; } = "Daily Ads Import Validation";
    }
}