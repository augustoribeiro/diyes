using Diyes.Store.Implementation;

namespace Diyes.Store.Interfaces
{
    public interface ISnapStore
    {
        AbstractAggregate Get(IIdentity identity);
        void Put(AbstractAggregate strippedAggregate);
    }
}