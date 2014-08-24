using System.Linq;
using Diyes.AppendOnlyStore.Interfaces;
using Raven.Client.Indexes;

namespace Diyes.RavenDbStore.Test
{
    public class RavenDbIndex : AbstractIndexCreationTask<DataWithVersion>
    {
        public RavenDbIndex()
        {
            Map = dataversions => from dataWithVersion in dataversions
                select new
                {
                    Version = dataWithVersion.Version,
                    Identity = dataWithVersion.Identity
                };

        }
    }
}