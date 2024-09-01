using AccountService.Persistence.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace AccountService.Persistence.Services;

public class ModifyConnectionService(IConfiguration configuration) : IModifyConnectionService
{
    public string ModifyConnectionString(string? user, string? branch)
    {
       var connectionString = configuration.GetConnectionString("DefaultConnection");
       
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