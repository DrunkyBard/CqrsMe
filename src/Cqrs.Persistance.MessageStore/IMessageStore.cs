using System;

namespace Cqrs.Persistance.MessageStore
{
    public interface IMessageStore
    {
        DispatchedMessage TryGetDispatchedMessage(Guid id);

        void UpdateToDispatched(Guid id);

        void SaveNew(Guid id);
    }
}
