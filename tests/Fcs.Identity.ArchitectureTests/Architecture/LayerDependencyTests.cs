using System.Reflection;
using Fcs.Identity.Domain.DonorProfiles;
using Fcs.Identity.WebApi.Models;
using FluentAssertions;
using NetArchTest.Rules;

namespace Fcs.Identity.ArchitectureTests.Architecture;

public class LayerDependencyTests
{
    private static readonly Assembly DomainAssembly = typeof(DonorProfile).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(Fcs.Identity.Application.DependencyInjection.DependencyInjection).Assembly;
    private static readonly Assembly WebApiAssembly = typeof(ApiResponse<>).Assembly;

    private static readonly Assembly[] InfrastructureAssemblyValues =
    [
        typeof(Fcs.Identity.Infrastructure.Kafka.DependencyInjection.DependencyInjection).Assembly,
        typeof(Fcs.Identity.Infrastructure.Keycloak.DependencyInjection.DependencyInjection).Assembly,
        typeof(Fcs.Identity.Infrastructure.SqlServer.DependencyInjection.DependencyInjection).Assembly
    ];

    public static TheoryData<Assembly, string> InfrastructureAssemblies => new()
    {
        { InfrastructureAssemblyValues[0], InfrastructureAssemblyValues[0].GetName().Name! },
        { InfrastructureAssemblyValues[1], InfrastructureAssemblyValues[1].GetName().Name! },
        { InfrastructureAssemblyValues[2], InfrastructureAssemblyValues[2].GetName().Name! }
    };

    public static TheoryData<Assembly, string> InnerLayersAndForbiddenWebApiDependency => new()
    {
        { DomainAssembly, WebApiAssembly.GetName().Name! },
        { ApplicationAssembly, WebApiAssembly.GetName().Name! },
        { InfrastructureAssemblyValues[0], WebApiAssembly.GetName().Name! },
        { InfrastructureAssemblyValues[1], WebApiAssembly.GetName().Name! },
        { InfrastructureAssemblyValues[2], WebApiAssembly.GetName().Name! }
    };

    [Fact]
    public void Given_DomainLayer_When_ArchitectureIsValidated_Then_ShouldNotDependOnOtherFcsProjects()
    {
        // Arrange
        var forbiddenDependencies = new[]
        {
            ApplicationAssembly.GetName().Name!,
            WebApiAssembly.GetName().Name!,
            InfrastructureAssemblyValues[0].GetName().Name!,
            InfrastructureAssemblyValues[1].GetName().Name!,
            InfrastructureAssemblyValues[2].GetName().Name!
        };

        // Act
        var result = Types
            .InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOnAny(forbiddenDependencies)
            .GetResult();

        // Assert
        result.ShouldBeSuccessful("the domain layer must remain independent from application, infrastructure, and web api projects");
    }

    [Theory]
    [InlineData("Fcs.Identity.Infrastructure.Kafka")]
    [InlineData("Fcs.Identity.Infrastructure.Keycloak")]
    [InlineData("Fcs.Identity.Infrastructure.SqlServer")]
    [InlineData("Fcs.Identity.WebApi")]
    public void Given_ApplicationLayer_When_ArchitectureIsValidated_Then_ShouldNotDependOnInfrastructureOrWebApi(string forbiddenDependency)
    {
        // Arrange
        var dependency = forbiddenDependency;

        // Act
        var result = Types
            .InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn(dependency)
            .GetResult();

        // Assert
        result.ShouldBeSuccessful("the application layer can orchestrate domain and contracts, but must not know infrastructure or web api details");
    }

    [Theory]
    [MemberData(nameof(InfrastructureAssemblies))]
    public void Given_InfrastructureLayer_When_ArchitectureIsValidated_Then_ShouldNotDependOnOtherInfrastructureProjects(Assembly assembly, string currentAssemblyName)
    {
        // Arrange
        var forbiddenInfrastructureDependencies = InfrastructureAssemblyValues
            .Select(infrastructureAssembly => infrastructureAssembly.GetName().Name!)
            .Where(assemblyName => assemblyName != currentAssemblyName)
            .ToArray();

        // Act
        var result = Types
            .InAssembly(assembly)
            .Should()
            .NotHaveDependencyOnAny(forbiddenInfrastructureDependencies)
            .GetResult();

        // Assert
        result.ShouldBeSuccessful("infrastructure adapters should not depend on each other");
    }

    [Theory]
    [MemberData(nameof(InnerLayersAndForbiddenWebApiDependency))]
    public void Given_InnerLayer_When_ArchitectureIsValidated_Then_ShouldNotDependOnWebApi(Assembly assembly, string webApiAssemblyName)
    {
        // Arrange
        var forbiddenDependency = webApiAssemblyName;

        // Act
        var result = Types
            .InAssembly(assembly)
            .Should()
            .NotHaveDependencyOn(forbiddenDependency)
            .GetResult();

        // Assert
        result.ShouldBeSuccessful("the web api is the composition edge and must not be referenced by inner layers");
    }
}

internal static class TestResultExtensions
{
    public static void ShouldBeSuccessful(this TestResult result, string because)
    {
        result.IsSuccessful.Should().BeTrue(
            because + ". Failing types: {0}",
            result.FailingTypes?.Select(type => type.FullName).Order().ToArray() ?? []);
    }
}
