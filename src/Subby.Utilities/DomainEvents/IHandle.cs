namespace Subby.Utilities.DomainEvents
{
    public interface IHandle<in TDomainEvent> where TDomainEvent : class
    {
        void Handle(TDomainEvent domainEvent);
    }
}