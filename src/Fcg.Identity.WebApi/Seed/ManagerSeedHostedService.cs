using Fcg.Identity.Application.Seed;
using Microsoft.Extensions.DependencyInjection;

namespace Fcg.Identity.WebApi.Seed;

public sealed class ManagerSeedHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ManagerSeedHostedService> _logger;

    public ManagerSeedHostedService(
        IServiceProvider serviceProvider,
        ILogger<ManagerSeedHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var managerSeeder = scope.ServiceProvider.GetRequiredService<IManagerSeeder>();

        _logger.LogInformation("Manager seed hosted service started.");
        await managerSeeder.SeedAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

}
