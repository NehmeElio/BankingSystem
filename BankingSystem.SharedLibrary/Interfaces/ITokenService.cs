

namespace BankingSystem.SharedLibrary.Interfaces;

public interface ITokenService
{
    Task<string> GetTokenAsync(string username, string password);

}