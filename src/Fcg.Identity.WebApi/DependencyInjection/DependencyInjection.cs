using System.Text.Json.Serialization;
using Asp.Versioning;
using Fcg.Identity.Infrastructure.SqlServer.Persistence;
using Fcg.Identity.WebApi.Filters;
using Fcg.Identity.WebApi.Observability;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.OpenTelemetry;
namespace Fcg.Identity.WebApi.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddWebApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
               .AddJsonOptions(options =>
               {
                   options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
               });

        services.AddEndpointsApiExplorer();
        services.AddHttpContextAccessor();
        services.AddSwaggerConfiguration(configuration);

        services.AddVersioning();
        services.AddFilters();
        services.AddHealthChecks().AddDbContextCheck<FcgIdentityDbContext>();
        services.AddRouting(options => options.LowercaseUrls = true);
        services.AddObservability(configuration);
        services.AddSerilogLogging(configuration);
        return services;
    }

    private static void AddSwaggerConfiguration(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "FCG.Identity - V1",
                Version = "v1.0"
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
            });
        });
    }

    private static void AddVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
    }

    private static void AddFilters(this IServiceCollection services)
    {
        services.AddMvc(options =>
        {
            options.Filters.Add<TrimStringsActionFilter>();
        });
    }

    private static void AddObservability(this IServiceCollection services, IConfiguration configuration)
    {
        var options = new ObservabilityOptions();
        configuration.GetSection(ObservabilityOptions.SectionName).Bind(options);

        var environment = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Production";

        var resourceBuilder = ObservabilityTelemetry.CreateResourceBuilder(options, environment);

        services.AddOpenTelemetry()
            .WithTracing(builder => builder.ConfigureTracing(options, resourceBuilder))
            .WithMetrics(builder => builder.ConfigureMetrics(options, resourceBuilder));
    }

    private static void AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
    {
        var options = new ObservabilityOptions();
        configuration.GetSection(ObservabilityOptions.SectionName).Bind(options);

        var environment = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Production";

        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithProperty("Application", "FCG.Identity")
            .Enrich.WithProperty("Environment", environment)
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}");

        if (options.EnableOtlpExporter && !string.IsNullOrEmpty(options.OtlpEndpoint))
        {
            loggerConfig.WriteTo.OpenTelemetry(otlpOptions =>
            {
                otlpOptions.Endpoint = $"{options.OtlpEndpoint}/otlp/v1/logs";
                otlpOptions.Protocol = OtlpProtocol.HttpProtobuf;
                otlpOptions.Headers = new Dictionary<string, string>
                {
                    ["Authorization"] = options.OtlpAuthHeader
                };
                otlpOptions.ResourceAttributes = new Dictionary<string, object>
                {
                    ["service.name"] = options.ServiceName,
                    ["deployment.environment"] = environment
                };
            });
        }

        Log.Logger = loggerConfig.CreateLogger();

        Log.Information("Starting {Application} application", "FCG.Identity");
        Log.Information("Environment: {Environment}", environment);

        if (options.EnableOtlpExporter)
        {
            Log.Information("OTLP exporter enabled — sending telemetry to {Endpoint}", options.OtlpEndpoint);
        }
        else
        {
            Log.Information("OTLP exporter disabled — telemetry is console-only");
        }

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog();
        });
    }
}
