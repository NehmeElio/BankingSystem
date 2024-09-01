using System.Security.Authentication;
using AccountService.Persistence.Context;
using AccountService.Persistence.Interfaces;
using BankingSystem.SharedLibrary.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace AccountService.Application.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantMiddleware> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation(context.User.Claims.FirstOrDefault(c => c.Type == "exp")?.Value);

            var realmAccessClaim = context.User.FindFirst( "realm_access")?.Value;

            if (realmAccessClaim == null)
            {
                throw new NotFoundException("Realm access claim not found.");
            }

            var roles = System.Text.Json.JsonSerializer.Deserialize<List<string>>(realmAccessClaim);
            if (roles == null)
            {
                throw new NotFoundException("Realm access claim roles not found.");
            }

            var username = context.User.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
            if (username == null || roles == null) throw new NotFoundException("Username or roles not found.");

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BankContext>();
                var tenantService = scope.ServiceProvider.GetRequiredService<ITenantService>();

                if (roles.Contains("employee") || roles.Contains("customer"))
                {
                    var branchId = dbContext.Users.Where(u => u.Username == username).Select(u => u.Branch).FirstOrDefault();
                    if (branchId != null)
                    {
                        var branchName = (await dbContext.Branches.FindAsync(branchId))?.BranchName;
                        if (branchName != null)
                        {
                            tenantService.SetBranch(branchName);
                            _logger.LogInformation("Branch set to {Branch}", branchName);
                        }
                    }

                    var roleId = dbContext.Users.Where(u => u.Username == username).Select(u => u.Role).FirstOrDefault();
                    if (roleId != null)
                    {
                        var roleName = (await dbContext.Roles.FindAsync(roleId))?.RoleName;
                        if (roleName != null)
                        {
                            tenantService.SetUser(roleName);
                            _logger.LogInformation("Role/User set to {Role}", roleName);
                        }
                    }
                }
                else if (roles.Contains("admin"))
                {
                    tenantService.SetUser("postgres");
                    _logger.LogInformation("Role/User set to {Role}", "postgres");
                }
                else
                {
                    throw new AuthenticationException("User role not found.");
                }
            }

            await _next(context);
        }
    }
}
