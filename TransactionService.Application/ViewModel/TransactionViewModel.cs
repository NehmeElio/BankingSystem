namespace TransactionService.Application.ViewModel;

public class TransactionViewModel
{
    public string? Username { get; set; }

    public string? TransactionType { get; set; }

    public decimal? Amount { get; set; }

    public DateOnly? TransactionDate { get; set; }
    
}