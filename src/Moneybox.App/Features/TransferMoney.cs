using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class TransferMoney
    {
        private IAccountRepository accountRepository;
        private INotificationService notificationService;

        public TransferMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            this.accountRepository = accountRepository;
            this.notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            #region originalCode 
            //var from = this.accountRepository.GetAccountById(fromAccountId);
            //var to = this.accountRepository.GetAccountById(toAccountId);

            ////var fromBalance = from.Balance - amount;
            ////if (fromBalance < 0m)
            ////{
            ////    throw new InvalidOperationException("Insufficient funds to make transfer");
            ////}

            ////if (fromBalance < 500m)
            ////{
            ////    this.notificationService.NotifyFundsLow(from.User.Email);
            ////}

            ////var paidIn = to.PaidIn + amount;
            ////if (paidIn > Account.PayInLimit)
            ////{
            ////    throw new InvalidOperationException("Account pay in limit reached");
            ////}

            ////if (Account.PayInLimit - paidIn < 500m)
            ////{
            ////    this.notificationService.NotifyApproachingPayInLimit(to.User.Email);
            ////}

            ////from.Balance = from.Balance - amount;
            ////from.Withdrawn = from.Withdrawn - amount;

            ////to.Balance = to.Balance + amount;
            ////to.PaidIn = to.PaidIn + amount;

            //this.accountRepository.Update(from);
            //this.accountRepository.Update(to);
            #endregion

            #region refactoredCode
            try
            {
                var fromAccount = this.accountRepository.GetAccountById(fromAccountId);
                var toAccount = this.accountRepository.GetAccountById(toAccountId);

                if (fromAccount == null || toAccount == null)
                {
                    var errorMessage = (fromAccount == null) ? "Invalid account details for fromAccountId" : "Invalid account details for toAccountId";
                    throw new NullReferenceException(errorMessage);
                }

                //Send Money
                fromAccount.SendMoney(amount);
                if(fromAccount.IsBalanceLow()) //Check Balance after Sending Money
                {
                    this.notificationService.NotifyFundsLow(fromAccount.User.Email);
                }

                //Receive Money
                toAccount.ReceiveMoney(amount);
                if (toAccount.IsPayInLimitClose())
                {
                    this.notificationService.NotifyApproachingPayInLimit(toAccount.User.Email);
                }

                this.accountRepository.Update(fromAccount);
                this.accountRepository.Update(toAccount);
            }
            catch(InvalidOperationException exInvalid)
            {
                throw exInvalid;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #endregion
        }
    }
}
