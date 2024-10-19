using Bogus;
using FinancialAudit.Application;
using FinancialAudit.Application.Interfaces;
using FinancialAudit.Application.Services;
using FinancialAudit.Application.Utils;
using FinancialAudit.Domain.Entities;
using FinancialAudit.Domain.Interfaces;
using FinancialAuditApi.DTOs;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinancialAudit.Tests;

public class TransactionServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<ITransactionStrategyFactory> _mockStrategyFactory;
    private readonly TransactionService _transactionService;

    public TransactionServiceTests()
    {
        Mock<ITransactionRepository> mockTransactionRepo = new();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockStrategyFactory = new Mock<ITransactionStrategyFactory>();
        Mock<ILogger<TransactionService>> mockLogger = new();
        
        _transactionService = new TransactionService(
            mockTransactionRepo.Object, 
            _mockUserRepo.Object, 
            _mockStrategyFactory.Object, 
            mockLogger.Object);
    }

    [Fact]
    public async Task Given_ValidDepositTransaction_When_CreateTransactionAsyncCalled_Then_StrategyExecutesSuccessfully()
    {
        // Arrange
        var user = new Faker<User>().RuleFor(u => u.Balance, 100m).Generate();
        _mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(user);

        var depositStrategy = new Mock<ITransactionStrategy>();
        _mockStrategyFactory.Setup(f => f.GetStrategy("Deposit")).Returns(depositStrategy.Object);
        
        var mockTransactionRepo = new Mock<ITransactionRepository>();
        mockTransactionRepo.Setup(repo => repo.BeginTransactionAsync()).ReturnsAsync(Mock.Of<IDbContextTransaction>());

        var transactionDto = new CreateTransactionDto
        {
            Amount = 50m,
            Type = "Deposit",
            UserId = 1
        };
        
        var transactionService = new TransactionService(
            mockTransactionRepo.Object, 
            _mockUserRepo.Object, 
            _mockStrategyFactory.Object, 
            Mock.Of<ILogger<TransactionService>>());

        // Act
        var result = await transactionService.CreateTransactionAsync(transactionDto);

        // Assert
        Assert.True(result.IsSuccess());
        depositStrategy.Verify(s => s.ExecuteAsync(user, transactionDto.Amount), Times.Once);
        mockTransactionRepo.Verify(repo => repo.BeginTransactionAsync(), Times.Once);
    }


    [Fact]
    public async Task Given_InvalidUser_When_CreateTransactionAsyncCalled_Then_ReturnWithoutDataError()
    {
        // Arrange
        _mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))!.ReturnsAsync((User)null!);

        var transactionDto = new CreateTransactionDto
        {
            Amount = 50m,
            Type = "Deposit",
            UserId = 1
        };

        // Act
        var result = await _transactionService.CreateTransactionAsync(transactionDto);

        // Assert
        Assert.True(result.IsError());
        Assert.IsType<ReturnedWithoutData>(result.GetError());
    }
}