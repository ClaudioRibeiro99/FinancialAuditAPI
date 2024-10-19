namespace FinancialAuditApi.DTOs;

public record CreateTransactionDto
{
    public required decimal Amount { get; set; }
    public required string Type { get; set; } // Deposit, Withdrawal, Purchase
    public required int UserId { get; set; }
}