﻿using System;
using System.Collections.Generic;

namespace AppendOnlyStore.Interfaces
{
    public interface IAppendOnlyStore : IDisposable
    {
        void Append(string name, byte[] data, int expectedVersion = -1);
        IEnumerable<DataWithVersion> Read(string identity);
    }
}