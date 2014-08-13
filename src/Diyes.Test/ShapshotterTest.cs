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
        private AggregateRepositoryWithSnapshoting _aggregateRepository;
        private Snapper _snapper;

        [SetUp]
        public void Setup()
        {
            _store = new EventStore(new InMemoryAppendOnlyStore());
            _snapper = new Snapper(new SnapshotStore());
            _aggregateRepository = new AggregateRepositoryWithSnapshoting(_store,_snapper);
        }

        [Test]
        public void SnapshotFrequencyIsOne_SavesSnapAfterFirstSave()
        {
            var identity = new Identity(Guid.NewGuid());
            _snapper.SnapshotFrequency = 1;

            var concreteAggregate = _aggregateRepository.Load<ConcreteAggregate>(identity);
            concreteAggregate.Create();

            _aggregateRepository.Save(concreteAggregate);

            var loadedSnapshot = _snapper.LoadSnap<ConcreteAggregate>(identity);

            Check.That(loadedSnapshot).IsNotNull();
            Check.That(loadedSnapshot.IsCreated).IsTrue();
        }

        [Test]
        public void SnapshotFrequencyIsTwo_SavesSnapAfterSecondSave()
        {
            const int number = 42;
            var identity = new Identity(Guid.NewGuid());
            _snapper.SnapshotFrequency = 2;

            var concreteAggregate = _aggregateRepository.Load<ConcreteAggregate>(identity);
            concreteAggregate.Create();

            _aggregateRepository.Save(concreteAggregate);

            var loadedSnapshot = _snapper.LoadSnap<ConcreteAggregate>(identity);
            Check.That(loadedSnapshot).IsNull();

            concreteAggregate = _aggregateRepository.Load<ConcreteAggregate>(identity);
            concreteAggregate.ChangeNumber(number);

            _aggregateRepository.Save(concreteAggregate);


            loadedSnapshot = _snapper.LoadSnap<ConcreteAggregate>(identity);
            Check.That(loadedSnapshot).IsNotNull();
            Check.That(loadedSnapshot.IsCreated).IsTrue();
            Check.That(loadedSnapshot.Number).IsEqualTo(number);
        }

        [Test]
        public void SnapshotFrequencyIsTwo_SnapshotIsNotCurrent_AggregateLoaderLoadsLastVersion()
        {
            const int numberOne = 1;
            const int numberTwo = 2;
            const int numberThree = 3;

            var identity = new Identity(Guid.NewGuid());
            _snapper.SnapshotFrequency = 2;

            var concreteAggregate = _aggregateRepository.Load<ConcreteAggregate>(identity);
            concreteAggregate.Create();

            _aggregateRepository.Save(concreteAggregate);

            concreteAggregate = _aggregateRepository.Load<ConcreteAggregate>(identity);
            concreteAggregate.ChangeNumber(numberOne);

            _aggregateRepository.Save(concreteAggregate);

            concreteAggregate = _aggregateRepository.Load<ConcreteAggregate>(identity);
            concreteAggregate.ChangeNumber(numberTwo);

            _aggregateRepository.Save(concreteAggregate);

            concreteAggregate = _aggregateRepository.Load<ConcreteAggregate>(identity);
            concreteAggregate.ChangeNumber(numberThree);

            _aggregateRepository.Save(concreteAggregate);


            concreteAggregate = _aggregateRepository.Load<ConcreteAggregate>(identity);

            Check.That(concreteAggregate.Number).IsEqualTo(numberThree);
        }
    }
}
