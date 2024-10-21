using Microsoft.AspNetCore.Http;

namespace FinancialAudit.Application.DTOs;

public class ImportTransactionRequestDto
{
    public IFormFile File { get; set; }
    public string Format { get; set; }
}