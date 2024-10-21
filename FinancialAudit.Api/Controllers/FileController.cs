using FinancialAudit.Application.DTOs;
using FinancialAudit.Application.Interfaces;
using FinancialAudit.Application.Utils;
using Microsoft.AspNetCore.Mvc;

namespace FinancialAuditApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly ITransactionExportService _exportService;
    private readonly ITransactionImportService _importService;

    public FileController(ITransactionExportService exportService, ITransactionImportService importService)
    {
        _exportService = exportService;
        _importService = importService;
    }

    /// <summary>
    /// Exporta todas as transações no formato especificado (CSV ou XLSX).
    /// </summary>
    /// <param name="request">
    /// Objeto que contém o formato de exportação desejado (CSV ou XLSX). 
    /// Informe o formato como "csv" para CSV ou "xlsx" para Excel.
    /// Exemplo de formato aceito:
    /// 
    /// ```json
    /// {
    ///     "format": "csv"
    /// }
    /// ```
    /// </param>
    /// <returns>
    /// Retorna um arquivo contendo as transações no formato solicitado.
    /// Se o formato for CSV, o arquivo será retornado como 'transactions_{timestamp}.csv'.
    /// Se o formato for XLSX, o arquivo será retornado como 'transactions_{timestamp}.xlsx'.
    /// 
    /// Exemplo de resposta bem-sucedida (200 OK):
    /// - Cabeçalho Content-Type: "text/csv" ou "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    /// - Corpo: Arquivo binário correspondente ao formato solicitado.
    /// 
    /// Exemplo de erro (400 Bad Request):
    /// - Quando o formato não é especificado ou é inválido.
    /// - Corpo da resposta: 
    /// ```json
    /// {
    ///     "success": false,
    ///     "message": "Formato inválido. Deve ser 'csv' ou 'xlsx'."
    /// }
    /// ```
    /// 
    /// Exemplo de erro (404 Not Found):
    /// - Quando não há transações disponíveis para exportação.
    /// - Corpo da resposta: 
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
    public async Task<IActionResult> ExportTransactions([FromBody] ExportRequestDto request)
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
/// O arquivo deve ser no formato CSV ou XLSX, e deve conter as colunas: UserId, Amount, Type, e Date.
/// Transações sem um usuário correspondente na base de dados serão ignoradas e relatadas no resultado.
/// </summary>
/// <param name="file">O arquivo contendo as transações a serem importadas.</param>
/// <param name="format">O formato do arquivo, podendo ser "csv" ou "xlsx".</param>
/// <returns>
/// 200 OK: Retorna o resultado da importação, incluindo uma lista das transações importadas com sucesso
/// e uma lista das transações ignoradas (onde o UserId não foi encontrado no banco de dados).
/// 400 Bad Request: Se o arquivo estiver em um formato inválido ou se ocorrer algum erro de validação.
/// 404 Not Found: Se não for encontrado o arquivo ou um formato inválido for especificado.
/// </returns>
/// <remarks>
/// Exemplos de formatos de arquivo suportados:
/// 
/// Arquivo CSV:
/// ```csv
/// UserId,Amount,Type,Date
/// 25ec78e9-5dd3-409f-aef1-44c715707fa1,5000.00,Deposit,20/10/2024 13:21:14
/// 25ec78e9-5dd3-409f-aef1-44c715707fa1,100.00,Purchase,20/10/2024 13:21:14
/// ```
/// 
/// Arquivo XLSX:
/// - As colunas devem ser as mesmas do formato CSV.
/// - O formato de data deve ser `dd/MM/yyyy HH:mm:ss`.
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
        var result = await _importService.ImportTransactionsAsync(request.File, request.Format);

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