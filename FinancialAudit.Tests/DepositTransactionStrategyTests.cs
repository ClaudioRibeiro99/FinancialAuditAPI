using Bogus;
using FinancialAudit.Application.Strategies;
using FinancialAudit.Domain.Entities;

namespace FinancialAudit.Tests;

public class DepositTransactionStrategyTests
{
    [Fact]
    public async Task Given_ValidUser_When_Deposit_Then_BalanceIsIncreased()
    {
        // Arrange
        var user = new Faker<User>()
            .RuleFor(u => u.Balance, 100m)
            .Generate();
        
        var depositStrategy = new DepositTransactionStrategy();
        var depositAmount = 50m;

        // Act
        await depositStrategy.ExecuteAsync(user, depositAmount);

        // Assert
        Assert.Equal(150m, user.Balance);
    }

    [Fact]
    public async Task Given_InvalidAmount_When_Deposit_Then_ThrowsException()
    {
        // Arrange
        var user = new Faker<User>().RuleFor(u => u.Balance, 100m).Generate();
        var depositStrategy = new DepositTransactionStrategy();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => depositStrategy.ExecuteAsync(user, -50m));
    }
}