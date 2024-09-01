using Microsoft.Extensions.Configuration;
using Npgsql;
using TransactionService.Persistence.Interfaces;

namespace TransactionService.Persistence.Services;

public class ModifyConnectionService(IConfiguration configuration) : IModifyConnectionService
{
    public string ModifyConnectionString(string? user, string? branch)
    {
      
       
       var connectionStringBuilder = new NpgsqlConnectionStringBuilder(configuration.GetConnectionString("DefaultConnection"));
       
       if (!string.IsNullOrEmpty(user))
       {
           connectionStringBuilder.Username = user;
       }

       if (!string.IsNullOrEmpty(branch))
       {
           connectionStringBuilder.SearchPath = branch;
       }
       
       return connectionStringBuilder.ToString();
    }
}