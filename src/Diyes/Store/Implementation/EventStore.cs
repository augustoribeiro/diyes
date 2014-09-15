using System.Collections.Generic;
using System.Linq;
using Diyes.AppendOnlyStore.Interfaces;
using Diyes.Store.Interfaces;
using Newtonsoft.Json;

namespace Diyes.Store.Implementation
{
    public class EventStore : IEventStore
    {
        private readonly IAppendOnlyStore _appendOnlyStore;
        private JsonSerializerSettings _settings;

        public EventStore(IAppendOnlyStore appendOnlyStore)
        {
            _appendOnlyStore = appendOnlyStore;

             _settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                Formatting = Formatting.Indented
            };
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

        public void AppendToStream(IIdentity id, int originalVersion, IEnumerable<Event> events)
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

        private string SerializeEvent(Event[] e)
        {
            return JsonConvert.SerializeObject(e, _settings);
        }

        private Event[] DeserializeEvent(string data)
        {
            return JsonConvert.DeserializeObject<Event[]>(data, _settings);
        }


        
    }
}