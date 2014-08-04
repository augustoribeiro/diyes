using System;

namespace Diyes.Store.Interfaces
{
    public interface IEvent
    {
        IIdentity AggregateId { get; }
    }
}


// event table
// aggregate id
// data
// version

//aggregate table
// aggregate id
// type
// version