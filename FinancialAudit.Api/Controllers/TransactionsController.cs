using System.Text.RegularExpressions;
using FinancialAudit.Application.DTOs;
using FinancialAudit.Application.Interfaces;
using FinancialAudit.Application.Utils;
using FinancialAuditApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FinancialAuditApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetAllTransactions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _transactionService.GetAllTransactionsAsync(pageNumber, pageSize);

        return result.Match<IActionResult>(
            transactions => Ok(new ApiResponse<PaginatedResult<TransactionDto>>(true, transactions)),
            error =>
            {
                if (error.ErrorType == ErrorType.Exception)
                    return BadRequest(new ApiResponse<AppError>(false, error));
                return NoContent();
            });
    }
    
    [HttpGet("user/{userId}/balance")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserBalance(int userId)
    {
        var result = await _transactionService.GetUserBalanceAsync(userId);

        return result.Match<IActionResult>(
            userBalance => Ok(new ApiResponse<UserBalanceDto>(true, userBalance)),
            error =>
            {
                if (error.ErrorType == ErrorType.Exception)
                    BadRequest(new ApiResponse<string?>(false, error.Detail));
                return NotFound(new ApiResponse<string?>(false, error.Detail));;
            }
        );
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionDto transactionDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse<string?>(false, "Dados inválidos"));

        var result = await _transactionService.CreateTransactionAsync(transactionDto);
        
        return result.Match<IActionResult>(
            success => StatusCode(StatusCodes.Status201Created, new ApiResponse<string>(true, success)),
            error => BadRequest(new ApiResponse<string?>(false, error.Detail))
        );
        
    }
}