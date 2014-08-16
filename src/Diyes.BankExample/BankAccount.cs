using System;
using Diyes.Store.Implementation;
using Diyes.Store.Interfaces;

namespace Diyes.BankExample
{
    public class Account : AbstractAggregate
    {
        protected Account(EventStream eventStream) : base(eventStream)
        {
        }

        public int Balance { get; private set; }
        public bool IsOpen { get; private set; }

        public void Open()
        {
            if (IsOpen)
            {
                throw new BankAccountException("This account was already open");
            }

            Apply(new AccountOpened());
        }

        public void When(AccountOpened e)
        {
            IsOpen = true;
            Balance = 0;
        }

        public void Deposit(int value)
        {
            IsOpenGuard();

            Apply(new DepositMade(value));
        }

        private void IsOpenGuard()
        {
            if (!IsOpen)
            {
                throw new BankAccountException("Trying to deposit money in an unexisting account");
            }
        }

        public void When(DepositMade e)
        {
            Balance = Balance + e.Value;
        }

        public void Withdraw(int value)
        {
            IsOpenGuard();

            if (value > Balance)
            {
                throw new BankAccountException("Trying to withdraw more money than what is available");
            }

            Apply(new WithdrawalMade(value));
        }

        public void When(WithdrawalMade e)
        {
            Balance = Balance - e.Value;
        }
    }

    public class WithdrawalMade : Event
    {
        public readonly int Value;

        public WithdrawalMade(int value)
        {
            Value = value;
        }
    }

    public class DepositMade : Event
    {
        public readonly int Value;

        public DepositMade(int value)
        {
            Value = value;
        }
    }

    public class AccountOpened : Event
    {
        
    }

    public class BankAccountException : Exception
    {
        public BankAccountException(string message) : base(message)
        {
            
        }
    }
}
