using System;
using System.Collections.Generic;

namespace Diyes.Store.Interfaces
{
    public class OptimisticConcurrencyException : Exception
    {
        public int Version { get; set; }
        public int ExpectedVersion { get; set; }
        public IIdentity Id { get; set; }
        public List<Event> Events { get; set; }

        public OptimisticConcurrencyException(int version, int expectedVersion, IIdentity id, List<Event> events)
        {
            Version = version;
            ExpectedVersion = expectedVersion;
            Id = id;
            Events = events;
        }
    }
}