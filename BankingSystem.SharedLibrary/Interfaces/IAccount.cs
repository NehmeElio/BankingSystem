namespace BankingSystem.SharedLibrary.Interfaces;

public interface IAccount
{
    public int AccountId { get; set; }
    
    public int CustomerId { get; set; }
    
    public decimal? Balance { get; set; }
    
    public DateOnly? CreatedDate { get; set; }
    
    public ICustomer AccountNavigation { get; set; }
}