namespace FinancialAudit.Domain.Entities;

public record User
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public decimal Balance { get; set; }

    public ICollection<Transaction>? Transactions { get; init; }
}