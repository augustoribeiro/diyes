using System;
using Diyes.Store.Implementation;
using Diyes.Store.Interfaces;

namespace Diyes.Test
{
    public class ConcreteAggregate : AbstractAggregate
    {
        public bool IsCreated { get; protected set; }

        protected ConcreteAggregate(EventStream eventStream) : base(eventStream)
        {

        }

        public void Create()
        {
            Apply(new ConcreteAggregateCreated(Id));
        }

        public void When(ConcreteAggregateCreated e)
        {
            IsCreated = true;
        }
    }

    [Serializable]
    public class ConcreteAggregateCreated : IEvent
    {
        public ConcreteAggregateCreated(IIdentity id)
        {
            AggregateId = id;
        }

        public IIdentity AggregateId { get; private set; }
    }
}