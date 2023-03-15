#tool nuget:?package=dotnet-sonarscanner&version=5.11.0

#addin nuget:?package=Cake.Sonar&version=1.1.31

#load ".cake-scripts/parameters.cake"

readonly var parameters = BuildParameters.Instance(Context);

Task("Clean")
    .Does(() => {
        var deleteDirectorySettings = new DeleteDirectorySettings();
        deleteDirectorySettings.Recursive = true;
        deleteDirectorySettings.Force = true;

        foreach (var directory in parameters.Paths.Directories.ToClean)
        {
            if (DirectoryExists(directory))
            {
                DeleteDirectory(directory, deleteDirectorySettings);
            }
        }
    });

Task("Restore-NuGet-Packages")
    .Does(() => {
        DotNetRestore(parameters.Solution);
    });

Task("Build")
    .Does(() => {
        DotNetBuild(parameters.Solution, new DotNetBuildSettings
        {
            NoRestore = true,
            Configuration = parameters.Configuration
        });
    });

Task("Test")
    .Does(() => {
        DotNetTest(parameters.Solution, new DotNetTestSettings
        {
            Configuration = parameters.Configuration,
            Verbosity = parameters.Verbosity,
            NoBuild = false,
            NoRestore = true,
            Collectors = new[] { "XPlat Code Coverage;Format=opencover" },
            Loggers = new[] { "trx" },
            Filter = parameters.TestFilter,
            ResultsDirectory = parameters.Paths.Directories.TestResultsDirectoryPath,
            ArgumentCustomization = args => args
        });
    });

Task("Sonar-Begin")
  .Does(() => {
        SonarBegin(new SonarBeginSettings
        {
            Url = parameters.SonarQubeCredentials.Url,
            Key = parameters.SonarQubeCredentials.Key,
            Login = parameters.SonarQubeCredentials.Token,
            Organization = parameters.SonarQubeCredentials.Organization,
            Branch = parameters.IsPullRequest ? null : parameters.Branch, // A pull request analysis cannot have the branch analysis parameter 'sonar.branch.name'.
            UseCoreClr = true,
            Silent = true,
            Version = "0.0.0",
            PullRequestProvider = "GitHub",
            PullRequestGithubEndpoint = "https://api.github.com/",
            PullRequestGithubRepository = "PetrBilek1/headlines",
            PullRequestKey = parameters.IsPullRequest && System.Int32.TryParse(parameters.PullRequestId, out var id) ? id : (int?)null,
            PullRequestBranch = parameters.SourceBranch,
            PullRequestBase = parameters.TargetBranch,
            OpenCoverReportsPath = $"{MakeAbsolute(parameters.Paths.Directories.TestResultsDirectoryPath)}/**/*.opencover.xml",
            VsTestReportsPath = $"{MakeAbsolute(parameters.Paths.Directories.TestResultsDirectoryPath)}/**/*.trx"
        });
    });

Task("Sonar-End")
  .Does(() => {
        SonarEnd(new SonarEndSettings
        {
            Login = parameters.SonarQubeCredentials.Token,
            UseCoreClr = true
        });
    });

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("Build");
    //.IsDependentOn("Test");

RunTarget(parameters.Target);