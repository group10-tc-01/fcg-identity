using Fcg.Identity.Domain.Abstractions;
using Fcg.Identity.Domain.AuditLogs;
using Fcg.Identity.Infrastructure.SqlServer.Persistence;
using Fcg.Identity.IntegratedTests.Configurations;
using Fcg.Identity.IntegratedTests.Support;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Fcg.Identity.IntegratedTests.Infrastructure.SqlServer.Persistence.Repositories;

[Collection(IntegrationTestCollection.Name)]
public sealed class AuditLogRepositoryTests : IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;

    public AuditLogRepositoryTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public Task InitializeAsync()
    {
        return _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [DockerAvailableFact]
    public async Task Given_AddAsync_Called_When_AuditLogIsSaved_Then_ShouldPersistAuditLog()
    {
        // Arrange
        var auditLog = AuditLog.Create(AuditActions.DonorRegistered, "DonorProfile", actorType: "Doador").Value;
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAuditLogRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var dbContext = scope.ServiceProvider.GetRequiredService<FcgIdentityDbContext>();

        // Act
        await repository.AddAsync(auditLog, CancellationToken.None);
        await unitOfWork.SaveChangesAsync(CancellationToken.None);
        var result = await dbContext.AuditLogs.FindAsync([auditLog.Id], CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Action.Should().Be(AuditActions.DonorRegistered);
        result.EntityName.Should().Be("DonorProfile");
    }
}
