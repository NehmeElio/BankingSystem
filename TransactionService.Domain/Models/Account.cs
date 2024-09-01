namespace TransactionService.Domain.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public int? CustomerId { get; set; }

    public decimal? Balance { get; set; }

    public DateOnly? CreatedDate { get; set; }

    public string? Email { get; set; }

    public string? Username { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<RecurrentTransaction> RecurrentTransactions { get; set; } = new List<RecurrentTransaction>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
