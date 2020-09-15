using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class WithdrawMoney
    {
        private IAccountRepository accountRepository;
        private INotificationService notificationService;

        public WithdrawMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            this.accountRepository = accountRepository;
            this.notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, decimal amount)
        {
            // TODO:
            try
            {
                var fromAccount = this.accountRepository.GetAccountById(fromAccountId);
                if (fromAccount == null)
                {
                    throw new NullReferenceException("Invalid account details.");
                }

                //Send Money
                fromAccount.SendMoney(amount);
                if (fromAccount.IsBalanceLow()) //Check Balance after Sending Money
                {
                    this.notificationService.NotifyFundsLow(fromAccount.User.Email);
                }

                this.accountRepository.Update(fromAccount);
            }
            catch (InvalidOperationException exInvalid)
            {
                throw exInvalid;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
