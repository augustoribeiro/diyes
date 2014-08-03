using System;
using Diyes.Interfaces;

namespace Diyes.Implementation
{
    public class Identity : IIdentity
    {
        public Identity(Guid id)
        {
            if(id == Guid.Empty)
                throw new ArgumentException(string.Format("Cannot create an identity with {0}", Guid.Empty));

            Id = id;
        }

        public Guid Id { get; private set; }
    }
}