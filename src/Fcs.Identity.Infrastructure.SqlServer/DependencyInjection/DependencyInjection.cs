using System.Diagnostics.CodeAnalysis;
using Fcs.Identity.Domain.Abstractions;
using Fcs.Identity.Domain.DonorProfiles;
using Fcs.Identity.Domain.ManagerProfiles;
using Fcs.Identity.Infrastructure.SqlServer.Persistence;
using Fcs.Identity.Infrastructure.SqlServer.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fcs.Identity.Infrastructure.SqlServer.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddSqlServerInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<FcsIdentityDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SqlServer")));

        services.AddScoped<IDonorProfileRepository, DonorProfileRepository>();
        services.AddScoped<IManagerProfileRepository, ManagerProfileRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<FcsIdentityDbContext>());
        services.AddHealthChecks().AddDbContextCheck<FcsIdentityDbContext>("sqlserver");

        return services;
    }
}
