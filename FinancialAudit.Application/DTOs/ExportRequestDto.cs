namespace FinancialAudit.Application.DTOs;

public record ExportRequestDto
{
    public required string Format { get; set; } // "csv" ou "xlsx"
}