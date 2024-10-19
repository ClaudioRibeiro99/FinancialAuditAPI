namespace FinancialAudit.Domain.Entities;

public record User
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Balance { get; set; }

    public ICollection<Transaction>? Transactions { get; set; }
}