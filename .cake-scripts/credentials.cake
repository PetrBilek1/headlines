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