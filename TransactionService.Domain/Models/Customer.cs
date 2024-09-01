namespace TransactionService.Domain.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public int? NumberOfAccounts { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
