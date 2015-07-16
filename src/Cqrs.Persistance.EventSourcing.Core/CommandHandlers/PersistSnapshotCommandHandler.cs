using Commands.Core;
using Cqrs.Domain.EventSourcing;
using Cqrs.Persistance.EventSourcing.Core.Commands;

namespace Cqrs.Persistance.EventSourcing.Core.CommandHandlers
{
    internal sealed class PersistSnapshotCommandHandler<TSnapshot> : ICommandHandler<PersistSnapshotCommand<TSnapshot>> 
        where TSnapshot : Snapshot
    {
        private readonly ISnapshotDataContext _snapshotDataContext;
        private readonly int _persistSnapshotPeriod;

        public PersistSnapshotCommandHandler(ISnapshotDataContext snapshotDataContext)
        {
            _snapshotDataContext = snapshotDataContext;
            _persistSnapshotPeriod = 500; //TODO: Period
        }

        public void Execute(PersistSnapshotCommand<TSnapshot> command)
        {
            if (command.ProducedEventCount < _persistSnapshotPeriod)
            {
                return;
            }

            var snapshotMetadata = new SnapshotMetadata(command.PreviouslyDispatchedCommandIds);
            var snapshotEnvelope = new SnapshotEnvelope<TSnapshot>(command.Snapshot, snapshotMetadata);
            _snapshotDataContext.Persist(snapshotEnvelope);
        }
    }
}
