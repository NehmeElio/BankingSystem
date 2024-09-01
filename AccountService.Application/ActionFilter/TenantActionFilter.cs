
using System.Security.Authentication;
using System.Security.Claims;

using AccountService.Persistence.Context;
using AccountService.Persistence.Interfaces;
using BankingSystem.SharedLibrary.Exceptions;

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace AccountService.Application.ActionFilter;

public class TenantActionFilter : IAsyncActionFilter
{
    private readonly ILogger<TenantActionFilter> _logger;
    private readonly ITenantService _tenantService;

    public TenantActionFilter(
        ILogger<TenantActionFilter> logger,
        ITenantService tenantService)
    {
        _logger = logger;
        _tenantService = tenantService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        
        foreach (Claim claim in httpContext.User.Claims)
        {
            _logger.LogInformation($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
        }

        _logger.LogInformation(httpContext.User.Claims.FirstOrDefault(c => c.Type == "exp")?.Value);

        /*var realmAccessClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "realm_access")?.Value;

        if (realmAccessClaim == null)
        {
            throw new NotFoundException("Realm access claim not found.");
        }*/

        var roles = httpContext.User.Claims
            .Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
            .Select(c => c.Value)
            .ToList();
        if (roles.Count == 0)
        {
            _logger.LogWarning("Realm access claim roles not found.");
        }

        var username = httpContext.User.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
        if (username == null )
        {
            throw new NotFoundException("Username not found.");
        }

        if (roles.Contains("employee") || roles.Contains("customer"))
        {
            /*var branch = _dbContext.Users.Where(u => u.Username == username).Select(u => u.Branch).FirstOrDefault();
            if (branch != null)
            {
                var branchName = branch.BranchName;
                if (branchName != null)
                {
                    _tenantService.SetBranch(branchName);
                    _logger.LogInformation("Branch set to {Branch}", branchName);

                }

                var roleId = _dbContext.Users.Where(u => u.Username == username).Select(u => u.RoleId).FirstOrDefault();
                if (roleId != null)
                {
                    var roleName = (await _dbContext.Roles.FindAsync(roleId))?.RoleName;
                    if (roleName != null)
                    {
                        _tenantService.SetUser(roleName);
                        _logger.LogInformation("Role/User set to {Role}", roleName);
                    }
                }
            }*/
            var branchRole = roles.FirstOrDefault(r => r.EndsWith("branch"));
            if (branchRole != null)
            {
                _tenantService.SetBranch(branchRole);
                if(roles.Contains("employee"))  _tenantService.SetUser($"{branchRole}_employee");
                else _tenantService.SetUser($"{branchRole}_customer");
                
                _logger.LogInformation($"Will be logging with User: {_tenantService.GetTenant()?.User} and Branch: {_tenantService.GetTenant()?.Branch}" );
               
                
            }

        }
        else if (roles.Contains("admin"))
        {
            
            _logger.LogInformation("Admin is logging with the default user");
        }
        else
        {
            throw new AuthenticationException("User role not found.");
        }

        await next();
    }
}