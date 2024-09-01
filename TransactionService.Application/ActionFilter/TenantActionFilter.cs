using System.Security.Authentication;
using BankingSystem.SharedLibrary.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using TransactionService.Persistence.Interfaces;

namespace TransactionService.Application.ActionFilter;

public class TenantActionFilter(
    ILogger<TenantActionFilter> logger,
    ITenantService tenantService)
    : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        
        foreach (var claim in httpContext.User.Claims)
        {
            logger.LogInformation($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
        }

        logger.LogInformation(httpContext.User.Claims.FirstOrDefault(c => c.Type == "exp")?.Value);

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
            logger.LogWarning("Realm access claim roles not found.");
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
                tenantService.SetBranch(branchRole);
                if(roles.Contains("employee"))  tenantService.SetUser($"{branchRole}_employee");
                else tenantService.SetUser($"{branchRole}_customer");
                
                logger.LogInformation($"Will be logging with User: {tenantService.GetTenant()?.User} and Branch: {tenantService.GetTenant()?.Branch}" );
               
                
            }

        }
        else if (roles.Contains("admin"))
        {
            
            logger.LogInformation("Admin is logging with the default user");
        }
        else
        {
            throw new AuthenticationException("User role not found.");
        }

        await next();
    }
}