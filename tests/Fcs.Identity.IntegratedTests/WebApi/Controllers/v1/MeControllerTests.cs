using System.Net;
using System.Net.Http.Json;
using Fcs.Identity.Application.UseCases.Profiles.GetMe;
using Fcs.Identity.CommomTestsUtilities.Builders.DonorProfiles;
using Fcs.Identity.Domain.Abstractions;
using Fcs.Identity.Domain.DonorProfiles;
using Fcs.Identity.Domain.ManagerProfiles;
using Fcs.Identity.Domain.Shared;
using Fcs.Identity.IntegratedTests.Configurations;
using Fcs.Identity.IntegratedTests.Support;
using Fcs.Identity.WebApi.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Fcs.Identity.IntegratedTests.WebApi.Controllers.v1;

[Collection(IntegrationTestCollection.Name)]
public sealed class MeControllerTests : IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public MeControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
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
    public async Task Given_MeEndpoint_Called_When_UserIsDonor_Then_ShouldReturnDonorProfile()
    {
        // Arrange
        var donorProfile = new DonorProfileBuilder().Build();
        await SaveDonorProfileAsync(donorProfile);
        AddAuthenticationHeaders(donorProfile.KeycloakUserId, IdentityRoles.Donor);

        // Act
        var response = await _client.GetAsync("/api/v1/me");
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse<GetMeResponse>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        payload.Should().NotBeNull();
        payload!.Success.Should().BeTrue();
        payload.Data.Should().NotBeNull();
        payload.Data!.Id.Should().Be(donorProfile.Id);
        payload.Data.Role.Should().Be(IdentityRoles.Donor);
    }

    [DockerAvailableFact]
    public async Task Given_MeEndpoint_Called_When_UserIsManager_Then_ShouldReturnManagerProfile()
    {
        // Arrange
        var managerProfile = ManagerProfile.Create(Guid.NewGuid().ToString(), "Gestor ONG", "gestor@ong.test").Value;
        await SaveManagerProfileAsync(managerProfile);
        AddAuthenticationHeaders(managerProfile.KeycloakUserId, IdentityRoles.Manager);

        // Act
        var response = await _client.GetAsync("/api/v1/me");
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse<GetMeResponse>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        payload.Should().NotBeNull();
        payload!.Success.Should().BeTrue();
        payload.Data.Should().NotBeNull();
        payload.Data!.Id.Should().Be(managerProfile.Id);
        payload.Data.Role.Should().Be(IdentityRoles.Manager);
    }

    [DockerAvailableFact]
    public async Task Given_MeEndpoint_Called_When_UserIsNotAuthenticated_Then_ShouldReturnUnauthorized()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync("/api/v1/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [DockerAvailableFact]
    public async Task Given_MeEndpoint_Called_When_ProfileDoesNotExist_Then_ShouldReturnNotFound()
    {
        // Arrange
        AddAuthenticationHeaders(Guid.NewGuid().ToString(), IdentityRoles.Donor);

        // Act
        var response = await _client.GetAsync("/api/v1/me");
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        payload.Should().NotBeNull();
        payload!.Success.Should().BeFalse();
        payload.Message.Should().Be("Profile was not found.");
    }

    private void AddAuthenticationHeaders(string keycloakUserId, string role)
    {
        _client.DefaultRequestHeaders.Remove(TestAuthenticationHandler.KeycloakUserIdHeader);
        _client.DefaultRequestHeaders.Remove(TestAuthenticationHandler.RoleHeader);
        _client.DefaultRequestHeaders.Add(TestAuthenticationHandler.KeycloakUserIdHeader, keycloakUserId);
        _client.DefaultRequestHeaders.Add(TestAuthenticationHandler.RoleHeader, role);
    }

    private async Task SaveDonorProfileAsync(DonorProfile donorProfile)
    {
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IDonorProfileRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        await repository.AddAsync(donorProfile, CancellationToken.None);
        await unitOfWork.SaveChangesAsync(CancellationToken.None);
    }

    private async Task SaveManagerProfileAsync(ManagerProfile managerProfile)
    {
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IManagerProfileRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        await repository.AddAsync(managerProfile, CancellationToken.None);
        await unitOfWork.SaveChangesAsync(CancellationToken.None);
    }
}
