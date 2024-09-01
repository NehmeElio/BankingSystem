namespace BankingSystem.SharedLibrary.Settings;

public class AuthenticationSettings
{
    public string? Authority { get; set; }
    
    public string? ServerUrl { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? KeyCloakUrl{ get; set; }
    public string? Audience { get; set; }
    
    public string? AdminUsername { get; set; }
    
    public string? AdminPassword { get; set; }
    
    public string? Realm { get; set; }
}