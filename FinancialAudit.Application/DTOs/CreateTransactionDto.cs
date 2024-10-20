namespace FinancialAudit.Application.DTOs;

public record CreateTransactionDto
{
    public required decimal Amount { get; init; }
    public required string Type { get; init; }
    public required int UserId { get; init; }
}