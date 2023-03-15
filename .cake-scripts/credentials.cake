internal sealed class SonarQubeCredentials
{
    public string Url { get; private set; }
    public string Key { get; private set; }
    public string Token { get; private set; }
    public string Organization { get; private set; }

    private SonarQubeCredentials(string url, string key, string token, string organization)
    {
        Url = url;
        Key = key;
        Token = token;
        Organization = organization;
    }

    public static SonarQubeCredentials GetSonarQubeCredentials(ICakeContext context)
    {
        return new SonarQubeCredentials
        (
            context.EnvironmentVariable("SONARCLOUD_URL"),
            context.EnvironmentVariable("SONARCLOUD_KEY"),
            context.EnvironmentVariable("SONARCLOUD_TOKEN"),
            context.EnvironmentVariable("SONARCLOUD_ORGANIZATION")
        );
    }
}

internal sealed class NuGetPackageSourceCredentials
{
    public string Username { get; private set; }
    public string Password { get; private set; }
    public string Name { get; private set; }
    public string Source { get; private set; }

    private NuGetPackageSourceCredentials(string username, string password, string name, string source)
    {
        Username = username;
        Password = password;
        Name = name;
        Source = source;
    }

    public static NuGetPackageSourceCredentials GetPBilekPackageSourceCredentials(ICakeContext context)
    {
        return new NuGetPackageSourceCredentials
        (
            context.EnvironmentVariable("GITHUB_ACTOR"),
            "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",//context.EnvironmentVariable("PBILEK_PACKAGES_TOKEN"),
            "pbilek-github",
            "https://nuget.pkg.github.com/PetrBilek1/index.json"
        );
    }
}