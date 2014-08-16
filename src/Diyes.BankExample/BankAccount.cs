using System;
using Diyes.Store.Implementation;
using Diyes.Store.Interfaces;

namespace Diyes.BankExample
{
    public class BankAccount : AbstractAggregate
    {
        protected BankAccount(EventStream eventStream) : base(eventStream)
        {
        }

        public void Open()
        {
            if (IsOpen)
            {
                throw new BankAccountException("This account was already open");
            }

            Apply(new AccountOpened(Id));
        }

        public bool IsOpen { get; private set; }
    }

    public class AccountOpened : Event
    {
        public AccountOpened(IIdentity id)
        {
            AggregateId = id;
        }

        public IIdentity AggregateId { get; private set; }
    }

    public class BankAccountException : Exception
    {
        public BankAccountException(string thisAccountWasAlreadyOpen)
        {
            
        }
    }
}
