using System;
using System.Runtime.Serialization;

namespace Diyes.Store.Interfaces
{
  
    public interface IIdentity 
    {
        Guid Id { get; }
    }
}