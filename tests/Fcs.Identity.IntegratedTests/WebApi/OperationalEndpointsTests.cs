using System.Net;
using Fcs.Identity.IntegratedTests.Configurations;
using Fcs.Identity.IntegratedTests.Support;
using FluentAssertions;

namespace Fcs.Identity.IntegratedTests.WebApi;

[Collection(IntegrationTestCollection.Name)]
public sealed class OperationalEndpointsTests
{
    private readonly HttpClient _client;

    public OperationalEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [DockerAvailableFact]
    public async Task Given_HealthEndpoint_Called_When_ServiceIsRunning_Then_ShouldReturnOk()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [DockerAvailableFact]
    public async Task Given_MetricsEndpoint_Called_When_ServiceIsRunning_Then_ShouldReturnPrometheusMetrics()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync("/metrics");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().Contain("# HELP");
    }
}
