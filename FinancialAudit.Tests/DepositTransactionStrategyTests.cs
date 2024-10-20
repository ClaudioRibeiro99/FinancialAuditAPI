using Bogus;
using FinancialAudit.Application.Strategies;
using FinancialAudit.Domain.Entities;
using FinancialAudit.Application.Utils;

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
        var result = await depositStrategy.ExecuteAsync(user, depositAmount);

        // Assert
        Assert.Equal(150m, user.Balance);
        Assert.Equal(TransactionResult.Success, result);
    }

    [Fact]
    public async Task Given_InvalidAmount_When_Deposit_Then_ReturnsInvalidTransaction()
    {
        // Arrange
        var user = new Faker<User>()
            .RuleFor(u => u.Balance, 100m)
            .Generate();
        
        var depositStrategy = new DepositTransactionStrategy();
        var invalidAmount = -50m;

        // Act
        var result = await depositStrategy.ExecuteAsync(user, invalidAmount);

        // Assert
        Assert.Equal(TransactionResult.InvalidTransaction, result);
    }
}