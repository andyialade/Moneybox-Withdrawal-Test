using System;

namespace Moneybox.App
{
    public class Account
    {
        #region private
        private const decimal PayInLimit = 4000m;
        private const decimal MinimunBalace = 500m;
        #endregion

        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Balance { get; set; }

        public decimal Withdrawn { get; set; }

        public decimal PaidIn { get;  set; }

        public void ReceiveMoney(decimal amount)
        {
            //Account Receiving Payment
            if (PaidIn + amount > Account.PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }
            PaidIn = PaidIn + amount;
            Balance = Balance + amount;
        }

        public void SendMoney(decimal amount)
        {
            //Account Sending Payment
            if (Balance - amount < 0m)
            {
                throw new InvalidOperationException("Insufficient funds to make transfer");
            }
            Balance = Balance - amount;
            Withdrawn = Withdrawn - amount;
        }

        public bool IsBalanceLow()
        {
            return Balance < MinimunBalace ? true : false;
        }

        public bool IsPayInLimitClose()
        {
            return PayInLimit - PaidIn < MinimunBalace ? true : false;
        }
    }
}
