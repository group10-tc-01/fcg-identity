using Fcg.Identity.Application.Abstractions.Identity;
using Fcg.Identity.Application.Abstractions.Messaging;
using Fcg.Identity.Application.Audit;
using Fcg.Identity.Application.Seed;
using Fcg.Identity.CommomTestsUtilities.TestDoubles;
using Fcg.Identity.Domain.Abstractions;
using Fcg.Identity.Domain.ManagerProfiles;
using Fcg.Identity.Domain.Shared.Results;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Fcg.Identity.UnitTests.Application.Seed;

public sealed class ManagerSeederTests
{
    [Fact]
    public async Task Given_SeedAsync_When_ManagerSeedIsDisabled_Then_ShouldNotCallIdentityProvider()
    {
        // Arrange
        var dependencies = CreateDependencies(new ManagerSeedSettings { Enabled = false });
        var service = CreateSeeder(dependencies);

        // Act
        await service.SeedAsync(CancellationToken.None);

        // Assert
        dependencies.IdentityProvider.EnsureManagerCalls.Should().Be(0);
        dependencies.UnitOfWork.SaveChangesCalls.Should().Be(0);
        dependencies.MessagePublisher.PublishedMessages.Should().BeEmpty();
    }

    [Fact]
    public async Task Given_SeedAsync_When_RequiredSettingsAreMissing_Then_ShouldThrow()
    {
        // Arrange
        var dependencies = CreateDependencies(new ManagerSeedSettings
        {
            Enabled = true,
            FullName = "",
            Email = "gestor@email.com",
            Password = "StrongPassword123!"
        });
        var service = CreateSeeder(dependencies);

        // Act
        var act = () => service.SeedAsync(CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Manager seed FullName must be configured.");
    }

    [Fact]
    public async Task Given_SeedAsync_When_ProfileDoesNotExist_Then_ShouldCreateManagerProfileAndPublishAudit()
    {
        // Arrange
        var dependencies = CreateDependencies(CreateEnabledSettings());
        dependencies.IdentityProvider.ConfigureEnsureManagerResult(new EnsureManagerIdentityUserResponse("manager-keycloak-user-id"));
        var service = CreateSeeder(dependencies);

        // Act
        await service.SeedAsync(CancellationToken.None);

        // Assert
        dependencies.IdentityProvider.EnsureManagerCalls.Should().Be(1);
        dependencies.ManagerProfileRepository.ManagerProfiles.Should().ContainSingle(profile =>
            profile.KeycloakUserId == "manager-keycloak-user-id" &&
            profile.FullName == dependencies.Settings.FullName &&
            profile.Email.Value == dependencies.Settings.Email);
        dependencies.UnitOfWork.SaveChangesCalls.Should().Be(1);
        var auditEvent = dependencies.MessagePublisher.PublishedMessages.OfType<AuditLogRequestedEvent>().Single();
        auditEvent.Action.Should().Be(AuditActions.ManagerSeeded);
        auditEvent.EntityName.Should().Be(nameof(ManagerProfile));
        auditEvent.Metadata.Should().ContainKey("email");
        auditEvent.Metadata.Should().ContainKey("keycloakUserId");
    }

    [Fact]
    public async Task Given_SeedAsync_When_ProfileExistsByKeycloakUserId_Then_ShouldUpdateManagerProfile()
    {
        // Arrange
        var dependencies = CreateDependencies(CreateEnabledSettings());
        var existingProfile = ManagerProfile.Create("manager-keycloak-user-id", "Nome Antigo", "antigo@email.com").Value;
        await dependencies.ManagerProfileRepository.AddAsync(existingProfile, CancellationToken.None);
        dependencies.IdentityProvider.ConfigureEnsureManagerResult(new EnsureManagerIdentityUserResponse("manager-keycloak-user-id"));
        var service = CreateSeeder(dependencies);

        // Act
        await service.SeedAsync(CancellationToken.None);

        // Assert
        dependencies.ManagerProfileRepository.ManagerProfiles.Should().ContainSingle();
        var profile = dependencies.ManagerProfileRepository.ManagerProfiles.Single();
        profile.Id.Should().Be(existingProfile.Id);
        profile.FullName.Should().Be(dependencies.Settings.FullName);
        profile.Email.Value.Should().Be(dependencies.Settings.Email);
        dependencies.UnitOfWork.SaveChangesCalls.Should().Be(1);
    }

    [Fact]
    public async Task Given_SeedAsync_When_EmailBelongsToAnotherKeycloakUser_Then_ShouldThrow()
    {
        // Arrange
        var dependencies = CreateDependencies(CreateEnabledSettings());
        var conflictingProfile = ManagerProfile.Create("other-keycloak-user-id", "Outro Gestor", dependencies.Settings.Email).Value;
        await dependencies.ManagerProfileRepository.AddAsync(conflictingProfile, CancellationToken.None);
        dependencies.IdentityProvider.ConfigureEnsureManagerResult(new EnsureManagerIdentityUserResponse("manager-keycloak-user-id"));
        var service = CreateSeeder(dependencies);

        // Act
        var act = () => service.SeedAsync(CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Manager seed email '{dependencies.Settings.Email}' is already linked to another Keycloak user id.");
        dependencies.UnitOfWork.SaveChangesCalls.Should().Be(0);
        dependencies.MessagePublisher.PublishedMessages.Should().BeEmpty();
    }

    [Fact]
    public async Task Given_SeedAsync_When_IdentityProviderFails_Then_ShouldThrow()
    {
        // Arrange
        var dependencies = CreateDependencies(CreateEnabledSettings());
        dependencies.IdentityProvider.ConfigureEnsureManagerResult(
            Error.Failure("IdentityProvider.Unavailable", "The identity provider is unavailable."));
        var service = CreateSeeder(dependencies);

        // Act
        var act = () => service.SeedAsync(CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Manager seed failed in identity provider. ErrorCode: IdentityProvider.Unavailable");
    }

    private static ManagerSeeder CreateSeeder(SeedDependencies dependencies)
    {
        return new ManagerSeeder(
            dependencies.IdentityProvider,
            dependencies.ManagerProfileRepository,
            dependencies.UnitOfWork,
            dependencies.MessagePublisher,
            Options.Create(dependencies.Settings),
            NullLogger<ManagerSeeder>.Instance);
    }

    private static SeedDependencies CreateDependencies(ManagerSeedSettings settings)
    {
        var identityProvider = new FakeIdentityProvider();
        var managerProfileRepository = new InMemoryManagerProfileRepository();
        var unitOfWork = new FakeUnitOfWork();
        var messagePublisher = new FakeMessagePublisher();
        var services = new ServiceCollection();

        services.AddSingleton<IIdentityProvider>(identityProvider);
        services.AddSingleton<IManagerProfileRepository>(managerProfileRepository);
        services.AddSingleton<IUnitOfWork>(unitOfWork);
        services.AddSingleton<IMessagePublisher>(messagePublisher);

        return new SeedDependencies(
            settings,
            services.BuildServiceProvider(),
            identityProvider,
            managerProfileRepository,
            unitOfWork,
            messagePublisher);
    }

    private static ManagerSeedSettings CreateEnabledSettings()
    {
        return new ManagerSeedSettings
        {
            Enabled = true,
            FullName = "Gestor ONG",
            Email = "gestor@email.com",
            Password = "StrongPassword123!"
        };
    }

    private sealed record SeedDependencies(
        ManagerSeedSettings Settings,
        IServiceProvider ServiceProvider,
        FakeIdentityProvider IdentityProvider,
        InMemoryManagerProfileRepository ManagerProfileRepository,
        FakeUnitOfWork UnitOfWork,
        FakeMessagePublisher MessagePublisher);
}
