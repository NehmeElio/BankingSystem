namespace BankingSystem.SharedLibrary.Interfaces;

public interface IRole
{
    int RoleId { get; set; }

    string? RoleName { get; set; }

    ICollection<IUser> Users { get; set; }
}