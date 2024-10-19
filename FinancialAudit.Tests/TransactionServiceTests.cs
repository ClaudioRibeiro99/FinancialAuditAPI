using Bogus;
using FinancialAudit.Application;
using FinancialAudit.Application.Error;
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
    private readonly Mock<ITransactionRepository> _mockTransactionRepo;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly TransactionService _transactionService;
    private readonly Faker<Transaction> _transactionFaker;

    public TransactionServiceTests()
    {
        _mockTransactionRepo = new Mock<ITransactionRepository>();
        _mockUserRepo = new Mock<IUserRepository>();
        Mock<ILogger<TransactionService>> mockLogger = new();
        _transactionService = new TransactionService(_mockTransactionRepo.Object, _mockUserRepo.Object, mockLogger.Object);
        
        _transactionFaker = new Faker<Transaction>()
            .RuleFor(t => t.Id, f => f.Random.Int(1, 1000))
            .RuleFor(t => t.Amount, f => f.Finance.Amount(10, 1000))
            .RuleFor(t => t.Type, f => f.PickRandom<TransactionType>())
            .RuleFor(t => t.Date, f => f.Date.Past())
            .RuleFor(t => t.UserId, f => f.Random.Int(1, 10));
    }

    [Fact]
    public async Task Given_TransactionsExist_When_GetAllTransactionsAsyncCalled_Then_ReturnPagedResult()
    {
        // Arrange
        var transactions = _transactionFaker.Generate(10);
        _mockTransactionRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(transactions);

        // Act
        var result = await _transactionService.GetAllTransactionsAsync(1, 5);

        // Assert
        Assert.True(result.IsSuccess());
        var paginatedResult = result.GetSuccessResult();
        Assert.Equal(10, paginatedResult.TotalItems);
        Assert.Equal(5, paginatedResult.Items.Count());
    }

    [Fact]
    public async Task Given_PageNumberIsInvalid_When_GetAllTransactionsAsyncCalled_Then_ReturnInvalidPageNumberError()
    {
        // Arrange
        var transactions = _transactionFaker.Generate(3);
        _mockTransactionRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(transactions);

        // Act
        var result = await _transactionService.GetAllTransactionsAsync(2, 5);

        // Assert
        Assert.True(result.IsError());
        Assert.IsType<InvalidPageNumber>(result.GetError());
    }

    [Fact]
    public async Task Given_NoTransactionsExist_When_GetAllTransactionsAsyncCalled_Then_ReturnWithoutDataError()
    {
        // Arrange
        var transactions = new List<Transaction>();
        if (transactions == null) throw new ArgumentNullException(nameof(transactions));
        _mockTransactionRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(transactions);

        // Act
        var result = await _transactionService.GetAllTransactionsAsync(1, 5);

        // Assert
        Assert.True(result.IsError());
        Assert.IsType<ReturnedWithoutData>(result.GetError());
    }

    [Fact]
    public async Task Given_DatabaseErrorOccurs_When_GetAllTransactionsAsyncCalled_Then_ReturnDatabaseException()
    {
        // Arrange
        _mockTransactionRepo.Setup(repo => repo.GetAllAsync()).ThrowsAsync(new Exception("Erro no banco de dados"));

        // Act
        var result = await _transactionService.GetAllTransactionsAsync(1, 5);

        // Assert
        Assert.True(result.IsError());
        Assert.IsType<DatabaseException>(result.GetError());
    }

    [Fact]
    public async Task Given_UserHasInsufficientBalance_When_CreateTransactionAsyncCalled_Then_ReturnInsufficientBalanceError()
    {
        // Arrange
        var user = new Faker<User>().RuleFor(u => u.Balance, 50m).Generate();
        _mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(user);

        var transactionDto = new CreateTransactionDto
        {
            Amount = 100m,
            Type = "Withdrawal",
            UserId = 1
        };

        // Act
        var result = await _transactionService.CreateTransactionAsync(transactionDto);

        // Assert
        Assert.True(result.IsError());
        Assert.IsType<InsufficientBalance>(result.GetError());
    }

    [Fact]
    public async Task Given_UserDoesNotExist_When_CreateTransactionAsyncCalled_Then_ReturnWithoutDataError()
    {
        // Arrange
        _mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))!.ReturnsAsync((User)null!);

        var transactionDto = new CreateTransactionDto
        {
            Amount = 100m,
            Type = "Deposit",
            UserId = 1
        };

        // Act
        var result = await _transactionService.CreateTransactionAsync(transactionDto);

        // Assert
        Assert.True(result.IsError());
        Assert.IsType<ReturnedWithoutData>(result.GetError());
    }

    [Fact]
    public async Task Given_ValidTransaction_When_CreateTransactionAsyncCalled_Then_ReturnSuccess()
    {
        // Arrange
        var user = new Faker<User>().RuleFor(u => u.Balance, 100m).Generate();
        
        _mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(user);
        
        _mockTransactionRepo.Setup(repo => repo.BeginTransactionAsync())
            .ReturnsAsync(Mock.Of<IDbContextTransaction>());

        var transactionDto = new CreateTransactionDto
        {
            Amount = 50m,
            Type = "Deposit",
            UserId = user.Id
        };

        // Act
        var result = await _transactionService.CreateTransactionAsync(transactionDto);

        // Assert
        Assert.True(result.IsSuccess());
        Assert.Equal("Transação criada com sucesso", result.GetSuccessResult());
        
        _mockTransactionRepo.Verify(repo => repo.AddAsync(It.Is<Transaction>(t =>
            t.Amount == transactionDto.Amount && t.UserId == transactionDto.UserId
        )), Times.Once);
        
        _mockUserRepo.Verify(repo => repo.UpdateAsync(It.Is<User>(u =>
                u.Id == transactionDto.UserId && u.Balance == 150m
        )), Times.Once);
    }

}