namespace TransactionService.Domain.Models;

public partial class Branch
{
    public int BranchId { get; set; }

    public string? BranchName { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
