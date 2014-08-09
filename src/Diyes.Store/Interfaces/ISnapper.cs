using Diyes.Store.Implementation;

namespace Diyes.Store.Interfaces
{
    public interface ISnapper
    {
        void SaveSnap(AbstractAggregate aggregate);
        T LoadSnap<T>(IIdentity identity) where T : AbstractAggregate;
        bool ShouldSnap(int version);
    }
}