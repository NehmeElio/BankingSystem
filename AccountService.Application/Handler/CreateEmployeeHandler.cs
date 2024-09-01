using AccountService.Application.Caching;
using AccountService.Application.Commands;
using AccountService.Infrastructure.Interfaces;
using AccountService.Persistence.Context;
using AccountService.Persistence.Models;
using BankingSystem.SharedLibrary.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AccountService.Application.Handler;

public class CreateEmployeeHandler:IRequestHandler<CreateEmployeeCommand>
{
    private readonly BankContext _bankContext;
    private readonly ILogger<CreateEmployeeHandler> _logger;
    private readonly IAddKeycloakAccountService _addKeycloakAccountService;
    private readonly ILocalStorage _localStorage;
    private readonly IGrpcService _grpcService;


    public CreateEmployeeHandler(BankContext bankContext, ILogger<CreateEmployeeHandler> logger, IAddKeycloakAccountService addKeycloakAccountService, ILocalStorage localStorage, IGrpcService grpcService)
    {
        _bankContext = bankContext;
        _logger = logger;
        _addKeycloakAccountService = addKeycloakAccountService;
        _localStorage = localStorage;
        _grpcService = grpcService;
    }

    public async Task Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        AddAccountDto addAccountDto = new AddAccountDto
        {
            FirstName = request.AddUserDto.FirstName,
            LastName = request.AddUserDto.LastName,
            Username = request.AddUserDto.Username,
            Password = request.AddUserDto.Password,
            Email = request.AddUserDto.Email
        };
        await _addKeycloakAccountService.AddAccount(addAccountDto, "employee", request.AddUserDto.Branch);
        _logger.LogInformation($"Keycloak account for {request.AddUserDto.Username} created");
        
        
        User user = new User
        {
            Username = request.AddUserDto.Username,
            Email = request.AddUserDto.Email,
            FirstName = request.AddUserDto.FirstName,
            LastName = request.AddUserDto.LastName,
            RoleId = _localStorage.Roles.First(r => r.Key == "employee").Value,
            BranchId =await _bankContext.Branches
                .Where(b=>b.BranchName==request.AddUserDto.Branch)
                .Select(b=>b.BranchId).FirstOrDefaultAsync(cancellationToken: cancellationToken)
        };

        await _bankContext.Users.AddAsync(user, cancellationToken);
        await _bankContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"User {request.AddUserDto.Username} created and added to database");
        
        var response=await _grpcService.CreateUserAsync(new CreateUserRequest
        {
            FirstName = request.AddUserDto.FirstName,
            LastName = request.AddUserDto.LastName,
            Username = request.AddUserDto.Username,
            Email = request.AddUserDto.Email,
            Role = "employee",
            Branch = request.AddUserDto.Branch
        });
        
        _logger.LogInformation("Create User Response Success: {Response}",response.IsSuccess);
        
        
    }
}