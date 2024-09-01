using Microsoft.AspNetCore.Authorization;

namespace BankingSystem.SharedLibrary.Models;

public class RolesRequirement : IAuthorizationRequirement
{
    public List<string> RequiredRoles { get; }

    public RolesRequirement(List<string> requiredRoles)
    {
        RequiredRoles = requiredRoles;
    }
}
