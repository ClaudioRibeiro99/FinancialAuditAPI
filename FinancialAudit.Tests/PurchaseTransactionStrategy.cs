using Bogus;
using FinancialAudit.Application.Strategies;
using FinancialAudit.Domain.Entities;

namespace FinancialAudit.Tests;

public class PurchaseTransactionStrategyTests
{
    [Fact]
    public async Task Given_SufficientBalance_When_Purchase_Then_BalanceIsDecreased()
    {
        // Arrange
        var user = new Faker<User>().RuleFor(u => u.Balance, 200m).Generate();
        var purchaseStrategy = new PurchaseTransactionStrategy();
        var purchaseAmount = 100m;

        // Act
        await purchaseStrategy.ExecuteAsync(user, purchaseAmount);

        // Assert
        Assert.Equal(100m, user.Balance);
    }

    [Fact]
    public async Task Given_InsufficientBalance_When_Purchase_Then_ThrowsException()
    {
        // Arrange
        var user = new Faker<User>().RuleFor(u => u.Balance, 50m).Generate();
        var purchaseStrategy = new PurchaseTransactionStrategy();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => purchaseStrategy.ExecuteAsync(user, 100m));
    }
}