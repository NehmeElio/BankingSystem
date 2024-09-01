using System.ComponentModel.DataAnnotations;

namespace BankingSystem.SharedLibrary.DTO;

public class AddUserDto
{
    public required string Username { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Branch { get; set; }
    public string? Password { get; set; }
}