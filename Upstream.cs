using System.Text.Json.Nodes;

public record Upstream(
    string Name,
    string Url,
    string ResponseCode,
    Dictionary<string, string> ResponseHeaders,
    string StartTime,
    string EndTime,
    string StartTimeUtc,
    string EndTimeUtc,
    double Duration,
    JsonObject ResponseBody,
    string ExceptionMessage)
{
    public Upstream(string Name, string Url, string ExceptionMessage) : this(Name, Url, string.Empty,
        new Dictionary<string, string>(), string.Empty, string.Empty, string.Empty, string.Empty, 0,
        new JsonObject(), ExceptionMessage)
    {
    }
}

public record UpstreamResults(
    string ApplicationName,
    string VersionTag,
    string VersionGitSHA,
    string VersionCommitDate,
    string Environment,
    string StartTime,
    string EndTime,
    string StartTimeUtc,
    string EndTimeUtc,
    double Duration,
    List<Upstream> UpstreamCalls
);
