using System.Collections.Generic;
using System.Linq;
using Diyes.AppendOnlyStore.Interfaces;

namespace Diyes.AppendOnlyStore.Implementations
{
    public class InMemoryAppendOnlyStore : IAppendOnlyStore
    {
        private List<DataWithVersion> events = new List<DataWithVersion>();
        private object _lock = new object();

        public void Append(string name, string data, int expectedVersion = -1)
        {
            lock (_lock)
            {
                var version = 0;
                var dataWithVersions = events.Where(e => e.Identity == name);
                
                if (dataWithVersions.Any())
                {
                    version = dataWithVersions.Max(e => e.Version);    
                }
                
                if (version != expectedVersion)
                {
                    throw new AppendOnlyConcurrencyException(expectedVersion);
                }

                events.Add(new DataWithVersion(name,version + 1,data));
            }
        }

        public IEnumerable<IDataWithVersion> Read(string identity)
        {
            return events.Where(e => e.Identity == identity).OrderBy(e => e.Version);
        }

        public IEnumerable<IDataWithVersion> ReadAfterVersion(string identity, int version)
        {
            return events.Where(e => e.Identity == identity).OrderBy(e => e.Version > version);
        }

        public void Dispose()
        {
            _lock = null;
            events = null;
        }

    }
}