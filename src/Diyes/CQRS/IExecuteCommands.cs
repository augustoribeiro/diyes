using Diyes.Store.Implementation;

namespace Diyes.CQRS
{
    public interface IExecuteCommands
    {
        void Execute<T>(ICommand<T> cmd) where T : AbstractAggregate;
    }
}