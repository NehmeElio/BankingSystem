namespace AccountService.Persistence.Interfaces;

public interface IModifyConnectionService
{
    string ModifyConnectionString( string? user, string? branch);
}