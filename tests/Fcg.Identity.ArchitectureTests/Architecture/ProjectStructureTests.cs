using System.Xml.Linq;
using FluentAssertions;

namespace Fcg.Identity.ArchitectureTests.Architecture;

public class ProjectStructureTests
{
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    public static TheoryData<string> ProductionProjects => new()
    {
        "src/Fcg.Identity.Application/Fcg.Identity.Application.csproj",
        "src/Fcg.Identity.Domain/Fcg.Identity.Domain.csproj",
        "src/Fcg.Identity.Infrastructure.Auth/Fcg.Identity.Infrastructure.Auth.csproj",
        "src/Fcg.Identity.Infrastructure.Http/Fcg.Identity.Infrastructure.Http.csproj",
        "src/Fcg.Identity.Infrastructure.Kafka/Fcg.Identity.Infrastructure.Kafka.csproj",
        "src/Fcg.Identity.Infrastructure.SqlServer/Fcg.Identity.Infrastructure.SqlServer.csproj",
        "src/Fcg.Identity.Messages/Fcg.Identity.Messages.csproj",
        "src/Fcg.Identity.WebApi/Fcg.Identity.WebApi.csproj"
    };

    [Theory]
    [MemberData(nameof(ProductionProjects))]
    public void Production_Projects_Should_Target_Net8(string relativeProjectPath)
    {
        var projectPath = Path.Combine(RepositoryRoot, relativeProjectPath);
        var targetFramework = GetTargetFramework(projectPath);

        targetFramework.Should().Be("net8.0", "Fase 05 backend services are standardized on .NET 8");
    }

    [Fact]
    public void Source_Projects_Should_Follow_Fcg_Identity_Structure()
    {
        var expectedProjectDirectories = new[]
        {
            "src/Fcg.Identity.Domain",
            "src/Fcg.Identity.Application",
            "src/Fcg.Identity.Infrastructure.Auth",
            "src/Fcg.Identity.Infrastructure.Http",
            "src/Fcg.Identity.Infrastructure.Kafka",
            "src/Fcg.Identity.Infrastructure.SqlServer",
            "src/Fcg.Identity.WebApi"
        };

        expectedProjectDirectories
            .Select(path => Path.Combine(RepositoryRoot, path))
            .Should()
            .OnlyContain(path => Directory.Exists(path), "fcg-identity must preserve the phase 04 clean architecture project layout");
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Fcg.Identity.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not find fcg-identity repository root.");
    }

    private static string? GetTargetFramework(string projectPath)
    {
        return GetTargetFrameworkFrom(projectPath)
               ?? GetTargetFrameworkFrom(Path.Combine(RepositoryRoot, "Directory.Build.props"));
    }

    private static string? GetTargetFrameworkFrom(string path)
    {
        var project = XDocument.Load(path);

        return project
            .Root?
            .Elements("PropertyGroup")
            .Elements("TargetFramework")
            .Select(element => element.Value)
            .FirstOrDefault();
    }
}
