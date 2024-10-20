using FinancialAudit.Application.DTOs;
using FinancialAudit.Application.Utils;
using OneOf;

namespace FinancialAudit.Application.Interfaces
{
    public interface IUserService
    {
        Task<OneOf<UserBalanceDto, AppError>> GetUserBalanceAsync(Guid userId);
        Task<bool> UserExistsAsync(Guid userId);
    }
}