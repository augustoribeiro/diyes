using System;
using Diyes.AppendOnlyStore.Implementations;
using Diyes.Store.Implementation;
using NFluent;
using NUnit.Framework;

namespace Diyes.Test
{
    [TestFixture]
    public class ShapshotterTest
    {
        private EventStore _store;
        private AggregateRepository _aggregateRepository;
        private Snapper _snapper;

        [SetUp]
        public void Setup()
        {
            _store = new EventStore(new InMemoryAppendOnlyStore());
            _snapper = new Snapper(new SnapshotStore());
            _aggregateRepository = new AggregateRepositoryWithSnapshoting(_store,_snapper);
        }

        [Test]
        public void Test()
        {
            var identity = new Identity(Guid.NewGuid());
            _snapper.SnapshotFrequency = 1;

            var concreteAggregate = _aggregateRepository.Load<ConcreteAggregate>(identity);
            concreteAggregate.Create();

            _aggregateRepository.Save(concreteAggregate);

            var loadedSnapshot = _snapper.LoadSnap<ConcreteAggregate>(identity);

            Check.That(loadedSnapshot.IsCreated).IsTrue();
        }
    }
}
