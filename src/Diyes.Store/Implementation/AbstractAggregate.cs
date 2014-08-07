using System.Collections.Generic;
using Diyes.Store.Interfaces;

namespace Diyes.Store.Implementation
{
    public abstract class AbstractAggregate 
    {
        public IIdentity Id { get; private set; }
        internal int Version { get; private set; }
        internal List<IEvent> Changes { get; private set; }

        protected AbstractAggregate(EventStream eventStream)
        {
            Id = eventStream.Id;
            Version = eventStream.Version;
            Changes = new List<IEvent>();

            foreach (var @event in eventStream.Events)
            {
                Mutate(@event);
            }
        }

        protected void Apply(IEvent e)
        {
            Changes.Add(e);
            Mutate(e);
        }

        private void Mutate(IEvent @event)
        {
            ((dynamic) this).When((dynamic) @event);
        }
    }
 
}