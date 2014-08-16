using System;
using System.Linq;
using Diyes.AppendOnlyStore.Implementations;
using Diyes.Store.Implementation;
using Diyes.Store.Interfaces;
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
        public void LoadEventStream_AggregateHasNoEvents_ReturnsNoEvents()
        {
            var stream = _store.LoadEventStream(new Identity(Guid.NewGuid()));
            Check.That(stream.Version).IsEqualTo(0);
            Check.That(EnumerableExtensions.Count(stream.Events)).IsEqualTo(0);
        }

        [Test]
        public void SavingEventStream_WithWrongInitialVerion_ThrowsConcurrencyException()
        {
            var random = new Random();
            var identity = new Identity(Guid.NewGuid());
            var concreteEvent = new TestEvent(random.Next(0, 1000));
            var originalVersion = 1;

            Check.ThatCode(() => _store.AppendToStream(identity, originalVersion, new[] { concreteEvent })).Throws<OptimisticConcurrencyException>();
        }

        [Test]
        public void LoadEventStream_AfterSavingOneEvent_ReturnsThatEvent()
        {
            var random = new Random();
            var identity = new Identity(Guid.NewGuid());
            var concreteEvent = new TestEvent(random.Next(0, 1000));
            var originalVersion = 0;

            _store.AppendToStream(identity, originalVersion, new[] { concreteEvent });

            var stream = _store.LoadEventStream(identity);

            Check.That(stream.Events.Count).IsEqualTo(1);
            Check.That(stream.Version).IsEqualTo(originalVersion + 1);

            var e = stream.Events.First();
            Check.That(e).IsEqualTo(concreteEvent);
        }

        [Test]
        public void SimulateAggregateOperations_LoadingAggregate_SavingEvents_Reloading()
        {
            //Arrange
            var random = new Random();
            var identity = new Identity(Guid.NewGuid());
            var testEvent1 = new TestEvent(random.Next(0, 1000));
            var testEvent2 = new TestEvent(random.Next(0, 1000));


            //Act
            //initial load
            var eventStream = _store.LoadEventStream(identity);

            //emiting test event 1
            _store.AppendToStream(identity, eventStream.Version, new[] { testEvent1 });

            //reload event stream
            eventStream = _store.LoadEventStream(identity);

            //emiting test event 2
            _store.AppendToStream(identity, eventStream.Version, new[] { testEvent2 });

            //reload event stream
            eventStream = _store.LoadEventStream(identity);


            //Assert
            Check.That(eventStream.Events.Count).IsEqualTo(2);
            Check.That(eventStream.Version).IsEqualTo(2);

            var events = eventStream.Events.ToArray();

            Check.That(events[0]).IsEqualTo(testEvent1);
            Check.That(events[1]).IsEqualTo(testEvent2);
        }


        [Test]
        public void SimulateAggregateOperations_ForceConcurrencyException()
        {
            //Arrange
            var random = new Random();
            var identity = new Identity(Guid.NewGuid());
            var testEvent1 = new TestEvent(random.Next(0, 1000));
            var testEvent2 = new TestEvent(random.Next(0, 1000));


            //Act
            //initial load
            var eventStreamSession1 = _store.LoadEventStream(identity);
            var eventStreamSession2 = _store.LoadEventStream(identity);

            //emiting test event 1 in session 1
            _store.AppendToStream(identity, eventStreamSession1.Version, new[] { testEvent1 });

            //emiting test event 1 in session 2 should fail
            Check.ThatCode(() => _store.AppendToStream(identity, eventStreamSession2.Version, new[] {testEvent2})).Throws<OptimisticConcurrencyException>();
        }
    }
}