using System.Collections.Generic;

namespace Diyes.Store.Interfaces
{
    public interface IEventStore
    {
        EventStream LoadEventStream(IIdentity id);
        void AppendToStream(IIdentity id, int originalVersion, IEnumerable<IEvent> events);
        EventStream LoadEventStreamAfterVersion(IIdentity identity, int version);
    }
}