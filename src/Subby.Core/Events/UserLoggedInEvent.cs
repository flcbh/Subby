using Subby.Utilities.DomainEvents;

namespace Subby.Core.Events
{
    public class UserLoggedInEvent : DomainEventBase
    {
        public string Email { get; set; }
        public string PushToken { get; set; }

        public UserLoggedInEvent(string email, string pushToken)
        {
            Email = email;
            PushToken = pushToken;
        }
    }
}