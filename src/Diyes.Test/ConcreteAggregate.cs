using System;
using Diyes.Store.Implementation;
using Diyes.Store.Interfaces;

namespace Diyes.Test
{
    public class ConcreteAggregate : AbstractAggregate
    {
        public int Number { get; private set; }
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

        public void ChangeNumber(int number)
        {
            if(!IsCreated)
                throw new ConcreteAggregateException("Aggregate has not been previously created");

            Apply(new NumberChanged(Id,number));
        }

        public void When(ConcreteAggregateCreated e)
        {
            IsCreated = true;
        }

        public void When(NumberChanged e)
        {
            Number = e.Number;
        }
    }

    public class ConcreteAggregateException : Exception
    {
        public ConcreteAggregateException(string message):base(message)
        {
        }
    }

    
    public class ConcreteAggregateCreated : IEvent
    {
        public ConcreteAggregateCreated(IIdentity id)
        {
            AggregateId = id;
        }

        public IIdentity AggregateId { get; private set; }
    }

    
    public class NumberChanged : IEvent
    {
        public NumberChanged(IIdentity id, int number)
        {
            AggregateId = id;
            Number = number;
        }

        public IIdentity AggregateId { get; private set; }
        public int Number { get; private set; }
    }
}