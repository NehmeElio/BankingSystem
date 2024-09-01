using System.Security.Claims;
using BankingSystem.SharedLibrary.Models;
using Microsoft.AspNetCore.Authorization;

namespace BankingSystem.SharedLibrary.Helper;

public class RolesAuthorizationHandler : AuthorizationHandler<RolesRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesRequirement requirement)
    {
        var userRoles = GetRoles(context.User);

        if (requirement.RequiredRoles.Any(role => userRoles.Contains(role)))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    private List<string> GetRoles(ClaimsPrincipal user)
    {
        return user.Claims
            .Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
            .Select(c => c.Value)
            .ToList();
    }
}
