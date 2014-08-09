using System;
using Diyes.CQRS;
using Diyes.Store.Implementation;
using Diyes.Store.Interfaces;

namespace Diyes.Test
{
    public class Command<T> : ICommand<T> where T : AbstractAggregate
    {
        public Command(Identity aggregateId, Action<T> action)
        {
            AggregateId = aggregateId;
            Action = action;
        }

        public IIdentity AggregateId { get; private set; }
        public Action<T> Action { get; private set; }
    }
}