using System;
using Diyes.AppendOnlyStore.Implementations;
using Diyes.CQRS;
using Diyes.Store.Implementation;
using NFluent;
using NUnit.Framework;

namespace Diyes.Test
{
    [TestFixture]
    public class CommandDispatcherTest
    {
        private CommandDispatcher _commandDispatcher;
        private AggregateRepository _aggregateRepository;

        [SetUp]
        public void Setup()
        {
            _aggregateRepository = new AggregateRepository(new EventStore(new InMemoryAppendOnlyStore()));
            _commandDispatcher =
                new CommandDispatcher(_aggregateRepository);
        }

        [Test]
        public void ExecuteCreatedCommand_OnConcreteAggregate_IsInFactCreated()
        {
            var aggregateId = new Identity(Guid.NewGuid());
            _commandDispatcher.Execute(new Command<ConcreteAggregate>(aggregateId, aggregate => aggregate.Create()));

            var reloadedAggregate = _aggregateRepository.Load<ConcreteAggregate>(aggregateId);
            
            Check.That(reloadedAggregate.IsCreated).IsTrue();
        }

        [Test]
        public void ExecuteTwoCommand_OnConcreteAggregate_CommandsAreIssued()
        {
            var aggregateId = new Identity(Guid.NewGuid());
            _commandDispatcher.Execute(new Command<ConcreteAggregate>(aggregateId, aggregate => aggregate.Create()));

            const int number = 42;
            _commandDispatcher.Execute(new Command<ConcreteAggregate>(aggregateId, aggregate => aggregate.ChangeNumber(number)));

            var reloadedAggregate = _aggregateRepository.Load<ConcreteAggregate>(aggregateId);

            Check.That(reloadedAggregate.IsCreated).IsTrue();
            Check.That(reloadedAggregate.Number).IsEqualTo(number);
        }
    }
}
