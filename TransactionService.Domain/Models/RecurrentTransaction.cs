namespace TransactionService.Domain.Models;

public partial class RecurrentTransaction
{
    public int RecurrentTransactionId { get; set; }

    public int? AccountId { get; set; }

    public decimal? Balance { get; set; }

    public DateOnly? CreatedDate { get; set; }

    public int? IntervalTypeId { get; set; }

    public int? IntervalValue { get; set; }

    public DateOnly? DisabledDate { get; set; }
    
    public DateOnly? NextExecutionDate { get; set; }
    
    public DateOnly? LastExecutionDate { get; set; }
    
    public int? Version { get; set; }

    public virtual Account? Account { get; set; }

    public virtual IntervalType? IntervalType { get; set; }
}
