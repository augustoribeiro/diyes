using System;
using System.Collections.Generic;
using System.Linq;
using Diyes.AppendOnlyStore.Interfaces;
using Raven.Abstractions.Exceptions;
using Raven.Client;

namespace Diyes.RavenDbStore
{
    public class RavenDbStore : IAppendOnlyStore
    {
        private readonly IDocumentStore _documentStore;

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
                var @event = new RavenEvent(identity, data, version);
                ravenData.Events.Add(@event);
                
                try
                {
                    session.SaveChanges();
                }
                catch (ConcurrencyException exception)
                {
                    throw new AppendOnlyConcurrencyException(version, expectedVersion, identity);
                }
            }
        }

        static int GetVersion(int expectedVersion, RavenData ravenData)
        {
            var version = 0;
            if (ravenData.Events.Any())
            {
                version = ravenData.Events.Max(x => x.Version);
            }

            if (version != expectedVersion)
            {
                throw new AppendOnlyConcurrencyException(expectedVersion);
            }
            return version + 1;
        }

        static RavenData GetOrCreateRavenData(string identity, IDocumentSession session)
        {
            var ravenData = session.Load<RavenData>(identity);

            if (ravenData == null)
            {
                ravenData = new RavenData(identity);
                session.Store(ravenData);
            }
            return ravenData;
        }

        public IEnumerable<IDataWithVersion> Read(string identity)
        {
            IEnumerable<IDataWithVersion> events;
            using (var session = _documentStore.OpenSession())
            {
                var ravenData = session.Load<RavenData>(identity);
                if (ravenData == null)
                {
                    return new List<RavenEvent>();
                }

                events = ravenData.Events.Select(x => new RavenEvent(x.Identity, x.Data, x.Version));
            }
            return events;
        }

        public IEnumerable<IDataWithVersion> ReadAfterVersion(string identity, int version)
        {
            IEnumerable<IDataWithVersion> events;
            using (var session = _documentStore.OpenSession())
            {
                var ravenData = session.Load<RavenData>(identity);
                if (ravenData != null)
                {
                    events = ravenData.Events.Where(x => x.Version > version)
                    .OrderBy(x => x.Version)
                    .Select(x => new RavenEvent(x.Identity, x.Data, x.Version));
                    
                }
                else
                {
                    events = new List<RavenEvent>();
                }
            }
            return events;
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

    public class RavenEvent : IDataWithVersion
    {
        public string Data { get; protected set; }
        public string Identity { get; private set; }
        public int Version { get; protected set; }

        public RavenEvent(string identity, string data, int  version)
        {
            Identity = identity;
            Version = version;
            Data = data;
        }
    }
}