using System.Diagnostics.CodeAnalysis;
using Fcs.Identity.Application.DependencyInjection;
using Fcs.Identity.Infrastructure.Kafka.DependencyInjection;
using Fcs.Identity.Infrastructure.Keycloak.DependencyInjection;
using Fcs.Identity.Infrastructure.SqlServer.DependencyInjection;
using Fcs.Identity.WebApi.DependencyInjection;

namespace Fcs.Identity.WebApi;

[ExcludeFromCodeCoverage]
public class Program
{
    protected Program() { }

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddWebApi(builder.Configuration);
        builder.Services.AddApplication();
        builder.Services.AddSqlServerInfrastructure(builder.Configuration);
        builder.Services.AddKafkaInfrastructure(builder.Configuration);
        builder.Services.AddKeycloakInfrastructure(builder.Configuration);

        var app = builder.Build();
        app.UseWebApiPipeline();
        app.Run();
    }
}
