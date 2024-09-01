namespace TransactionService.Application.ViewModel;

public class AccountViewModel
{

    public decimal? Balance { get; set; }

    public DateOnly? CreatedDate { get; set; }

    public string? Email { get; set; }

    public string? Username { get; set; }
    
}