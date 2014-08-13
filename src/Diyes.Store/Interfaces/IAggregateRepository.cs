using Diyes.Store.Implementation;

namespace Diyes.Store.Interfaces
{
    public interface IAggregateRepository
    {
        T Load<T>(IIdentity aggregateId) where T : AbstractAggregate;
        void Save<T>(T abstractAggregate) where T : AbstractAggregate;
    }
}