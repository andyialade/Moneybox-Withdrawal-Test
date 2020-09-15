using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;
using System;
using Xunit;

namespace Moneybox.App.Tests
{
    public class WithdrawMoneyShould
    {
        [Fact]
        public void Return_Exception_When_Amount_Account_Is_Null()
        {
            //Arrange
            var mockAccountRepo = new Mock<IAccountRepository>();
            mockAccountRepo.Setup(x => x.GetAccountById(It.IsAny<Guid>())).Returns((Account)null);

            var sut = new WithdrawMoney(mockAccountRepo.Object, null);

            //Act
            Action act = () => sut.Execute(It.IsAny<Guid>(), It.IsAny<decimal>());

            //Assert
            NullReferenceException exception = Assert.Throws<NullReferenceException>(act);
            Assert.Equal("Invalid account details.", exception.Message);
            Assert.IsType<NullReferenceException>(exception);
        }

        [Fact]
        public void Return_InvalidOperation_When_Amount_GreaterThan_Balance()
        {
            //Arrange
            var accountA = new Account() { Balance = 10m };

            var mockAccountRepo = new Mock<IAccountRepository>();
            mockAccountRepo.Setup(x => x.GetAccountById(It.IsAny<Guid>())).Returns(accountA);

            var sut = new WithdrawMoney(mockAccountRepo.Object, null);

            //Act
            Action act = () => sut.Execute(It.IsAny<Guid>(), 12m);

            //Assert
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(act);
            Assert.Equal("Insufficient funds to make transfer", exception.Message);
        }

        [Fact]
        public void Verify_Notification_Is_Called_LowBalance()
        {
            //Arrange
            var account_a_guid = new Guid("BC1165F6C64B4070BF955715C44D3182");
            var userA = new User() { Id = new Guid("5102C132883E4BB79C95B472F552E9F6"), Name = "Test User", Email = "test.user@mail.com" };
            var accountA = new Account() { Id = account_a_guid, User = userA, Balance = 100m, PaidIn = 500m };

            var mockAccountRepo = new Mock<IAccountRepository>();
            mockAccountRepo.Setup(x => x.GetAccountById(account_a_guid)).Returns(accountA);

            var mockNotificationSvc = new Mock<INotificationService>();

            var sut = new WithdrawMoney(mockAccountRepo.Object, mockNotificationSvc.Object);

            //Act
            sut.Execute(account_a_guid, 12m);

            //Assert
            mockNotificationSvc.Verify(x => x.NotifyFundsLow(It.IsAny<string>()), Times.Once);
        }
    }
}
