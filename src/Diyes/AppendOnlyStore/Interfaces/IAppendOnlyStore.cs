using System;
using System.Collections.Generic;

namespace Diyes.AppendOnlyStore.Interfaces
{
    public interface IAppendOnlyStore : IDisposable
    {
        void Append(string name, string data, int expectedVersion = -1);
        IEnumerable<IDataWithVersion> Read(string identity);
        IEnumerable<IDataWithVersion> ReadAfterVersion(string identity, int version);
    }
}