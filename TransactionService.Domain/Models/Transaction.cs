namespace TransactionService.Domain.Models;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public int? AccountId { get; set; }

    public int? TransactionTypeId { get; set; }

    public decimal? Amount { get; set; }

    public DateOnly? TransactionDate { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual Account? Account { get; set; }

    public virtual TransactionType? TransactionType { get; set; }
}
