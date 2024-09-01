using AccountService.Infrastructure.Interfaces;
using BankingSystem.SharedLibrary.Exceptions;
using Grpc.Net.Client;

namespace AccountService.Infrastructure.Services;

public class GrpcService:IGrpcService
{
    private readonly AccountCreation.AccountCreationClient _client;
    

    public GrpcService(string? url)
    {
        if (url != null)
        {
            
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress(url);
            _client = new AccountCreation.AccountCreationClient(channel);
        }
        else
        {
            throw new IncorrectSettingsException("Grpc Server Url not set in appsettings.json");
        }
    }

    public async Task<CreateAccountResponse> CreateAccountAsync(CreateAccountRequest request)
    {
        return await _client.CreateAccountAsync(request);
    }

    public async Task<CreateCustomerResponse> CreateCustomerAsync(CreateCustomerRequest request)
    {
        return await _client.CreateCustomerAsync(request);
    }

    public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request)
    {
        return await _client.CreateUserAsync(request);
    }
}