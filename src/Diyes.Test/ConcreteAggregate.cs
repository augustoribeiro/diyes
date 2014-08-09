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
            if (IsCreated)
                throw new ConcreteAggregateException("Aggregate has been previously created");
            Apply(new ConcreteAggregateCreated(Id));
        }

        public void When(ConcreteAggregateCreated e)
        {
            IsCreated = true;
        }
    }

    public class ConcreteAggregateException : Exception
    {
        public ConcreteAggregateException(string message):base(message)
        {
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