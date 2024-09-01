using AccountService.Application.Commands;
using AccountService.Domain.Models;
using AccountService.Infrastructure.Interfaces;
using AccountService.Persistence.Context;
using AccountService.Persistence.Interfaces;
using AccountService.Persistence.Models;
using AccountService.Persistence.Services;
using BankingSystem.SharedLibrary.Exceptions;
using BankingSystem.SharedLibrary.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AccountService.Application.Handler;

public class CreateAccountHandler: IRequestHandler<CreateAccountCommand>
{
    private readonly BankContext _context;
    private readonly ILogger<CreateAccountHandler> _logger;
    private readonly IAddKeycloakAccountService _addKeycloakAccountService;
    private readonly ITenantService _tenantService;
    private readonly IGrpcService _grpcService;
    
    //private readonly IValidator<Customer> _customerValidator

    public CreateAccountHandler(BankContext context, ILogger<CreateAccountHandler> logger, IAddKeycloakAccountService addKeycloakAccountService, ITenantService tenantService, IGrpcService grpcService)
    {
        _context = context;
        _logger = logger;
        _addKeycloakAccountService = addKeycloakAccountService;
        _tenantService = tenantService;
        _grpcService = grpcService;
    }

    public async Task Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var branchName = _tenantService.GetTenant()?.Branch;
        
        var firstName = request.AddAccountDto.FirstName;
        var lastName = request.AddAccountDto.LastName;
        if(firstName == null || lastName == null) throw new MissingInformationException("Missing Information for Account Creation (Firstname or Lastname)");
        
        if (!(await CustomerExists(firstName, lastName)))
        {
            Customer customerToAdd = new Customer
            {
                FirstName = firstName,
                LastName = lastName,
                NumberOfAccounts = 0,
            };
            await _context.Customers.AddAsync(customerToAdd, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Customer Created");

            var response=await _grpcService.CreateCustomerAsync(new CreateCustomerRequest
            {
                FirstName = customerToAdd.FirstName,
                LastName = customerToAdd.LastName,
                NumberOfAccounts = 0,
                Branch = branchName
            });
            
            _logger.LogInformation("Create Customer Response Success: {Response}",response.IsSuccess);
        }

        var accountExists =await _context.Users.AnyAsync(a => a.Username == request.AddAccountDto.Username, cancellationToken: cancellationToken);
        if (accountExists) throw new DuplicateException($"username {request.AddAccountDto.Username} already exists");
        

        var customerId = await _context.Customers.Where(c => c.FirstName == firstName && c.LastName == lastName)
            .Select(c => c.CustomerId).FirstOrDefaultAsync(cancellationToken);
        var customer = _context.Customers.FindAsync(customerId).Result;
        
        if (customer != null)
        {
            customer.NumberOfAccounts = (customer.NumberOfAccounts ?? 0) + 1;
            if(customer.NumberOfAccounts>5) throw new MaxAccountException("Customer cannot have more than 5 accounts");
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Customer Updated");
        }

        Account account = new Account
        {
            CustomerId = customerId,
            Balance = 0,
            CreatedDate = DateOnly.FromDateTime(DateTime.Now),
            Username = request.AddAccountDto.Username,
            Email = request.AddAccountDto.Email,
        };


        await _context.Accounts.AddAsync(account, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Account for customer created");
        
        await _addKeycloakAccountService.AddAccount(request.AddAccountDto,"customer");

        var response2=await _grpcService.CreateAccountAsync(new CreateAccountRequest
        {
            FirstName = account.Customer?.FirstName,
            LastName = account.Customer?.LastName,
            Balance = 0,
            Username = account.Username,
            Email = account.Email,
            Branch=branchName
        });
        
        _logger.LogInformation("Create Customer Response Success: {Response}",response2.IsSuccess);

       
  
        var userToAdd = new User
        {
            Username = request.AddAccountDto.Username,
            Email = request.AddAccountDto.Email,
            FirstName = request.AddAccountDto.FirstName,
            LastName = request.AddAccountDto.LastName,
            RoleId = _context.Roles
                .Where(r => r.RoleName == "customer")
                .Select(r => r.RoleId)
                .FirstOrDefault(),

            BranchId = branchName != null
                ? _context.Branches
                    .Where(b => b.BranchName == branchName)
                    .Select(b => b.BranchId)
                    .FirstOrDefault()
                : (int?)null
        };
        await _context.Users.AddAsync(userToAdd, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Account for user created");

        var response3=await _grpcService.CreateUserAsync(new CreateUserRequest
        {
            FirstName =userToAdd.FirstName,
            LastName = userToAdd.LastName,
            Username = userToAdd.Username,
            Email = userToAdd.Email,
            Role ="customer",
            Branch = branchName
        });
        
        _logger.LogInformation("Create User Response Success: {Response}",response3.IsSuccess);

    }
    private async Task<bool> CustomerExists(string firstname, string lastname)
    {
        return await _context.Customers.AnyAsync(c => c.FirstName == firstname && c.LastName == lastname);
    }

    
}