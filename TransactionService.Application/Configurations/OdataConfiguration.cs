
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using TransactionService.Domain.Models;

namespace TransactionService.Application.Configurations;

public static class OdataConfiguration
{
    public static void AddODataServices(this IServiceCollection services)
    {
        services.AddControllers().AddOData(opt =>
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<Transaction>("Transactions");

            opt.AddRouteComponents("odata", GetEdmModel(builder))
                .Select()
                .Filter()
                .OrderBy()
                .Count()
                .SetMaxTop(100);
        });
    }

    private static IEdmModel GetEdmModel(ODataConventionModelBuilder builder)
    {
        return builder.GetEdmModel();
    }
}