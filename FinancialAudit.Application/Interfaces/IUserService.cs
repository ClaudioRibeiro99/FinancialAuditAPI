using FinancialAudit.Application.DTOs;
using FinancialAudit.Application.Utils;
using OneOf;

namespace FinancialAudit.Application.Interfaces
{
    public interface IUserService
    {
        Task<OneOf<UserBalanceDto, AppError>> GetUserBalanceAsync(int userId);
        Task<bool> UserExistsAsync(int userId);
    }
}