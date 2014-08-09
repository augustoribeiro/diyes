using System;
using System.Collections.Generic;
using System.Reflection;
using Diyes.Store.Interfaces;

namespace Diyes.Store.Implementation
{
    public class Snapper : ISnapper
    {
        private readonly ISnapStore _snapshotStore;
        private int _snapshotFrequency = 10;


        public Snapper(ISnapStore snapshotStore)
        {
            _snapshotStore = snapshotStore;
        }

        public int SnapshotFrequency
        {
            get { return _snapshotFrequency; }
            set { _snapshotFrequency = value; }
        }

        public void SaveSnap(AbstractAggregate aggregate)
        {
            if (ShouldSnap(aggregate.Version))
            {

                _snapshotStore.Put(aggregate);
            }
        }

        public T LoadSnap<T>(IIdentity identity) where T : AbstractAggregate
        {
            var snap = _snapshotStore.Get(identity);

            if (snap == null)
                return null;

            return (T) snap;
        }

        public bool ShouldSnap(int version)
        {
            return version%SnapshotFrequency == 0;
        }

    }
}