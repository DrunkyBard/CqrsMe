namespace Cqrs.Domain.EventSourcing
{
    public interface IOriginator<out TSnapshot> where TSnapshot : Snapshot
    {
        TSnapshot GetSnapshot();
    }
}
