using System.Collections.Generic;
using System.Linq;
using Diyes.Store.Interfaces;

namespace Diyes.Store.Implementation
{
    public abstract class AbstractAggregate 
    {
        public IIdentity Id { get; private set; }
        internal int Version { get; private set; }
        internal List<Event> Changes { get; private set; }


        protected AbstractAggregate(EventStream eventStream)
        {
            Id = eventStream.Id;
            Version = eventStream.Version;
            Changes = new List<Event>();

            foreach (var @event in eventStream.Events)
            {
                Mutate(@event);
            }
        }

        protected void Apply(Event @event)
        {
            @event.AggregateId = Id;
            Changes.Add(@event);
            Mutate(@event);
        }

        private void Mutate(Event @event)
        {
            ((dynamic) this).When((dynamic) @event);
        }

        internal void ReplayEvents(EventStream eventStream)
        {
            if (eventStream.Events.Any())
            {
                Version = eventStream.Version;

                foreach (var @event in eventStream.Events)
                {
                    Mutate(@event);
                }
            }
        }
    }
 
}