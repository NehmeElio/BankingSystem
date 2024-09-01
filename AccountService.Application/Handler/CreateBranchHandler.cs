using AccountService.Application.Commands;

using AccountService.Domain.Models;
using AccountService.Infrastructure.Interfaces;
using AccountService.Persistence.Context;
using AccountService.Persistence.Interfaces;
using AccountService.Persistence.Models;
using BankingSystem.SharedLibrary.DTO;
using BankingSystem.SharedLibrary.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AccountService.Application.Handler;

public class CreateBranchHandler(
    IBranchSchemaService branchSchemaService,
    BankContext context,
    ILogger<CreateBranchHandler> logger,
    IValidator<CreateBranchDto> validator,
    IRabbitMqSenderService<CreateBranchDto> rabbitMqSenderService,
    IAddKeycloakAccountService addKeycloakAccountService)
    : IRequestHandler<CreateBranchCommand, Unit>
{
    public async Task<Unit> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        
        var newBranch = request.NewBranch;
        var sourceSchemaName = request.SourceSchemaName;
        

        var validationResult = await validator.ValidateAsync(newBranch,cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogError("Validation Failed");
            throw new ValidationException(validationResult.Errors);
        }

        var newBranchName = newBranch.BranchName;
        var newBranchAddress = newBranch.Address;
        
        var branchExists=await context.Branches.AnyAsync(x=>x.BranchName==newBranchName, cancellationToken: cancellationToken);
        
        if(branchExists) throw new DuplicateException("The branch name "+newBranchName+" already exists");
        
        if (newBranchName != null)
        {
            branchSchemaService.CreateBranchSchema(newBranchName, sourceSchemaName);
            logger.LogInformation("Branch Created");
            
            context.Branches.Add(new Branch()
            {
                BranchName = newBranchName,
                Address = newBranchAddress
            });


            
            rabbitMqSenderService.PublishMessage(newBranch);
            logger.LogInformation("rabbitmq branch info sent");
        }

        logger.LogInformation("Branch and Roles Added to Database Table");
        
        await context.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Branch Saved to Database");

        if (newBranchName != null) await addKeycloakAccountService.AddBranchRole(newBranchName);
        
        logger.LogInformation("Added Role Realm to Keycloak");


        return Unit.Value;
    }
}