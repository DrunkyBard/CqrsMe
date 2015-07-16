namespace Cqrs.Domain.Core.Infrastructure
{
    public interface IAction<in TActionContext> where TActionContext : IActionContext
    {
        void Do(TActionContext actionContext);
    }
}
