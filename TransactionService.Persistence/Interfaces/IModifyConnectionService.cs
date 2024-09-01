using Microsoft.Extensions.Configuration;

namespace TransactionService.Persistence.Interfaces;

public interface IModifyConnectionService
{
    string ModifyConnectionString( string? user, string? branch);
}