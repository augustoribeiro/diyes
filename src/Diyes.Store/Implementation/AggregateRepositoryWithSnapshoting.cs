using System;
using Diyes.Store.Interfaces;

namespace Diyes.Store.Implementation
{
    public class AggregateRepositoryWithSnapshoting : IAggregateRepository
    {
        private readonly IAggregateRepository _aggregateRepository;
        private readonly ISnapper _snapper;

        public AggregateRepositoryWithSnapshoting(IAggregateRepository aggregateRepository, ISnapper snapper)
        {
            if (aggregateRepository == null) throw new ArgumentNullException("aggregateRepository");

            _aggregateRepository = aggregateRepository;
            Store = _aggregateRepository.Store;
            _snapper = snapper;
        }

        public void Save<T>(T abstractAggregate) where T : AbstractAggregate
        {
            _aggregateRepository.Save(abstractAggregate);

            var aggregateToSnap = _aggregateRepository.Load<T>(abstractAggregate.Id);

            _snapper.SaveSnap(aggregateToSnap);
        }

        public EventStore Store { get; private set; }

        public T Load<T>(IIdentity aggregateId) where T : AbstractAggregate
        {
            var aggregate = _snapper.LoadSnap<T>(aggregateId);

            if (aggregate != null)
            {
                var eventStream = Store.LoadEventStreamAfterVersion(aggregateId, aggregate.Version);
                aggregate.ReplayEvents(eventStream);
            }

            return _aggregateRepository.Load<T>(aggregateId);
        }
    }
}