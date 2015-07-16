namespace Cqrs.Domain.Core.Infrastructure
{
    public interface IQuery<in TCriteria, out TResult>
    {
        TResult Ask(TCriteria criteria);
    }
}
