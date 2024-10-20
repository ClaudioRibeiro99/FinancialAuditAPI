using FinancialAudit.Application.DTOs;
using FinancialAudit.Application.Interfaces;
using FinancialAudit.Application.Utils;
using FinancialAudit.Domain.Interfaces;
using OneOf;

namespace FinancialAudit.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<OneOf<UserBalanceDto, AppError>> GetUserBalanceAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return new ReturnedWithoutData();

        var userBalanceDto = new UserBalanceDto
        {
            UserId = user.Id,
            Balance = user.Balance
        };

        return userBalanceDto;
    }

    public async Task<bool> UserExistsAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null;
    }
}