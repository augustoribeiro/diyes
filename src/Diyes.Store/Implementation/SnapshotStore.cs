using System.Collections.Generic;
using Diyes.Store.Interfaces;

namespace Diyes.Store.Implementation
{
    public class SnapshotStore : ISnapStore
    {
        private Dictionary<IIdentity, AbstractAggregate> _store = new Dictionary<IIdentity, AbstractAggregate>(); 

        public AbstractAggregate Get(IIdentity identity)
        {
            AbstractAggregate snap = null;
            _store.TryGetValue(identity, out snap);

            return snap;
        }

        public void Put(AbstractAggregate strippedAggregate)
        {
            _store.Add(strippedAggregate.Id,strippedAggregate);
        }
    }
}