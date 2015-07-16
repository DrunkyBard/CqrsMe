namespace Cqrs.Domain.EventSourcing
{
    public interface IEventSourced
    {
        int Version { get; }
    }
}
