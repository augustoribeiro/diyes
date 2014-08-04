using System;

namespace Diyes.AppendOnlyStore.Interfaces
{
    public class AppendOnlyConcurrencyException : Exception
    {
        public int ExpectedVersion { get; private set; }

        public AppendOnlyConcurrencyException(int expectedVersion)
        {
            ExpectedVersion = expectedVersion;
        }
    }
}