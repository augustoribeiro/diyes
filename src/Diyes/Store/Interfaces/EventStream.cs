using System.Collections.Generic;

namespace Diyes.Store.Interfaces
{
    public class EventStream
    {
        public IIdentity Id { get; private set; }
        public int Version;
        public List<Event> Events = new List<Event>();

        public EventStream(IIdentity id)
        {
            Id = id;
        }
    }
}