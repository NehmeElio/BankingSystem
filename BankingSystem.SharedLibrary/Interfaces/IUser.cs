namespace BankingSystem.SharedLibrary.Interfaces;

public interface IUser
{
    int UserId { get; set; }

    string? Username { get; set; }

    string? Email { get; set; }

    string? FirstName { get; set; }

    string? LastName { get; set; }

    int? RoleId { get; set; }

    int? BranchId { get; set; }

    IBranch? Branch { get; set; }

    IRole? Role { get; set; }
}