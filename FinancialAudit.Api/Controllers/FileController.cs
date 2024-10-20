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

    public FileController(ITransactionExportService exportService)
    {
        _exportService = exportService;
    }

    /// <summary>
    /// Exporta as transações no formato especificado (CSV ou XLSX).
    /// </summary>
    /// <param name="request">O formato de exportação, por exemplo, "csv" ou "xlsx".</param>
    /// <returns>Um arquivo exportado no formato solicitado.</returns>
    [HttpPost("export")]
    public async Task<IActionResult> ExportTransactions([FromBody] ExportRequestDto request)
    {
        var result = await _exportService.ExportTransactionsAsync(request);

        return result.Match<IActionResult>(
            fileBytes => File(fileBytes, GetMimeType(request.Format), $"transactions.{request.Format}"),
            error =>
            {
                if (error.ErrorType == ErrorType.Exception)
                    BadRequest(new ApiResponse<string?>(false, error.Detail));
                return NotFound(new ApiResponse<string?>(false, error.Detail));
            }
        );
    }

    /// <summary>
    /// Importa transações de um arquivo enviado (futuro).
    /// </summary>
    /// <returns>Resultado da importação.</returns>
    [HttpPost("import")]
    public IActionResult ImportTransactions()
    {
        // Lógica futura de importação de transações a partir de um arquivo
        return Ok(new { Success = true, Message = "Importação de transações será implementada." });
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