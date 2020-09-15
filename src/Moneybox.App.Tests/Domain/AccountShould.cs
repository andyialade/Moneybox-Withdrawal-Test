using System;
using Xunit;

namespace Moneybox.App.Tests
{
    public class AccountShould
    {
        [Fact]
        public void Return_InvalidOperation_When_Amount_GreaterThan_Balance()
        {
            //Arrange
            var sut = new Account() { Balance = 10m };

            //Act
            Action act = () => sut.SendMoney(12m);

            //Assert
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(act);
            Assert.Equal("Insufficient funds to make transfer", exception.Message);
            Assert.Equal(10m, sut.Balance);
        }

        [Fact]
        public void Return_True_When_Balance_LessThan_MinimumAllowed()
        {
            //Arrange
            var sut = new Account() { Balance = 499m };

            //Act
            var result = sut.IsBalanceLow();

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void Return_False_When_Balance_GreaterThan_MinimumAllowed()
        {
            //Arrange
            var sut = new Account() { Balance = 501m };

            //Act
            var result = sut.IsBalanceLow();

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Return_Updated_Balance()
        {
            //Arrange
            var sut = new Account() { Balance = 10m, Withdrawn = 10m };

            //Act
            sut.SendMoney(2m);

            //Assert
            Assert.Equal(8m, sut.Balance);
            Assert.Equal(8m, sut.Withdrawn);
        }

        [Fact]
        public void Return_InvalidOperation_When_Amount_GreaterThan_PaidIn()
        {
            //Arrange
            var sut = new Account() { PaidIn = 3950m };

            //Act
            Action act = () => sut.ReceiveMoney(55m);

            //Assert
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(act);
            Assert.Equal("Account pay in limit reached", exception.Message);
            Assert.Equal(3950m, sut.PaidIn);
        }

        [Fact]
        public void Return_True_When_PaidIn_LessThan_Limit()
        {
            //Arrange
            var sut = new Account() { PaidIn = 3520m };

            //Act
            var result = sut.IsPayInLimitClose();

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void Return_False_When_PaidIn_GreaterThan_Limit()
        {
            //Arrange
            var sut = new Account() { PaidIn = 3500m };

            //Act
            var result = sut.IsPayInLimitClose();

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Return_Updated_PaidIn()
        {
            //Arrange
            var sut = new Account() { PaidIn = 3800m, Balance = 100m };

            //Act
            sut.ReceiveMoney(100m);

            //Assert
            Assert.Equal(3900m, sut.PaidIn);
            Assert.Equal(200m, sut.Balance);
        }


    }
}
