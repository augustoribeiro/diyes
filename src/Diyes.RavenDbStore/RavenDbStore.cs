using System.Collections.Generic;
using System.Linq;
using Diyes.AppendOnlyStore.Interfaces;
using Raven.Client;

namespace Diyes.RavenDbStore
{
    public class RavenDbStore : IAppendOnlyStore
    {
        private IDocumentStore _documentStore;

        public RavenDbStore(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public void Append(string identity, string data, int expectedVersion = -1)
        {
            using (var session = _documentStore.OpenSession())
            {
                var ravenData = GetOrCreateRavenData(identity, session);
                var version = GetVersion(expectedVersion, ravenData);

                var @event = new RavenEvent(data, version);
                ravenData.Events.Add(@event);
                session.SaveChanges();
            }
        }

        private static int GetVersion(int expectedVersion, RavenData ravenData)
        {
            var version = 0;
            if (ravenData.Events.Any())
            {
                version = ravenData.Events.Max(x => x.Version) + 1;
            }

            if (version != expectedVersion)
            {
                throw new AppendOnlyConcurrencyException(expectedVersion);
            }
            return version;
        }

        private static RavenData GetOrCreateRavenData(string identity, IDocumentSession session)
        {
            var ravenData = session.Load<RavenData>(identity);

            if (ravenData == null)
            {
                ravenData = new RavenData(identity);
                session.Store(ravenData);
            }
            return ravenData;
        }

        public IEnumerable<DataWithVersion> Read(string identity)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<DataWithVersion> ReadAfterVersion(string identity, int version)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }

    public class RavenData
    {
        public string Id { get; protected set; }
        public List<RavenEvent> Events { get; set; }

        public RavenData(string id)
        {
            Id = id;
            Events = new List<RavenEvent>();
        }
    }

    public class RavenEvent
    {
        public string Data { get; protected set; }
        public int Version { get; protected set; }

        public RavenEvent(string data, int  version)
        {
            Version = version;
            Data = data;
        }
    }
}