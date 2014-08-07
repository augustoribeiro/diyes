using System.Collections.Generic;

namespace Diyes.Store.Interfaces
{
    public class EventStream
    {
        public IIdentity Id { get; private set; }
        public int Version;
        public List<IEvent> Events = new List<IEvent>();

        public EventStream(IIdentity id)
        {
            Id = id;
        }
    }
}