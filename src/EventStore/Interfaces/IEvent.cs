using System;

namespace Diyes.Interfaces
{
    public interface IEvent
    {
        Guid AggregateId { get; }
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