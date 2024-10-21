namespace FinancialAudit.Application.DTOs;

public record ImportResultDto
{
    public int SuccessCount { get; set; }
    public string Message { get; set; } = string.Empty;
}