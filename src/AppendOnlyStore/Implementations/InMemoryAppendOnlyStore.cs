using System.Collections.Generic;
using System.Linq;
using AppendOnlyStore.Interfaces;

namespace AppendOnlyStore.Implementations
{
    public class InMemoryAppendOnlyStore : IAppendOnlyStore
    {
        private List<DataWithVersion> events = new List<DataWithVersion>();
        private object _lock = new object();

        public void Append(string name, byte[] data, int expectedVersion = -1)
        {
            lock (_lock)
            {
                var version = events.Where(e => e.Identity == name).Max(e => e.Version);
                if (version != expectedVersion)
                {
                    throw new AppendOnlyConcurrencyException(expectedVersion);
                }

                events.Add(new DataWithVersion(name,version,data));
            }
        }

        public IEnumerable<DataWithVersion> Read(string identity)
        {
            return events.Where(e => e.Identity == identity).OrderBy(e => e.Version);
        }

        public void Dispose()
        {
            _lock = null;
            events = null;
        }

    }
}