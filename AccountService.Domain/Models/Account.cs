using AccountService.Persistence.Models;

namespace AccountService.Domain.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public int? CustomerId { get; set; }

    public decimal? Balance { get; set; }

    public DateOnly? CreatedDate { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public virtual Customer? Customer { get; set; }
}
