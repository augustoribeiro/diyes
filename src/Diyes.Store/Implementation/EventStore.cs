using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Diyes.AppendOnlyStore.Interfaces;
using Diyes.Store.Interfaces;

namespace Diyes.Store.Implementation
{
    public class EventStore : IEventStore
    {
        private readonly IAppendOnlyStore _appendOnlyStore;
        private BinaryFormatter _formatter = new BinaryFormatter();

        public EventStore(IAppendOnlyStore appendOnlyStore)
        {
            _appendOnlyStore = appendOnlyStore;
        }

        public EventStream LoadEventStream(IIdentity identity)
        {
            var id = IdentityToString(identity);
            var records = _appendOnlyStore.Read(id);
            var stream = new EventStream(identity);

            foreach (var record in records)
            {
                stream.Events.AddRange(DeserializeEvent(record.Data));
                stream.Version = record.Version;
            }

            return stream;
        }

        public EventStream LoadEventStreamAfterVersion(IIdentity identity, int version)
        {
            var id = IdentityToString(identity);
            var records = _appendOnlyStore.ReadAfterVersion(id,version);
            var stream = new EventStream(identity);

            foreach (var record in records)
            {
                stream.Events.AddRange(DeserializeEvent(record.Data));
                stream.Version = record.Version;
            }

            return stream;
        }

        public void AppendToStream(IIdentity id, int originalVersion, IEnumerable<IEvent> events)
        {
            if(!events.Any())
                return;

            var name = IdentityToString(id);
            var data = SerializeEvent(events.ToArray());

            try
            {
                _appendOnlyStore.Append(name,data,originalVersion);
            }
            catch (AppendOnlyConcurrencyException e)
            {
                var server = LoadEventStream(id);
                throw new OptimisticConcurrencyException(server.Version, e.ExpectedVersion, id, server.Events);
            }
        }


        private string IdentityToString(IIdentity identity)
        {
            return identity.Id.ToString();
        }

        private byte[] SerializeEvent(IEvent[] e)
        {
            using (var mem = new MemoryStream())
            {
                _formatter.Serialize(mem,e);
                return mem.ToArray();
            }
        }

        private IEvent[] DeserializeEvent(byte[] data)
        {
            using (var mem = new MemoryStream(data))
            {
                return (IEvent[]) _formatter.Deserialize(mem);
            }
        }


        
    }
}