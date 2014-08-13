using Diyes.Store.Interfaces;

namespace Diyes.Store.Implementation
{
    public class AggregateRepositoryWithSnapshoting : AggregateRepository
    {
        private readonly EventStore _store;
        private readonly ISnapper _snapper;

        public AggregateRepositoryWithSnapshoting(EventStore store, ISnapper snapper) : base(store)
        {
            _store = store;
            _snapper = snapper;
        }

        public override void Save<T>(T abstractAggregate) 
        {
            base.Save(abstractAggregate);

            var aggregateToSnap = base.Load<T>(abstractAggregate.Id);

            _snapper.SaveSnap(aggregateToSnap);
        }

        public override T Load<T>(IIdentity aggregateId)
        {
            var aggregate = _snapper.LoadSnap<T>(aggregateId);

            if (aggregate != null)
            {
                var eventStream = _store.LoadEventStreamAfterVersion(aggregateId, aggregate.Version);
                aggregate.ReplayEvents(eventStream);
            }

            return base.Load<T>(aggregateId);
        }
    }
}