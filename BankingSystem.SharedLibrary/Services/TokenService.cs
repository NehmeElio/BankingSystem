using BankingSystem.SharedLibrary.Exceptions;
using BankingSystem.SharedLibrary.Interfaces;
using BankingSystem.SharedLibrary.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BankingSystem.SharedLibrary.Services;

public class TokenService : ITokenService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    private readonly ILogger<TokenService> _logger;
    /*KeycloakUrl = "http://localhost:8080/realms/InMind/protocol/openid-connect/token";
    ClientId= "lab7";
    ClientSecret="ftPrltK1HqTbUXxRlvTrZunff1mZSJhu" ;*/

    public TokenService(HttpClient httpClient, IConfiguration configuration, ILogger<TokenService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }
    public async Task<string> GetTokenAsync(string username, string password)
    {
        var authenticationSettings = new AuthenticationSettings();
        _configuration.GetSection(nameof(AuthenticationSettings)).Bind(authenticationSettings);
        _logger.LogInformation(authenticationSettings.ToString());
        var request = new HttpRequestMessage(HttpMethod.Post, authenticationSettings.KeyCloakUrl);
        if (authenticationSettings is { ClientSecret: not null, ClientId: not null })
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("client_id", authenticationSettings.ClientId),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("client_secret", authenticationSettings.ClientSecret)
            });
        else
        {
            throw new IncorrectSettingsException("Authentication Settings are not set correctly (usually we dont send this to the front" +
                                                 "end and log such errors but i kept it for clarity for now");
        }

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var tokenResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);

            return tokenResponse != null ? tokenResponse["access_token"] : "empty token";
        }
        else
        {
            throw new Exception($"Error retrieving token: {content}");
        }
    }
    
    
}