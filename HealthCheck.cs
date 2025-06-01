using Microsoft.Extensions.Diagnostics.HealthChecks;

// Reference: https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks
// Includes an example of a database probe.

public class DefaultHealthCheck(IConfiguration config, ILogger<DefaultHealthCheck> logger) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var isHealthy = true;
        var versionGitSHA = config["Version:GitSHA"] ?? "unknown";
        var versionCommitDate = config["Version:CommitDate"] ?? "unknown";
        var versionTag = config["Version:Tag"] ?? "unknown";
        IReadOnlyDictionary<string, object> data = new Dictionary<string, object>
        {
            { "VersionGitSHA", versionGitSHA },
            { "VersionCommitDate", versionCommitDate },
            { "VersionTag", versionTag }
        };


        // Add checks here

        if (isHealthy)
        {
            logger.LogInformation("The service is healthy (version {VersionGitSHA})", versionGitSHA);
            return Task.FromResult(
                new HealthCheckResult(
                    HealthStatus.Healthy, "The service is healthy", null, data));
        }

        logger.LogInformation("The service is unhealthy (version {VersionGitSHA})", versionGitSHA);
        return Task.FromResult(
            new HealthCheckResult(
                HealthStatus.Unhealthy, "The service is unhealthy", null, data));
    }
}
