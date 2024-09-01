namespace BankingSystem.SharedLibrary.Interfaces;

public interface ICustomer
{
    public int CustomerId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public int? NumberOfAccounts { get; set; }

    public IAccount? Account { get; set; }
}