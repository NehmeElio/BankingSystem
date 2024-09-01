using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransactionService.Application.Caching;
using TransactionService.Domain.Models;
using TransactionService.Persistence.Context;
using TransactionService.Persistence.Interfaces;

namespace TransactionService.Infrastructure.Services;

public class AccountCreationService : AccountCreation.AccountCreationBase
{
    // Inject dependencies such as a database context or service for handling account logic
    private readonly BankContext _context;
    private readonly ILogger<AccountCreationService> _logger;
    private readonly ILocalStorage _localStorage;
    private readonly ITenantService _tenantService;

    public AccountCreationService(BankContext context, ILogger<AccountCreationService> logger, ILocalStorage localStorage, ITenantService tenantService)
    {
        _context = context;
        _logger = logger;
        _localStorage = localStorage;
        _tenantService = tenantService;
    }

    public override async Task<CreateAccountResponse> CreateAccount(CreateAccountRequest request, ServerCallContext context)
    {
        _tenantService.SetTenant(request.Branch,"postgres");
        
        var customerId=await _context.Customers
            .Where(c=>c.FirstName==request.FirstName && c.LastName==request.LastName)
            .Select(c=>c.CustomerId)
            .FirstOrDefaultAsync();
        
        Account account = new Account
        {
            CustomerId = customerId,
            Balance = (decimal?)request.Balance,
            CreatedDate = DateOnly.FromDateTime(DateTime.Now),
            Email = request.Email,
            Username = request.Username
        };
        
        await _context.Accounts.AddAsync(account);
        
        var success = await _context.SaveChangesAsync() > 0;
        _logger.LogInformation("New account added to database: {Success}", success);

        var customer = await _context.Customers.FindAsync(customerId);
        if (customer != null)
        {
            customer.NumberOfAccounts += 1;
            await _context.SaveChangesAsync();
        }
        
        return new CreateAccountResponse
        {
            IsSuccess = success
        };
    }

    public override async Task<CreateCustomerResponse> CreateCustomer(CreateCustomerRequest request, ServerCallContext context)
    {
        _tenantService.SetTenant(request.Branch,"postgres");

        Customer customer = new Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            NumberOfAccounts = request.NumberOfAccounts,
        };

        await _context.Customers.AddAsync(customer);
        var success = await _context.SaveChangesAsync() > 0;
        
        return new CreateCustomerResponse
        {
            IsSuccess = success
        };
    }

    public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        _tenantService.SetTenant(request.Branch,"postgres");
        
        _localStorage.Roles.TryGetValue(request.Role,out var roleId);

        var branchId = _context.Branches.Where(b => b.BranchName == request.Branch).Select(b => b.BranchId)
            .FirstOrDefault();
       
        User user = new User
        {
            Username = request.Username,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            RoleId = (int?)roleId,
            BranchId = branchId
            
        };

        await _context.Users.AddAsync(user);
        var success=await _context.SaveChangesAsync()>0;
        
        return new CreateUserResponse
        {
            IsSuccess = success
        };
    }
}