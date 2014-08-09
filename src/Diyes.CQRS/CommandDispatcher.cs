using Diyes.Store.Implementation;

namespace Diyes.CQRS
{
    public class CommandDispatcher 
    {
        private readonly IAggregateRepository _aggregateRepository;

        public CommandDispatcher(IAggregateRepository aggregateRepository)
        {
            _aggregateRepository = aggregateRepository;
        }

        public void Execute<T>(ICommand<T> cmd) where T : AbstractAggregate
        {
            var aggregate = _aggregateRepository.Load<T>(cmd.AggregateId);
            cmd.Action(aggregate);
            _aggregateRepository.Save(aggregate);
        }
    }
}