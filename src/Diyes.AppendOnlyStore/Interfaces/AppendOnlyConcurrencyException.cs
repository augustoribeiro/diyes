using System;
using System.Collections.Generic;

namespace Diyes.AppendOnlyStore.Interfaces
{
    public class AppendOnlyConcurrencyException : Exception
    {
        public int ExpectedVersion { get; private set; }

        public AppendOnlyConcurrencyException(int expectedVersion)
        {
            ExpectedVersion = expectedVersion;
        }

        public AppendOnlyConcurrencyException(int version, int expectedversion, string name)
        {
            
        }
    }
}