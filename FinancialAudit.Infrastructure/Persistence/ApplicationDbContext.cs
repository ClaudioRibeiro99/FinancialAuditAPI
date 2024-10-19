using FinancialAudit.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinancialAudit.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.User)
            .WithMany(u => u.Transactions);
        
        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasPrecision(18, 2);
        
        modelBuilder.Entity<User>()
            .Property(u => u.Balance)
            .HasPrecision(18, 2);

        base.OnModelCreating(modelBuilder);
    }
}