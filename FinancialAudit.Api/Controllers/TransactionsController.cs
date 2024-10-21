using FinancialAudit.Application.DTOs;
using FinancialAudit.Application.Interfaces;
using FinancialAudit.Application.Utils;
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
    
    
    /// <summary>
    /// Retorna uma lista paginada de transações.
    /// </summary>
    /// <param name="pageNumber">O número da página a ser retornada. O valor padrão é 1.</param>
    /// <param name="pageSize">O número de itens por página. O valor padrão e máximo é 10.</param>
    /// <returns>
    /// 200 OK: Retorna a lista paginada de transações.
    /// 204 No Content: Se não houver transações disponíveis.
    /// 400 Bad Request: Se ocorrer algum erro durante o processamento.
    /// </returns>
    /// <remarks>
    /// Exemplo de requisição para recuperar a primeira página com 10 transações:
    /// 
    /// GET /api/Transactions?pageNumber=1&amp;pageSize=10
    /// 
    /// Exemplo de resposta com transações paginadas:
    /// 
    /// ```json
    /// {
    ///     "success": true,
    ///     "data": {
    ///         "items": [
    ///             {
    ///                 "id": 1,
    ///                 "userId": 123,
    ///                 "amount": 150.75,
    ///                 "type": "Deposit",
    ///                 "timestamp": "2024-01-15T10:30:00Z"
    ///             },
    ///             {
    ///                 "id": 2,
    ///                 "userId": 123,
    ///                 "amount": 50.00,
    ///                 "type": "Withdrawal",
    ///                 "timestamp": "2024-01-16T11:00:00Z"
    ///             }
    ///         ],
    ///         "totalCount": 2,
    ///         "pageSize": 10,
    ///         "currentPage": 1,
    ///         "totalPages": 1
    ///     }
    /// }
    /// ```
    /// </remarks>
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
    
    /// <summary>
    /// Retorna o saldo atual de um usuário específico.
    /// </summary>
    /// <param name="userId">ID do usuário cujo saldo será retornado.</param>
    /// <returns>
    /// 200 OK: Retorna o saldo do usuário.
    /// 404 Not Found: Se o usuário não for encontrado.
    /// 400 Bad Request: Se ocorrer um erro durante o processamento.
    /// </returns>
    /// <remarks>
    /// Exemplo de requisição para obter o saldo de um usuário:
    /// 
    /// GET /api/Transactions/user/123/balance
    /// 
    /// Exemplo de resposta com saldo do usuário:
    /// 
    /// ```json
    /// {
    ///     "success": true,
    ///     "data": {
    ///         "userId": 123,
    ///         "balance": 1000.50
    ///     }
    /// }
    /// ```
    /// 
    /// Caso o usuário não seja encontrado, o retorno será:
    /// 
    /// ```json
    /// {
    ///     "success": false,
    ///     "message": "Usuário não encontrado"
    /// }
    /// ```
    /// </remarks>
    [HttpGet("user/{userId}/balance")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserBalance(Guid userId)
    {
        var result = await _transactionService.GetUserBalanceAsync(userId);

        return result.Match<IActionResult>(
            userBalance => Ok(new ApiResponse<UserBalanceDto>(true, userBalance)),
            error =>
            {
                if (error.ErrorType == ErrorType.Exception)
                    BadRequest(new ApiResponse<string?>(false, error.Detail));
                return NotFound(new ApiResponse<string?>(false, error.Detail));
            }
        );
    }
    
    /// <summary>
    /// Cria uma nova transação financeira.
    /// Tipos de transações permitidos: "Deposit", "Withdrawal", "Purchase".
    /// </summary>
    /// <param name="transactionDto">Dados da transação a ser criada, incluindo o tipo de transação (Deposit, Withdrawal, Purchase).</param>
    /// <returns>
    /// 201 Created: Se a transação for criada com sucesso.
    /// 400 Bad Request: Se os dados fornecidos forem inválidos ou ocorrer algum erro durante o processamento.
    /// </returns>
    /// <remarks>
    /// Exemplos de tipos de transações:
    /// - **Deposit**: Adiciona fundos à conta do usuário.
    /// - **Withdrawal**: Deduz fundos da conta do usuário.
    /// - **Purchase**: Deduz fundos como parte de uma compra.
    ///
    /// Exemplo de JSON para criação de uma transação:
    /// 
    /// ```json
    /// {
    ///     "userId": 123,
    ///     "amount": 150.75,
    ///     "type": "Deposit"
    /// }
    /// ```
    /// 
    /// O campo `transactionDto.Type` deve ser preenchido com um dos seguintes valores:
    /// - "Deposit" (Depósito)
    /// - "Withdrawal" (Saque)
    /// - "Purchase" (Compra)
    ///
    /// Além disso, certifique-se de que o saldo do usuário é suficiente para "Withdrawal" ou "Purchase".
    /// </remarks>
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