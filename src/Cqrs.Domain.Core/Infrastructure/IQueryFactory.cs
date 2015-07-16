namespace Cqrs.Domain.Core.Infrastructure
{
    public interface IQueryFactory
    {
        IQuery<TCriteria, TResult> Create<TCriteria, TResult>() 
            where TCriteria : IQueryCriteria
            where TResult : IEntity;
    }
}
