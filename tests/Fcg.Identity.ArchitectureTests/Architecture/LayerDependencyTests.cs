using System.Reflection;
using Fcg.Identity.Domain.DonorProfiles;
using Fcg.Identity.Messages;
using Fcg.Identity.WebApi.Models;
using FluentAssertions;
using NetArchTest.Rules;

namespace Fcg.Identity.ArchitectureTests.Architecture;

public class LayerDependencyTests
{
    private static readonly Assembly DomainAssembly = typeof(DonorProfile).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(Fcg.Identity.Application.DependencyInjection.DependencyInjection).Assembly;
    private static readonly Assembly MessagesAssembly = typeof(ResourceMessages).Assembly;
    private static readonly Assembly WebApiAssembly = typeof(ApiResponse<>).Assembly;

    private static readonly Assembly[] InfrastructureAssemblyValues =
    [
        typeof(Fcg.Identity.Infrastructure.Kafka.DependencyInjection.DependencyInjection).Assembly,
        typeof(Fcg.Identity.Infrastructure.Keycloak.DependencyInjection.DependencyInjection).Assembly,
        typeof(Fcg.Identity.Infrastructure.SqlServer.DependencyInjection.DependencyInjection).Assembly
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
        { MessagesAssembly, WebApiAssembly.GetName().Name! },
        { InfrastructureAssemblyValues[0], WebApiAssembly.GetName().Name! },
        { InfrastructureAssemblyValues[1], WebApiAssembly.GetName().Name! },
        { InfrastructureAssemblyValues[2], WebApiAssembly.GetName().Name! }
    };

    [Fact]
    public void Domain_Should_Not_Depend_On_Other_Fcg_Projects()
    {
        var result = Types
            .InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOnAny([
                ApplicationAssembly.GetName().Name!,
                MessagesAssembly.GetName().Name!,
                WebApiAssembly.GetName().Name!,
                InfrastructureAssemblyValues[0].GetName().Name!,
                InfrastructureAssemblyValues[1].GetName().Name!,
                InfrastructureAssemblyValues[2].GetName().Name!
            ])
            .GetResult();

        result.ShouldBeSuccessful("the domain layer must remain independent from application, infrastructure, messages, and web api projects");
    }

    [Fact]
    public void Messages_Should_Not_Depend_On_Other_Fcg_Projects()
    {
        var result = Types
            .InAssembly(MessagesAssembly)
            .Should()
            .NotHaveDependencyOnAny([
                DomainAssembly.GetName().Name!,
                ApplicationAssembly.GetName().Name!,
                WebApiAssembly.GetName().Name!,
                InfrastructureAssemblyValues[0].GetName().Name!,
                InfrastructureAssemblyValues[1].GetName().Name!,
                InfrastructureAssemblyValues[2].GetName().Name!
            ])
            .GetResult();

        result.ShouldBeSuccessful("message contracts should stay isolated so services can share contracts without pulling application code");
    }

    [Theory]
    [InlineData("Fcg.Identity.Infrastructure.Kafka")]
    [InlineData("Fcg.Identity.Infrastructure.Keycloak")]
    [InlineData("Fcg.Identity.Infrastructure.SqlServer")]
    [InlineData("Fcg.Identity.WebApi")]
    public void Application_Should_Not_Depend_On_Infrastructure_Or_WebApi(string forbiddenDependency)
    {
        var result = Types
            .InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn(forbiddenDependency)
            .GetResult();

        result.ShouldBeSuccessful("the application layer can orchestrate domain and contracts, but must not know infrastructure or web api details");
    }

    [Theory]
    [MemberData(nameof(InfrastructureAssemblies))]
    public void Infrastructure_Should_Not_Depend_On_Other_Infrastructure_Projects(Assembly assembly, string currentAssemblyName)
    {
        var forbiddenInfrastructureDependencies = InfrastructureAssemblyValues
            .Select(infrastructureAssembly => infrastructureAssembly.GetName().Name!)
            .Where(assemblyName => assemblyName != currentAssemblyName)
            .ToArray();

        var result = Types
            .InAssembly(assembly)
            .Should()
            .NotHaveDependencyOnAny(forbiddenInfrastructureDependencies)
            .GetResult();

        result.ShouldBeSuccessful("infrastructure adapters should not depend on each other");
    }

    [Theory]
    [MemberData(nameof(InnerLayersAndForbiddenWebApiDependency))]
    public void Inner_Layers_Should_Not_Depend_On_WebApi(Assembly assembly, string webApiAssemblyName)
    {
        var result = Types
            .InAssembly(assembly)
            .Should()
            .NotHaveDependencyOn(webApiAssemblyName)
            .GetResult();

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
