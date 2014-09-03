using System;
using System.Linq;
using Diyes.Store.Implementation;
using Diyes.Store.Interfaces;
using NFluent;
using NUnit.Framework;

namespace Diyes.RavenDbStore.Test
{
    [TestFixture]
    public class RavenDbEventStoreTest : RavenFixture
    {
        private IEventStore _sut;

        public override void DoSetup()
        {
            _sut = new EventStore(new RavenDbStore(DocumentStore));
        }

        [Test]
        public void LoadEventStream_AggregateHasNoEvents_ReturnsNoEvent()
        {
            var stream = _sut.LoadEventStream(new Identity(Guid.NewGuid()));
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

            Check.ThatCode(() => _sut.AppendToStream(identity, originalVersion, new[] { concreteEvent })).Throws<OptimisticConcurrencyException>();
        }

        [Test]
        public void LoadEventStream_AfterSavingOneEvent_ReturnsThatEvent()
        {
            var random = new Random();
            var identity = new Identity(Guid.NewGuid());
            var concreteEvent = new TestEvent(random.Next(0, 1000));
            var originalVersion = 0;

            _sut.AppendToStream(identity, originalVersion, new[] { concreteEvent });

            var stream = _sut.LoadEventStream(identity);

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
            var eventStream = _sut.LoadEventStream(identity);

            //emiting test event 1
            _sut.AppendToStream(identity, eventStream.Version, new[] { testEvent1 });

            //reload event stream
            eventStream = _sut.LoadEventStream(identity);

            //emiting test event 2
            _sut.AppendToStream(identity, eventStream.Version, new[] { testEvent2 });

            //reload event stream
            eventStream = _sut.LoadEventStream(identity);

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
            var eventStreamSession1 = _sut.LoadEventStream(identity);
            var eventStreamSession2 = _sut.LoadEventStream(identity);

            //emiting test event 1 in session 1
            _sut.AppendToStream(identity, eventStreamSession1.Version, new[] { testEvent1 });

            //emiting test event 1 in session 2 should fail
            Check.ThatCode(() => _sut.AppendToStream(identity, eventStreamSession2.Version, new[] { testEvent2 }))
                .Throws<OptimisticConcurrencyException>();
        }
    }

    public class TestEvent : Event
    {
        public int Number { get; private set; }

        public TestEvent(int number)
        {
            Number = number;
        }

        protected bool Equals(TestEvent other)
        {
            return Equals(AggregateId, other.AggregateId) && Number == other.Number;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TestEvent)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((AggregateId != null ? AggregateId.GetHashCode() : 0) * 397) ^ Number;
            }
        }
    }
}