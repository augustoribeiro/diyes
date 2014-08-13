using System;
using Diyes.Store.Interfaces;

namespace Diyes.Test
{
    public class TestEvent : IEvent
    {
        public IIdentity AggregateId { get; private set; }
        public int Number { get; private set; }

        public TestEvent(IIdentity aggregateId, int number)
        {
            AggregateId = aggregateId;
            Number = number;
        }

        protected bool Equals(TestEvent other)
        {
            return Equals(AggregateId, other.AggregateId) && Number == other.Number;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TestEvent) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((AggregateId != null ? AggregateId.GetHashCode() : 0)*397) ^ Number;
            }
        }
    }
}