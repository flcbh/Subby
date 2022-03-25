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
    public class ApplicationNotSuitableEventHandler  : IHandle<ApplicationNotSuitableEvent>
    {
        private readonly ISendInBlue _emailSender;
        public ApplicationNotSuitableEventHandler(ISendInBlue emailSender)
        {
            _emailSender = emailSender;
        }
        
        public void Handle(ApplicationNotSuitableEvent domainEvent)
        {
            Guard.Against.Null(domainEvent, nameof(domainEvent));

            _emailSender.Send(domainEvent.Email, domainEvent.Name, "Unsuccessful Application", domainEvent.Body);
        }
    }
}