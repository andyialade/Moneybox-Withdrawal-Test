using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;
using System;
using Xunit;

namespace Moneybox.App.Tests
{
    public class TransferMoneyShould
    {
        [Fact]
        public void Return_NullException_When_Amount_FromAccount_Is_Null()
        {
            //Arrange
            var mockAccountRepo = new Mock<IAccountRepository>();
            mockAccountRepo.Setup(x => x.GetAccountById(It.IsAny<Guid>())).Returns((Account)null);

            var sut = new TransferMoney(mockAccountRepo.Object, null);

            //Act
            Action act = () => sut.Execute(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>());

            //Assert
            NullReferenceException exception = Assert.Throws<NullReferenceException>(act);
            Assert.Equal("Invalid account details for fromAccountId", exception.Message);
            Assert.IsType<NullReferenceException>(exception);
        }

        [Fact]
        public void Return_NullException_When_Amount_ToAccount_Is_Null()
        {
            //Arrange
            var account_a_guid = new Guid("BC1165F6C64B4070BF955715C44D3182"); 
            var accountA = new Account() { Id = account_a_guid};

            var mockAccountRepo = new Mock<IAccountRepository>();
            mockAccountRepo.Setup(x => x.GetAccountById(account_a_guid)).Returns(accountA);

            var sut = new TransferMoney(mockAccountRepo.Object, null);

            //Act
            Action act = () => sut.Execute(account_a_guid, It.IsAny<Guid>(), It.IsAny<decimal>());

            //Assert
            NullReferenceException exception = Assert.Throws<NullReferenceException>(act);
            Assert.Equal("Invalid account details for toAccountId", exception.Message);
            Assert.IsType<NullReferenceException>(exception);
        }

        [Fact]
        public void Return_InvalidOperation_When_Amount_GreaterThan_Balance()
        {
            //Arrange
            var accountA = new Account() { Balance = 10m };

            var mockAccountRepo = new Mock<IAccountRepository>();
            mockAccountRepo.Setup(x => x.GetAccountById(It.IsAny<Guid>())).Returns(accountA);
            
            var sut = new TransferMoney(mockAccountRepo.Object, null);

            //Act
            Action act = () => sut.Execute(It.IsAny<Guid>(), It.IsAny<Guid>(), 12m);

            //Assert
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(act);
            Assert.Equal("Insufficient funds to make transfer", exception.Message);
        }

        [Fact]
        public void Return_InvalidOperation_When_Amount_GreaterThan_PaidIn()
        {
            //Arrange
            var account_a_guid = new Guid("BC1165F6C64B4070BF955715C44D3182");
            var userA = new User() { Id = new Guid("5102C132883E4BB79C95B472F552E9F6"), Name = "Test User", Email = "test.user@mail.com" };
            var accountA = new Account() { Id = account_a_guid, User = userA, Balance = 100m };

            var account_b_guid = new Guid("162F6DB74C53449B818D7195DE940F0B");
            var userB = new User() { Id = new Guid("98DB31C6299E40CD94D6FE8C6C06A93F"), Name = "Test UserB", Email = "test.userb@mail.com" };
            var accountB = new Account() { Id = account_b_guid, User = userA, Balance = 100m, PaidIn = 3990m };

            var mockAccountRepo = new Mock<IAccountRepository>();
            mockAccountRepo.Setup(x => x.GetAccountById(account_a_guid)).Returns(accountA);
            mockAccountRepo.Setup(x => x.GetAccountById(account_b_guid)).Returns(accountB);

            var mockNotificationSvc = new Mock<INotificationService>();

            var sut = new TransferMoney(mockAccountRepo.Object, mockNotificationSvc.Object);

            //Act
            Action act = () => sut.Execute(account_a_guid, account_b_guid, 20m);

            //Assert
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(act);
            Assert.Equal("Account pay in limit reached", exception.Message);
        }

        [Fact]
        public void Verify_Notifications_Are_Called_LowBalance_And_PaidInLimit()
        {
            //Arrange
            var account_a_guid = new Guid("BC1165F6C64B4070BF955715C44D3182");
            var userA = new User() { Id = new Guid("5102C132883E4BB79C95B472F552E9F6"), Name =  "Test User", Email = "test.user@mail.com" };
            var accountA = new Account() { Id = account_a_guid, User = userA,  Balance = 100m, PaidIn = 500m };

            var account_b_guid = new Guid("162F6DB74C53449B818D7195DE940F0B");
            var userB = new User() { Id = new Guid("98DB31C6299E40CD94D6FE8C6C06A93F"), Name = "Test UserB", Email = "test.userb@mail.com" };
            var accountB = new Account() { Id = account_b_guid, User = userA, Balance = 100m, PaidIn = 3520m };

            var mockAccountRepo = new Mock<IAccountRepository>();
            mockAccountRepo.Setup(x => x.GetAccountById(account_a_guid)).Returns(accountA);
            mockAccountRepo.Setup(x => x.GetAccountById(account_b_guid)).Returns(accountB);

            var mockNotificationSvc = new Mock<INotificationService>();

            var sut = new TransferMoney(mockAccountRepo.Object, mockNotificationSvc.Object);

            //Act
            sut.Execute(account_a_guid, account_b_guid, 12m);

            //Assert
            mockNotificationSvc.Verify(x => x.NotifyFundsLow(It.IsAny<string>()), Times.Once);
            mockNotificationSvc.Verify(x => x.NotifyApproachingPayInLimit(It.IsAny<string>()), Times.Once);
        }
    }
}
