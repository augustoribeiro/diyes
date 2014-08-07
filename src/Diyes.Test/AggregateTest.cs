using System;
using Diyes.AppendOnlyStore.Implementations;
using Diyes.Store.Implementation;
using Diyes.Store.Interfaces;
using NFluent;
using NUnit.Framework;

namespace Diyes.Test
{
    [TestFixture]
    public class AggregateTest
    {
        private EventStore _store;
        private AggregateRepository _aggregateRepository;

        [SetUp]
        public void Setup()
        {
            _store = new EventStore(new InMemoryAppendOnlyStore());
            _aggregateRepository = new AggregateRepository(_store);
        }

        [Test]
        public void ConcreteAggregateLoad_AggregateHasNoCreatedEvent_IsCreatedIsFalse()
        {
            var identity = new Identity(Guid.NewGuid());

            var concreteAggregate = _aggregateRepository.Load<ConcreteAggregate>(identity);

            Check.That(concreteAggregate.IsCreated).IsFalse();
        }

        [Test]
        public void ConcreteAggregateLoad_AggregateCreate_IsCreatedIsTrue()
        {
            var identity = new Identity(Guid.NewGuid());
            var concreteAggregate = _aggregateRepository.Load<ConcreteAggregate>(identity);

            concreteAggregate.Create();

            Check.That(concreteAggregate.IsCreated).IsTrue();
        }


        [Test]
        public void CreateAggregate_SaveChangesAndReload_AggregateIsCreated()
        {
            var identity = new Identity(Guid.NewGuid());
            var concreteAggregate = _aggregateRepository.Load<ConcreteAggregate>(identity);

            concreteAggregate.Create();
            _aggregateRepository.Save(concreteAggregate);

            var reloadedConcreteAggregate = _aggregateRepository.Load<ConcreteAggregate>(identity);

            Check.That(reloadedConcreteAggregate.IsCreated).IsTrue();
        }

        [Test]
        public void CreateSameAggregateTwice_SaveChangesShouldFailOnTheSecond_ThrowsConcurencyException()
        {
            var identity = new Identity(Guid.NewGuid());
            var concreteAggregate = _aggregateRepository.Load<ConcreteAggregate>(identity);
            var anotherConcreteAggregate = _aggregateRepository.Load<ConcreteAggregate>(identity);

            concreteAggregate.Create();
            anotherConcreteAggregate.Create();

            _aggregateRepository.Save(concreteAggregate);

            Check.ThatCode(() => _aggregateRepository.Save(anotherConcreteAggregate)).Throws<EventStoreConcurrencyException>();
        }
    }
}
