using BankingSystem.SharedLibrary.DTO;

namespace AccountService.Infrastructure.Interfaces;

public interface IAddKeycloakAccountService
{
    Task<bool> AddAccount(AddAccountDto addAccountDto,string role,string? branch=null);
    Task AddBranchRole(string branch);
}