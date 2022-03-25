using System;
using System.Linq;
using Ardalis.GuardClauses;
using LastContent.Utilities.Email;
using Microsoft.AspNetCore.Http;
using Subby.Core.Entities;
using Subby.Core.Events;
using Subby.Core.Interfaces;
using Subby.Utilities.DomainEvents;
using Subby.Utilities.Interfaces;

namespace Subby.Core.Handlers
{
    public class EmailEventHandler  : IHandle<EmailEvent>
    {
        private readonly ISendInBlue _emailSender;
        public EmailEventHandler(ISendInBlue emailSender)
        {
            _emailSender = emailSender;
        }
        
        public void Handle(EmailEvent domainEvent)
        {
            Guard.Against.Null(domainEvent, nameof(domainEvent));

            _emailSender.Send(domainEvent.Email, domainEvent.Name, domainEvent.Subject, domainEvent.Body);
        }
    }
}