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

        public T Load<T>(IIdentity aggregateId) where T : AbstractAggregate
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

        public void Save(AbstractAggregate abstractAggregate)
        {
            var aggregateId = abstractAggregate.Id;
            var version = abstractAggregate.Version;
            var changes = abstractAggregate.Changes;

            _store.AppendToStream(aggregateId,version,changes);
           
        }
    }
}