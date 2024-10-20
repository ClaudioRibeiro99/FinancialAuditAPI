using Bogus;
using FinancialAudit.Application.Strategies;
using FinancialAudit.Application.Utils;
using FinancialAudit.Domain.Entities;

namespace FinancialAudit.Tests;

public class WithdrawalTransactionStrategyTests
{
    [Fact]
    public async Task Given_SufficientBalance_When_Withdraw_Then_BalanceIsDecreased()
    {
        // Arrange
        var user = new Faker<User>().RuleFor(u => u.Balance, 100m).Generate();
        var withdrawalStrategy = new WithdrawalTransactionStrategy();
        var withdrawalAmount = 50m;

        // Act
        await withdrawalStrategy.ExecuteAsync(user, withdrawalAmount);

        // Assert
        Assert.Equal(50m, user.Balance);
    }

    [Fact]
    public async Task Given_InsufficientBalance_When_Withdraw_Then_ReturnsInsufficientBalance()
    {
        // Arrange
        var user = new Faker<User>()
            .RuleFor(u => u.Balance, 30m)
            .Generate();
    
        var withdrawalStrategy = new WithdrawalTransactionStrategy();
        var withdrawalAmount = 50m;

        // Act
        var result = await withdrawalStrategy.ExecuteAsync(user, withdrawalAmount);

        // Assert
        Assert.Equal(TransactionResult.InsufficientBalance, result);
    }

}