using System.Collections.Generic;
using Diyes.AppendOnlyStore.Interfaces;
using Raven.Client;

namespace Diyes.RavenDbStore.Test
{
    public class RavenDbStore : IAppendOnlyStore
    {
        private IDocumentStore _documentStore;

        public RavenDbStore(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public void Append(string name, string data, int expectedVersion = -1)
        {
            throw new System.NotImplementedException();
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
}