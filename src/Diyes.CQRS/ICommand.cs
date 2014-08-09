using System;
using Diyes.Store.Implementation;
using Diyes.Store.Interfaces;

namespace Diyes.CQRS
{
    public interface ICommand<T>
        where T : AbstractAggregate
    {
        IIdentity AggregateId { get; }
        Action<T> Action { get; } 
    }
}
