using Ardalis.GuardClauses;
using Subby.Utilities.DomainEvents;
using Subby.Core.Events;


namespace Subby.Core.Handlers
{
    public class ItemCompletedEmailNotificationHandler : IHandle<ToDoItemCompletedEvent>
    {

        // public ItemCompletedEmailNotificationHandler(IEmailSender emailSender)
        // {
        //     _emailSender = emailSender;
        // }

        // configure a test email server to demo this works
        // https://ardalis.com/configuring-a-local-test-email-server

        public void Handle(ToDoItemCompletedEvent domainEvent)
        {
            Guard.Against.Null(domainEvent, nameof(domainEvent));
        }
    }
}
