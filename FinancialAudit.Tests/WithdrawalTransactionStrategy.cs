using Bogus;
using FinancialAudit.Application.Strategies;
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
    public async Task Given_InsufficientBalance_When_Withdraw_Then_ThrowsException()
    {
        // Arrange
        var user = new Faker<User>().RuleFor(u => u.Balance, 30m).Generate();
        var withdrawalStrategy = new WithdrawalTransactionStrategy();
        var withdrawalAmount = 50m;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => withdrawalStrategy.ExecuteAsync(user, withdrawalAmount));
    }
}