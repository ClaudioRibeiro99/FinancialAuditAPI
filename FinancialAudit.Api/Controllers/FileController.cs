using FinancialAudit.Application.DTOs;
using FinancialAudit.Application.Interfaces;
using FinancialAudit.Application.Utils;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FinancialAuditApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly ITransactionExportService _exportService;
    private readonly ITransactionImportService _importService;
    private readonly IValidator<ImportTransactionRequestDto> _validator;

    public FileController(
        ITransactionExportService exportService, 
        ITransactionImportService importService,
        IValidator<ImportTransactionRequestDto> validator)
    {
        _exportService = exportService;
        _importService = importService;
        _validator = validator;
    }

    /// <summary>
    /// Exporta todas as transações no formato especificado (CSV ou XLSX).
    /// </summary>
    /// <param name="request">
    /// Objeto que contém o formato de exportação desejado. Deve ser especificado como "csv" para CSV ou "xlsx" para Excel.
    /// Exemplo de formato aceito:
    /// 
    /// ```json
    /// {
    ///     "format": "csv"
    /// }
    /// ```
    /// </param>
    /// <returns>
    /// 200 OK: Retorna um arquivo contendo todas as transações no formato solicitado.
    /// - O arquivo será nomeado como 'transactions_{timestamp}.csv' para o formato CSV.
    /// - O arquivo será nomeado como 'transactions_{timestamp}.xlsx' para o formato Excel.
    /// 
    /// Exemplo de resposta bem-sucedida:
    /// - Cabeçalho Content-Type: "text/csv" para CSV ou "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" para Excel.
    /// - O corpo da resposta será um arquivo binário correspondente ao formato solicitado.
    /// 
    /// 400 Bad Request: Se o formato não for especificado ou for inválido.
    /// Exemplo de resposta para formato inválido:
    /// ```json
    /// {
    ///     "success": false,
    ///     "message": "Formato inválido. Deve ser 'csv' ou 'xlsx'."
    /// }
    /// ```
    /// 
    /// 404 Not Found: Se não houver transações disponíveis para exportação.
    /// Exemplo de resposta quando não há transações:
    /// ```json
    /// {
    ///     "success": false,
    ///     "message": "Nenhuma transação encontrada para exportar."
    /// }
    /// ```
    /// </returns>
    [HttpPost("export")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK, "csv/xlsx")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExportTransactions([FromQuery] ExportRequestDto request)
    {
        var result = await _exportService.ExportTransactionsAsync(request);

        return result.Match<IActionResult>(
            fileBytes => File(fileBytes, GetMimeType(request.Format), $"transactions_{DateTime.UtcNow}.{request.Format}"),
            error =>
            {
                if (error.ErrorType == ErrorType.Exception)
                    BadRequest(new ApiResponse<string?>(false, error.Detail));
                return NotFound(new ApiResponse<string?>(false, error.Detail));
            }
        );
    }

    /// <summary>
    /// Importa transações de um arquivo enviado.
    /// O arquivo deve ser no formato CSV ou XLSX, contendo as colunas: UserId, Amount, Type, e Date.
    /// Transações sem um usuário correspondente na base de dados serão ignoradas e relatadas no resultado.
    /// </summary>
    /// <param name="file">O arquivo contendo as transações a serem importadas (obrigatório).</param>
    /// <param name="format">O formato do arquivo, podendo ser "csv" ou "xlsx" (obrigatório).</param>
    /// <returns>
    /// 200 OK: Retorna o resultado da importação, incluindo uma lista das transações importadas com sucesso
    /// e uma lista das transações ignoradas (onde o UserId não foi encontrado no banco de dados).
    /// 400 Bad Request: Se o arquivo estiver em um formato inválido, ou se ocorrerem erros de validação.
    /// 404 Not Found: Se o arquivo não for encontrado ou se um formato inválido for especificado.
    /// </returns>
    /// <remarks>
    /// Exemplos de formatos de arquivo suportados:
    /// 
    /// Arquivo CSV:
    /// ```csv
    /// UserId,Amount,Type,Date
    /// 25ec78e9-5dd3-409f-aef1-44c715707fa1,5000.00,Deposit,20/10/2024 13:21:14
    /// e28d9d8a-3280-4d72-bfde-718cfe1fffb3,100.00,Purchase,22/10/2024 10:05:34
    /// ```
    /// 
    /// Arquivo XLSX:
    /// - As colunas devem ser as mesmas do formato CSV.
    /// - O formato da data deve ser `dd/MM/yyyy HH:mm:ss`.
    ///
    /// Exemplo de requisição via *form-data*:
    /// ```http
    /// POST /api/file/import?format=csv
    /// Content-Type: multipart/form-data
    ///
    /// File: transactions.csv
    /// ```
    /// </remarks>
    [HttpPost("import")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ImportTransactions(IFormFile file, [FromQuery] string format)
    {
        var request = new ImportTransactionRequestDto { File = file, Format = format };
        
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
            return BadRequest(new { Message = "Erros de validação encontrados.", Errors = errors });
        }

        var result = await _importService.ImportTransactionsAsync(file, format);
        
        return result.Match<IActionResult>(
            success => Ok(new ApiResponse<ImportResultDto>(true, success)),
            error =>
            {
                if (error.ErrorType == ErrorType.Exception)
                    return BadRequest(new ApiResponse<string?>(false, error.Detail));
                return NotFound(new ApiResponse<string?>(false, error.Detail));
            }
        );
    }

    private string GetMimeType(string format)
    {
        return format.ToLower() switch
        {
            "csv" => "text/csv",
            "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            _ => "application/octet-stream"
        };
    }
}