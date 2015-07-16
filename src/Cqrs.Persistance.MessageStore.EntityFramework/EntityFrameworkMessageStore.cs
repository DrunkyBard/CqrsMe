using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Cqrs.Persistance.MessageStore.EntityFramework
{
    internal sealed class EntityFrameworkMessageStore : IMessageStore
    {
        private readonly Func<DbContext> _dbContextFactory;

        public EntityFrameworkMessageStore(Func<DbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public DispatchedMessage TryGetDispatchedMessage(Guid id)
        {
            using (var dbContext = _dbContextFactory())
            {
                var dispatchedMessage = dbContext.Set<DispatchedMessageEntity>()
                    .SingleOrDefault(x => x.Id == id);

                if (dispatchedMessage == null)
                {
                    return null;
                }

                var payload =
                    JsonConvert.DeserializeObject<IReadOnlyCollection<object>>(
                        Encoding.UTF8.GetString(dispatchedMessage.Payload));
                
                return new DispatchedMessage(dispatchedMessage.Id, dispatchedMessage.IsDispatched, payload);
            }
        }

        public async void UpdateToDispatched(Guid id)
        {
            using (var dbContext = _dbContextFactory())
            {
                var updatedDispatchedMessage = Create(id, true, null);
                dbContext.Set<DispatchedMessageEntity>()
                    .Attach(updatedDispatchedMessage);
                dbContext.Entry(updatedDispatchedMessage).State = EntityState.Modified;

                await dbContext.SaveChangesAsync();
            }
        }

        public async void SaveNew(Guid id)
        {
            using (var dbContext = _dbContextFactory())
            {
                var newDispatchedMessage = Create(id, false, null);
                dbContext.Set<DispatchedMessageEntity>()
                    .Add(newDispatchedMessage);

                await dbContext.SaveChangesAsync();
            }
        }

        private DispatchedMessageEntity Create(Guid id, bool isDispatched, IReadOnlyCollection<object> payload)
        {
            var bytePayload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload));

            return new DispatchedMessageEntity
            {
                Id = id,
                IsDispatched = isDispatched,
                Payload = bytePayload
            };
        }
    }
}
