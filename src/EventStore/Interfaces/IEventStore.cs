using System.Collections.Generic;

namespace Diyes.Interfaces
{
    public interface IEventStore
    {
        EventStream LoadEventStream(IIdentity id);
        void AppendToStream(IIdentity id, int originalVersion, IEnumerable<IEvent> events);
    }
}