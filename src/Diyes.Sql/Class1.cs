using System;
using System.Collections.Generic;
using Diyes.AppendOnlyStore.Interfaces;

namespace Diyes.Sql
{
    public class Class1 : IAppendOnlyStore
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Append(string name, string data, int expectedVersion = -1)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DataWithVersion> Read(string identity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DataWithVersion> ReadAfterVersion(string identity, int version)
        {
            throw new NotImplementedException();
        }
    }
}
