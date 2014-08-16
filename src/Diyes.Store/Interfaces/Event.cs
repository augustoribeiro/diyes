using System;

namespace Diyes.Store.Interfaces
{
    public abstract class Event
    {
        public IIdentity AggregateId { get; internal set; }
    }
}