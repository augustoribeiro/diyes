using System;
using System.Reflection;
using Diyes.Store.Interfaces;

namespace Diyes.Store.Implementation
{
    public class AggregateRepository : IAggregateRepository
    {
        private readonly EventStore _store;

        public AggregateRepository(EventStore store)
        {
            _store = store;
        }

        public virtual void Save<T>(T abstractAggregate) where T : AbstractAggregate
        {
            var aggregateId = abstractAggregate.Id;
            var version = abstractAggregate.Version;
            var changes = abstractAggregate.Changes;

            _store.AppendToStream(aggregateId, version, changes);
        }


        public virtual T Load<T>(IIdentity aggregateId) where T : AbstractAggregate
        {
            var eventStream = _store.LoadEventStream(aggregateId);
            var typeT = typeof(T);
            var ctor = GetAggregateConstructor<T>(typeT);

            var instance = (T) ctor.Invoke(new object[] { eventStream });

            return instance;
        }

        private static ConstructorInfo GetAggregateConstructor<T>(Type typeT) where T : AbstractAggregate
        {
            var ctor = typeT.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance,
                null, new[] {typeof (EventStream)}, null);

            if (ctor == null)
                throw new InvalidOperationException(
                    "this should never happen since we demand that T is subclass of AbstractAggregate");

            return ctor;
        }
    }
}