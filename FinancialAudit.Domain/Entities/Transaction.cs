namespace FinancialAudit.Domain.Entities;

public record Transaction
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public TransactionType Type { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
}