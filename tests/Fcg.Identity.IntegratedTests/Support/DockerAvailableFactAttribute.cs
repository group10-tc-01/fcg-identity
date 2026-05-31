using System.Diagnostics;

namespace Fcg.Identity.IntegratedTests.Support;

[AttributeUsage(AttributeTargets.Method)]
public sealed class DockerAvailableFactAttribute : FactAttribute
{
    private static readonly Lazy<bool> DockerAvailable = new(IsDockerAvailable);

    public DockerAvailableFactAttribute()
    {
        if (!DockerAvailable.Value)
        {
            Skip = "Docker is required to run this Testcontainers integration test.";
        }
    }

    private static bool IsDockerAvailable()
    {
        if (string.Equals(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"), "true", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (string.Equals(Environment.GetEnvironmentVariable("RUN_TESTCONTAINERS"), "true", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DOCKER_HOST")))
        {
            return true;
        }

        if (!OperatingSystem.IsWindows() && File.Exists("/var/run/docker.sock"))
        {
            return true;
        }

        return DockerCliIsAvailable();
    }

    private static bool DockerCliIsAvailable()
    {
        try
        {
            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = "version --format {{.Server.Version}}",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });

            if (process is null)
            {
                return false;
            }

            return process.WaitForExit(milliseconds: 3000) && process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
}
