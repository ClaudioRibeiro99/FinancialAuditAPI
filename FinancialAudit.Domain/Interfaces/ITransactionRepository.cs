using FinancialAudit.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace FinancialAudit.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetAllAsync();
    Task AddAsync(Transaction transaction);
    Task<IDbContextTransaction> BeginTransactionAsync(); 
}