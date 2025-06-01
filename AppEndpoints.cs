using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

public static class AppEndpoints
{
    public static void Map(WebApplication app, IConfigurationSection appVersion)
    {
        app.MapHealthChecks("/healthcheck", new HealthCheckOptions
        {
            ResponseWriter = new HealthCheckResponseWriter(appVersion).WriteResponse
        });

        app.MapGet("/info", AppInfo.Create);

        app.MapGet("/unauthorized", Results.Unauthorized);

        app.MapGet("/upstream", (IConfiguration config, ILogger<Program> logger) =>
        {
            var timeFormat = "yyyy-MM-dd HH:mm:ss";
            var upstreamResults = new List<Upstream>();
            // Sometime, this might be refactored using the Option pattern (https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options):
            var endpoints = config.GetSection("upstream").GetSection("endpoints").GetChildren();
            HttpClient upstreamClient = new() { Timeout = TimeSpan.FromSeconds(10) };
            upstreamClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var startAll = DateTime.UtcNow;
            foreach (var endpoint in endpoints)
                try
                {
                    logger.LogInformation($"Calling upstream {endpoint.Key} -> {endpoint.Value}");
                    var start = DateTime.UtcNow;
                    var response = upstreamClient.GetAsync(endpoint.Value);
                    response.Wait();
                    var end = DateTime.UtcNow;
                    var duration = end - start;
                    var result = response.Result;
                    var headers = result.Headers;
                    var content = result.Content;
                    var allHeaders = new Dictionary<string, string>();
                    foreach (var header in headers) allHeaders.Add(header.Key, string.Join(";", header.Value.ToList()));
                    foreach (var header in content.Headers)
                        allHeaders.Add(header.Key, string.Join(";", header.Value.ToList()));
                    var jsonContent = new JsonObject();
                    if (content.Headers.ContentType is { MediaType: "application/json" })
                        try
                        {
                            jsonContent = content.ReadFromJsonAsync<JsonObject>().Result;
                        }
                        catch (Exception e)
                        {
                            logger.LogInformation(e.Message);
                        }

                    upstreamResults.Add(new Upstream(
                        endpoint.Key, endpoint.Value ?? string.Empty,
                        result.StatusCode.ToString(),
                        allHeaders,
                        start.ToLocalTime().ToString(timeFormat),
                        end.ToLocalTime().ToString(timeFormat),
                        start.ToUniversalTime().ToString(timeFormat),
                        end.ToUniversalTime().ToString(timeFormat),
                        duration.TotalSeconds,
                        jsonContent ?? new JsonObject(),
                        string.Empty
                    ));
                }
                catch (Exception e)
                {
                    upstreamResults.Add(new Upstream(endpoint.Key, endpoint.Value ?? string.Empty, e.Message));
                }

            var endAll = DateTime.UtcNow;
            var durationAll = endAll - startAll;
            return new UpstreamResults(
                config["applicationName"] ?? "unknown",
                config["Version:Tag"] ?? "unknown",
                config["Version:GitSHA"] ?? "unknown",
                config["Version:CommitDate"] ?? "unknown",
                config["ENVIRONMENT"] ?? "unknown",
                startAll.ToLocalTime().ToString(timeFormat),
                endAll.ToLocalTime().ToString(timeFormat),
                startAll.ToUniversalTime().ToString(timeFormat),
                endAll.ToUniversalTime().ToString(timeFormat),
                durationAll.TotalSeconds,
                upstreamResults
            );
        });
    }
}
