namespace TransactionService.Domain.Models;

public partial class IntervalType
{
    public int IntervalTypeId { get; set; }

    public string? IntervalTypeName { get; set; }

    public virtual ICollection<RecurrentTransaction> RecurrentTransactions { get; set; } = new List<RecurrentTransaction>();
}
