using System.Diagnostics.CodeAnalysis;
using Fcg.Identity.WebApi.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Fcg.Identity.WebApi.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class PipelineDependencyInjection
{
    public static WebApplication UseWebApiPipeline(this WebApplication app)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Application started successfully");
        logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);

        if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
        {
            app.ApplyMigrations();
            logger.LogInformation("Migrations applied");
        }

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseGlobalCorrelationId();
        app.UseRequestFlowLogging();
        app.UseCustomerExceptionHandler();

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            AllowCachingResponses = false,
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
            }

        });
        app.MapPrometheusScrapingEndpoint("/metrics");

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }

}
