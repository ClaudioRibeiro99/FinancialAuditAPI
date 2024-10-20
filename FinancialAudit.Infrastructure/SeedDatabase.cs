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
                new User { Name = "João Paulo", Balance = 1000.00m },
                new User { Name = "Maria Antonia", Balance = 2000.50m }
            };

            dbContext.Users.AddRange(users);
            await dbContext.SaveChangesAsync();
        }
        
        if (!dbContext.Transactions.Any())
        {
            var transactions = new List<Transaction>
            {
                new Transaction { UserId = 1, Amount = 5000.00m, Type = TransactionType.Deposit, Date = DateTime.UtcNow },
                new Transaction { UserId = 2, Amount = 1500.00m, Type = TransactionType.Deposit, Date = DateTime.UtcNow },
                new Transaction { UserId = 1, Amount = 100.00m, Type = TransactionType.Purchase, Date = DateTime.UtcNow },
                new Transaction { UserId = 2, Amount = 73.20m, Type = TransactionType.Purchase, Date = DateTime.UtcNow },
                new Transaction { UserId = 1, Amount = 27.00m, Type = TransactionType.Withdrawal, Date = DateTime.UtcNow },
                new Transaction { UserId = 2, Amount = 15.00m, Type = TransactionType.Withdrawal, Date = DateTime.UtcNow }
            };

            dbContext.Transactions.AddRange(transactions);
            await dbContext.SaveChangesAsync();
        }
    }
}