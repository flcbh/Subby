using System;
using System.Collections.Generic;
using Subby.Utilities.DomainEvents;
using Microsoft.Extensions.Logging;

namespace Subby.Infrastructure
{
    public class DomainEvents : IDomainEvents
    {
        private readonly Func<Type, object> _eventHandlerFactory;
        private readonly ILogger _logger;
        public DomainEvents(Func<Type, object> eventHandlerFactory)
        {
            _eventHandlerFactory = eventHandlerFactory;
            _logger = new LoggerFactory().CreateLogger(typeof(DomainEvents));

        }

        public void Raise<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : DomainEventBase
        {
            var handlers = _eventHandlerFactory(typeof(IEnumerable<IHandle<TDomainEvent>>));

            if (handlers as IEnumerable<IHandle<TDomainEvent>> != null)
            {
                foreach (var handler in handlers as IEnumerable<IHandle<TDomainEvent>>)
                {
                    try
                    {
                        handler.Handle(domainEvent);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Exception while handling domain event");
                    }
                }
            }
        }
    }
}