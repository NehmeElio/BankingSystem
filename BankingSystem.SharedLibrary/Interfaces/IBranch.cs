namespace BankingSystem.SharedLibrary.Interfaces;

public interface IBranch
{
    int BranchId { get; set; }

    string? BranchName { get; set; }

    string? Address { get; set; }

    ICollection<IUser> Users { get; set; }
}