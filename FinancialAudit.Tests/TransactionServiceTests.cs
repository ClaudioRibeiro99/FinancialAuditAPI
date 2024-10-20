using Bogus;
using FinancialAudit.Application;
using FinancialAudit.Application.DTOs;
using FinancialAudit.Application.Interfaces;
using FinancialAudit.Application.Services;
using FinancialAudit.Application.Utils;
using FinancialAudit.Domain.Entities;
using FinancialAudit.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinancialAudit.Tests;

public class TransactionServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<ITransactionStrategyFactory> _mockStrategyFactory;
    private readonly TransactionService _transactionService;
    private readonly Mock<ITransactionRepository> _mockTransactionRepo;

    public TransactionServiceTests()
    {
        _mockTransactionRepo = new Mock<ITransactionRepository>();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockStrategyFactory = new Mock<ITransactionStrategyFactory>();
        Mock<ILogger<TransactionService>> mockLogger = new();
        
        _transactionService = new TransactionService(
            _mockTransactionRepo.Object, 
            _mockUserRepo.Object, 
            _mockStrategyFactory.Object, 
            mockLogger.Object);
    }

    [Fact]
    public async Task Given_ValidDepositTransaction_When_CreateTransactionAsyncCalled_Then_StrategyExecutesSuccessfully()
    {
        // Arrange
        var user = new Faker<User>().RuleFor(u => u.Balance, 100m).Generate();
        _mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);

        var depositStrategy = new Mock<ITransactionStrategy>();
        depositStrategy.Setup(s => s.ExecuteAsync(user, It.IsAny<decimal>())).ReturnsAsync(TransactionResult.Success);
        _mockStrategyFactory.Setup(f => f.GetStrategy("Deposit")).Returns(depositStrategy.Object);
        
        _mockTransactionRepo.Setup(repo => repo.BeginTransactionAsync()).ReturnsAsync(Mock.Of<IDbContextTransaction>());

        var transactionDto = new CreateTransactionDto
        {
            Amount = 50m,
            Type = "Deposit",
            UserId = Guid.NewGuid()
        };

        // Act
        var result = await _transactionService.CreateTransactionAsync(transactionDto);

        // Assert
        Assert.True(result.IsSuccess());
        depositStrategy.Verify(s => s.ExecuteAsync(user, transactionDto.Amount), Times.Once);
        _mockTransactionRepo.Verify(repo => repo.BeginTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task Given_InvalidUser_When_CreateTransactionAsyncCalled_Then_ReturnWithoutDataError()
    {
        // Arrange
        _mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User)null!);

        var transactionDto = new CreateTransactionDto
        {
            Amount = 50m,
            Type = "Deposit",
            UserId = Guid.NewGuid()
        };

        // Act
        var result = await _transactionService.CreateTransactionAsync(transactionDto);

        // Assert
        Assert.True(result.IsError());
        Assert.IsType<ReturnedWithoutData>(result.GetError());
    }

    [Fact]
    public async Task Given_InsufficientBalance_When_CreateTransactionAsyncCalled_Then_ReturnsInsufficientBalanceError()
    {
        // Arrange
        var user = new Faker<User>().RuleFor(u => u.Balance, 30m).Generate();
        _mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);

        var withdrawalStrategy = new Mock<ITransactionStrategy>();
        withdrawalStrategy.Setup(s => s.ExecuteAsync(user, It.IsAny<decimal>())).ReturnsAsync(TransactionResult.InsufficientBalance);
        _mockStrategyFactory.Setup(f => f.GetStrategy("Withdrawal")).Returns(withdrawalStrategy.Object);

        var transactionDto = new CreateTransactionDto
        {
            Amount = 50m,
            Type = "Withdrawal",
            UserId = Guid.NewGuid()
        };

        // Act
        var result = await _transactionService.CreateTransactionAsync(transactionDto);

        // Assert
        Assert.True(result.IsError());
        Assert.IsType<InsufficientBalance>(result.GetError());
    }

    [Fact]
    public async Task Given_ValidUser_When_GetUserBalanceCalled_Then_ReturnsCorrectBalance()
    {
        // Arrange
        var user = new Faker<User>()
            .RuleFor(u => u.Id, Guid.NewGuid())
            .RuleFor(u => u.Balance, 100m)
            .Generate();

        _mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);

        // Act
        var result = await _transactionService.GetUserBalanceAsync(user.Id);

        // Assert
        Assert.True(result.IsSuccess());
        Assert.Equal(user.Balance, result.AsT0.Balance);
    }

    [Fact]
    public async Task Given_InvalidUser_When_GetUserBalanceCalled_Then_ReturnsUserNotFoundError()
    {
        // Arrange
        _mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User)null!);

        // Act
        var result = await _transactionService.GetUserBalanceAsync(Guid.NewGuid());

        // Assert
        Assert.True(result.IsError());
        Assert.IsType<UserNotFound>(result.GetError());
    }

    [Fact]
    public async Task Given_ValidPageRequest_When_GetAllTransactionsCalled_Then_ReturnsPagedTransactions()
    {
        // Arrange
        var transactions = new Faker<Transaction>()
            .RuleFor(t => t.Amount, f => f.Finance.Amount())
            .RuleFor(t => t.Type, f => f.PickRandom<TransactionType>())
            .RuleFor(t => t.UserId, f => f.Random.Guid())
            .Generate(20);
        
        _mockTransactionRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(transactions);

        // Act
        var result = await _transactionService.GetAllTransactionsAsync(1, 10);

        // Assert
        Assert.True(result.IsSuccess());
        var pagedResult = result.AsT0;
        Assert.Equal(10, pagedResult.Items.Count());
        Assert.Equal(2, pagedResult.TotalPages);
    }

    [Fact]
    public async Task Given_InvalidPageNumber_When_GetAllTransactionsCalled_Then_ReturnsInvalidPageNumberError()
    {
        // Arrange
        var transactions = new Faker<Transaction>()
            .RuleFor(t => t.Amount, f => f.Finance.Amount())
            .RuleFor(t => t.Type, f => f.PickRandom<TransactionType>())
            .RuleFor(t => t.UserId, f => f.Random.Guid())
            .Generate(20);

        _mockTransactionRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(transactions);

        // Act
        var result = await _transactionService.GetAllTransactionsAsync(5, 10);

        // Assert
        Assert.True(result.IsError());
        Assert.IsType<InvalidPageNumber>(result.GetError());
    }

    [Fact]
    public async Task Given_ExceptionInRepository_When_GetAllTransactionsCalled_Then_ReturnsInternalErrorException()
    {
        // Arrange
        _mockTransactionRepo.Setup(repo => repo.GetAllAsync()).ThrowsAsync(new Exception("DB Error"));

        // Act
        var result = await _transactionService.GetAllTransactionsAsync(1, 10);

        // Assert
        Assert.True(result.IsError());
        Assert.IsType<InternalErrorException>(result.GetError());
    }
}
