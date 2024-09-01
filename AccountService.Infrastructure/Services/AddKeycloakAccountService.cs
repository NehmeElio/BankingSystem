
using AccountService.Infrastructure.Interfaces;
using AccountService.Persistence.Interfaces;
using BankingSystem.SharedLibrary.DTO;
using BankingSystem.SharedLibrary.Settings;
using FluentValidation;
using Keycloak.Net;
using Keycloak.Net.Models.Roles;
using Keycloak.Net.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AccountService.Infrastructure.Services;

public class AddKeycloakAccountService:IAddKeycloakAccountService
{
    private readonly KeycloakClient _keycloakClient;
    private readonly AuthenticationSettings _authenticationSettings;
    private readonly IValidator<User> _validator;
    private readonly ILogger<AddKeycloakAccountService> _logger;
    private readonly ITenantService _tenantService;
    


    public AddKeycloakAccountService( AuthenticationSettings authenticationSettings, IValidator<User> validator, ILogger<AddKeycloakAccountService> logger, ITenantService tenantService)
    {
        _authenticationSettings = authenticationSettings;
        _validator = validator;
        _logger = logger;
        _tenantService = tenantService;
        _keycloakClient = new KeycloakClient(authenticationSettings.ServerUrl,
            authenticationSettings.AdminUsername,authenticationSettings.AdminPassword);
        
    }

    public async Task<bool> AddAccount(AddAccountDto addAccountDto,string role,string? branch=null)
    {
        User user = new User
        {
            UserName = addAccountDto.Username,
            FirstName = addAccountDto.FirstName,
            LastName = addAccountDto.LastName,
            Email = addAccountDto.Email,
            Enabled = true,
            EmailVerified = false,
            Credentials = new List<Credentials>
            {
                new()
                {
                    Type = "password",
                    Value = addAccountDto.Password // Password for the user
                }
            },
            
            //Groups = new List<string> { "branch" } // Assign to a group, if applicable
        };


        await _validator.ValidateAndThrowAsync(user);
        
        var userId = await _keycloakClient.CreateAndRetrieveUserIdAsync(_authenticationSettings.Realm, user);
        var rolesAv=await _keycloakClient.GetAvailableRealmRoleMappingsForUserAsync(_authenticationSettings.Realm, userId);
        var roleToAdd = rolesAv.FirstOrDefault(r => r.Name == role);
        
        _logger.LogInformation($"Role {role} added to user {addAccountDto.Username}");

        if (roleToAdd != null)
        {
            var branchRole=branch!=null?rolesAv.FirstOrDefault(r => r.Name == branch)
                    :rolesAv.FirstOrDefault(r => r.Name == _tenantService.GetTenant().Branch);
            if (branchRole != null)
            {
                List<Role> roles = new List<Role>()
                {
                    roleToAdd,
                    branchRole
                };
           
          
                /*_logger.LogInformation($"Role: Id={roleToAdd.Id}, Name={roleToAdd.Name}, Description={roleToAdd.Description}, ClientRole={roleToAdd.ClientRole}, " +
                               $"ContainerId={roleToAdd.ContainerId}, Attributes=[{string.Join(", ", roleToAdd.Attributes.Select(kvp => $"{kvp.Key}={kvp.Value}"))}]");*/
                //await _keycloakClient.CreateRoleAsync(_authenticationSettings.Realm, branchRole);
                //bug in keycloak api since 2016 cant add realm roles in one endpoint
                await _keycloakClient.AddRealmRoleMappingsToUserAsync(_authenticationSettings.Realm, userId, roles);
            }
        }


        return true;
    }

    public async Task AddBranchRole(string branch)
    {
        Role branchRole = new Role
        {
            Name = branch,
            ClientRole = false
        };
        await _keycloakClient.CreateRoleAsync(_authenticationSettings.Realm, branchRole);
    }
    
    
}