namespace FinancialAudit.Domain.Entities;

public record Transaction
{
    public Guid Id { get; init; }
    public decimal Amount { get; init; }
    public DateTime Date { get; init; }
    public TransactionType Type { get; init; }
    public Guid UserId { get; init; }
    public User? User { get; init; }
    public bool IsImported { get; set; }
}