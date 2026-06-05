using Fcs.Identity.Domain.Abstractions;
using Fcs.Identity.Domain.ManagerProfiles;
using Fcs.Identity.IntegratedTests.Configurations;
using Fcs.Identity.IntegratedTests.Support;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Fcs.Identity.IntegratedTests.Infrastructure.SqlServer.Persistence.Repositories;

[Collection(IntegrationTestCollection.Name)]
public sealed class ManagerProfileRepositoryTests : IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;

    public ManagerProfileRepositoryTests(CustomWebApplicationFactory factory)
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
    public async Task Given_AddAsync_Called_When_ManagerProfileIsSaved_Then_ShouldReturnById()
    {
        // Arrange
        var managerProfile = CreateManagerProfile();
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IManagerProfileRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        // Act
        await repository.AddAsync(managerProfile, CancellationToken.None);
        await unitOfWork.SaveChangesAsync(CancellationToken.None);
        var result = await repository.GetByIdAsync(managerProfile.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(managerProfile.Id);
        result.Email.Value.Should().Be(managerProfile.Email.Value);
    }

    [DockerAvailableFact]
    public async Task Given_GetByKeycloakUserIdAsync_Called_When_ManagerProfileExists_Then_ShouldReturnManagerProfile()
    {
        // Arrange
        var managerProfile = CreateManagerProfile();
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IManagerProfileRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        await repository.AddAsync(managerProfile, CancellationToken.None);
        await unitOfWork.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await repository.GetByKeycloakUserIdAsync(managerProfile.KeycloakUserId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.KeycloakUserId.Should().Be(managerProfile.KeycloakUserId);
    }

    [DockerAvailableFact]
    public async Task Given_ExistsByEmailAsync_Called_When_EmailExists_Then_ShouldReturnTrue()
    {
        // Arrange
        var managerProfile = CreateManagerProfile();
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IManagerProfileRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        await repository.AddAsync(managerProfile, CancellationToken.None);
        await unitOfWork.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await repository.ExistsByEmailAsync(managerProfile.Email.Value, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [DockerAvailableFact]
    public async Task Given_ExistsByEmailAsync_Called_When_EmailIsInvalid_Then_ShouldReturnFalse()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IManagerProfileRepository>();

        // Act
        var result = await repository.ExistsByEmailAsync("invalid-email", CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    private static ManagerProfile CreateManagerProfile()
    {
        return ManagerProfile.Create(Guid.NewGuid().ToString(), "Gestor ONG", $"gestor-{Guid.NewGuid():N}@ong.test").Value;
    }
}
