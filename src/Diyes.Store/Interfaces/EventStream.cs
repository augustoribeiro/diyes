using System.Collections.Generic;

namespace Diyes.Store.Interfaces
{
    public class EventStream
    {
        public int Version;
        public List<IEvent> Events = new List<IEvent>();
    }
}