using System;
using AppendOnlyStore.Implementations;
using Diyes.Implementation;
using Diyes.Interfaces;
using NFluent;
using NUnit.Framework;

namespace Diyes.Test
{
    [TestFixture]
    public class InMemoryEventStoreTest
    {
        private IEventStore _store;

        [SetUp]
        public void Setup()
        {
            _store = new EventStore(new InMemoryAppendOnlyStore());
        }

        [Test]
        public void SaveOneEvent_EventHasNoAggregateId_ThrowsException()
        {
            var stream = _store.LoadEventStream(new Identity(Guid.NewGuid()));
            Check.That(stream.Version).IsEqualTo(0);
            Check.That(stream.Events.Count()).IsEqualTo(0);
        }

        
    }
}