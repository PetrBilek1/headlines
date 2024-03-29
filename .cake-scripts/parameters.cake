#load "./credentials.cake"
#load "./version.cake"
#load "./paths.cake"

internal sealed class BuildParameters
{
    private BuildParameters()
    {
    }

    public string Solution { get; private set; }
    public string Target { get; private set; }
    public string Configuration { get; private set; }
    public string Sha { get; private set; }
    public string Branch { get; private set; }
    public string SourceBranch { get; private set; }
    public string TargetBranch { get; private set; }
    public string PullRequestId { get; private set; }
    public string Version { get; private set; }
    public string TestFilter { get; private set; }
    public bool IsLocalBuild { get; private set; }
    public bool IsReleaseBuild { get; private set; }
    public bool IsPullRequest { get; private set; }
    public DotNetVerbosity Verbosity { get; private set; }
    public SonarQubeCredentials SonarQubeCredentials { get; private set; }
    public BuildPaths Paths { get; private set; }

    public static BuildParameters Instance(ICakeContext context)
    {
        const string propertiesFilePath = "Directory.Build.props";
        const string solutionFilePath = "Headlines.sln";
        
        var buildInformation = BuildInformation.Instance(context, propertiesFilePath);

        return new BuildParameters
        {
            Solution = context.MakeAbsolute(new DirectoryPath(solutionFilePath)).FullPath,
            Target = context.Argument("target", "Default"),
            Configuration = context.Argument("configuration", "Release"),
            Sha = buildInformation.Sha,
            Branch = buildInformation.Branch,
            SourceBranch = buildInformation.SourceBranch,
            TargetBranch = buildInformation.TargetBranch,
            PullRequestId = buildInformation.PullRequestId,
            Version = buildInformation.Version,
            TestFilter = context.Argument<string>("test-filter", null),
            IsLocalBuild = buildInformation.IsLocalBuild,
            IsReleaseBuild = !buildInformation.IsLocalBuild && buildInformation.IsReleaseBuild,
            IsPullRequest = buildInformation.IsPullRequest,
            Verbosity = DotNetVerbosity.Quiet,
            SonarQubeCredentials = SonarQubeCredentials.GetSonarQubeCredentials(context),
            Paths = BuildPaths.Instance(context),
        };
    }
}