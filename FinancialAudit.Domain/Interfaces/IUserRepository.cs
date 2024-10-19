using FinancialAudit.Domain.Entities;

namespace FinancialAudit.Domain.Interfaces;

public interface IUserRepository
{
    Task<User> GetByIdAsync(int id);
    Task UpdateAsync(User user);
}