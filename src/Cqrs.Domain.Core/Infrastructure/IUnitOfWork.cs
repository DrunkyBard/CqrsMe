namespace Cqrs.Domain.Core.Infrastructure
{
    public interface IUnitOfWork
    {
        void Commit();
    }
}
