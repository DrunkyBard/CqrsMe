using Commands.Core;

namespace Cqrs.Domain.Core
{
    public interface ICommand<out TIdentity> : ICommand
    {
        TIdentity Identity { get; }
    }
}
