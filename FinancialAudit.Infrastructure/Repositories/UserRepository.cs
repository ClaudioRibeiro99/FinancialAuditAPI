using FinancialAudit.Domain.Entities;
using FinancialAudit.Domain.Interfaces;
using FinancialAudit.Infrastructure.Persistence;

namespace FinancialAudit.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetByIdAsync(int id)
    {
        return (await _context.Users.FindAsync(id))!;
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}