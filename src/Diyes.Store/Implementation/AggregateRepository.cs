using System;
using System.Reflection;
using Diyes.Store.Interfaces;

namespace Diyes.Store.Implementation
{
    public class AggregateRepository 
    {
        private readonly EventStore _store;

        public AggregateRepository(EventStore store)
        {
            _store = store;
        }

        public virtual T Load<T>(IIdentity aggregateId) where T : AbstractAggregate
        {
            var eventStream = _store.LoadEventStream(aggregateId);
            var typeT = typeof(T);
            var ctor = typeT.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance,
                null, new[] {typeof (EventStream)}, null);

            if (ctor == null)
                throw new InvalidOperationException("this should never happen since we demand that T is subclass of AbstractAggregate");

            var instance = (T) ctor.Invoke(new object[] { eventStream });

            return instance;
        }

        public virtual void Save(AbstractAggregate abstractAggregate)
        {
            var aggregateId = abstractAggregate.Id;
            var version = abstractAggregate.Version;
            var changes = abstractAggregate.Changes;

            _store.AppendToStream(aggregateId,version,changes);
        }
    }

    public class AggregateRepositoryWithSnapshoting : AggregateRepository
    {
        private readonly EventStore _store;
        private readonly ISnapper _snapper;

        public AggregateRepositoryWithSnapshoting(EventStore store, ISnapper snapper) : base(store)
        {
            _store = store;
            _snapper = snapper;
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

        public override void Save(AbstractAggregate abstractAggregate)
        {
            base.Save(abstractAggregate);

            var type = typeof(AggregateRepository);
            var loadMethod = type.GetMethod("Load").MakeGenericMethod(abstractAggregate.GetType());

            var aggregateToSnap = (AbstractAggregate)loadMethod.Invoke(this, new object[] { abstractAggregate.Id });
            _snapper.SaveSnap(aggregateToSnap);
            
        }
    }
}