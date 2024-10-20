using FinancialAudit.Domain.Entities;
using FinancialAudit.Infrastructure.Persistence;

namespace FinancialAudit.Infrastructure;

public static class SeedDatabase
{
    public static async Task SeedAsync(ApplicationDbContext dbContext)
    {
        if (!dbContext.Users.Any())
        {
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Name = "Claudio Marcelo", Balance = 5000.00m },
                new User { Id = Guid.NewGuid(), Name = "Eder Moises", Balance = 3000.00m }
            };
        
            await dbContext.Users.AddRangeAsync(users);
            await dbContext.SaveChangesAsync();
            
            var transactions = new List<Transaction>
            {
                new Transaction { UserId = users[0].Id, Amount = 5000.00m, Type = TransactionType.Deposit, Date = DateTime.UtcNow },
                new Transaction { UserId = users[1].Id, Amount = 1500.00m, Type = TransactionType.Deposit, Date = DateTime.UtcNow },
                new Transaction { UserId = users[0].Id, Amount = 100.00m, Type = TransactionType.Purchase, Date = DateTime.UtcNow },
                new Transaction { UserId = users[1].Id, Amount = 73.20m, Type = TransactionType.Purchase, Date = DateTime.UtcNow },
                new Transaction { UserId = users[0].Id, Amount = 27.00m, Type = TransactionType.Withdrawal, Date = DateTime.UtcNow },
                new Transaction { UserId = users[1].Id, Amount = 15.00m, Type = TransactionType.Withdrawal, Date = DateTime.UtcNow }
            };
        
            await dbContext.Transactions.AddRangeAsync(transactions);
            await dbContext.SaveChangesAsync();
        }
    }
}