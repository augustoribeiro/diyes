using System;
using Diyes.Store.Interfaces;

namespace Diyes.Store.Implementation
{
    [Serializable]
    public class Identity : IIdentity
    {
        public Guid Id { get; private set; }

        public Identity(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(string.Format("Cannot create an identity with {0}", Guid.Empty));

            Id = id;
        }

        protected bool Equals(Identity other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Identity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

      
    }
}