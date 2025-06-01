public class AppInfo
{
    private readonly IConfiguration _config;

    private AppInfo(IConfiguration config)
    {
        _config = config;
    }

    public string VersionTag => _config["Version:Tag"] ?? "unknown";
    public string VersionGitSHA => _config["Version:GitSHA"] ?? "unknown";
    public string VersionCommitDate => _config["Version:CommitDate"] ?? "unknown";
    public string Environment => _config["ENVIRONMENT"] ?? "unknown";
    public string ApplicationName => _config["applicationName"] ?? "unknown";

    public static AppInfo Create(IConfiguration config)
    {
        return new AppInfo(config);
    }
}
