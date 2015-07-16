using Commands.Core;

namespace Cqrs.Messaging.Core
{
    public interface ICommandBus
    {
        void Dispatch<TCommand>(TCommand command) where TCommand : ICommand;
    }
}
