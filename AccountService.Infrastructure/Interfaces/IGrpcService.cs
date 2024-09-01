namespace AccountService.Infrastructure.Interfaces;

public interface IGrpcService
{
    Task<CreateAccountResponse> CreateAccountAsync(CreateAccountRequest request);
    Task<CreateCustomerResponse> CreateCustomerAsync(CreateCustomerRequest request);
    Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request);
}