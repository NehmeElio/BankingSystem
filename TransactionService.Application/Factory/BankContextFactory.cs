using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TransactionService.Application.Interfaces;
using TransactionService.Persistence.Context;
using TransactionService.Persistence.Interfaces;

namespace TransactionService.Application.Factory
{
    public class BankContextFactory : IBankContextFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public BankContextFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public BankContext CreateDbContext(string schema)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BankContext>();

            // Resolve the scoped services from the service provider
            using (var scope = _serviceProvider.CreateScope())
            {
                var modifyConnectionService = scope.ServiceProvider.GetRequiredService<IModifyConnectionService>();
                var tenantService = scope.ServiceProvider.GetRequiredService<ITenantService>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<BankContext>>();
                tenantService.SetBranch(schema);
                /*var modifiedConnectionString = modifyConnectionService.ModifyConnectionString("postgres", schema);
                optionsBuilder.UseNpgsql(modifiedConnectionString);*/

                return new BankContext(optionsBuilder.Options, tenantService, modifyConnectionService, logger);
            }
        }
    }
}