namespace Subby.Utilities.DomainEvents
{
    public interface IDomainEvents
    {
        void Raise<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : DomainEventBase;
    }

}