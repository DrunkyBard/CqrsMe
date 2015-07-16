namespace Cqrs.Domain.Core.Infrastructure
{
    public interface IActionFactory
    {
        IAction<TContext> Create<TContext>()
            where TContext : IActionContext;
    }
}
