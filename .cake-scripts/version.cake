#addin nuget:?package=Cake.Git&version=3.0.0

internal sealed class BuildInformation
{
    private BuildInformation()
    {
    }

    public string Sha { get; private set; }
    public string Branch { get; private set; }
    public string SourceBranch { get; private set; }
    public string TargetBranch { get; private set; }
    public string PullRequestId { get; private set; }
    public bool IsLocalBuild { get; private set; }
    public bool IsReleaseBuild { get; private set; }
    public bool IsPullRequest { get; private set; }
    public bool ShouldPublish { get; private set; }

    public static BuildInformation Instance(ICakeContext context)
    {
        var buildSystem = context.BuildSystem();

        var isLocalBuild = buildSystem.IsLocalBuild;

        var environment = buildSystem.GitHubActions.Environment;

        var publishImages = context.EnvironmentVariable("PUBLISH_DOCKER_IMAGES");

        var git = context.GitBranchCurrent(".");

        var timeStamp = git.Tip.Committer.When.ToUnixTimeMilliseconds().ToString();

        var sha = git.Tip.Sha;

        var branch = git.FriendlyName;

        var isPullRequest = false;

        var isFork = false;

        var buildId = string.Empty;

        var pullRequestId = "0";

        string sourceBranch = null;

        string targetBranch = null;

        if (!isLocalBuild)
        {
            branch = environment.Workflow.RefName;
            isPullRequest = environment.PullRequest.IsPullRequest;
            isFork = "fork".Equals(environment.Workflow.EventName, StringComparison.OrdinalIgnoreCase);
            buildId = environment.Workflow.RunId;
        }

        if (isPullRequest)
        {
            pullRequestId = new string(environment.Workflow.Ref.Where(char.IsDigit).ToArray());
            sourceBranch = environment.Workflow.HeadRef;
            targetBranch = environment.Workflow.BaseRef;
        }

        var isReleaseBuild = GetIsReleaseBuild(branch);

        var shouldPublish = GetShouldPublish(branch) && ("1".Equals(publishImages, StringComparison.Ordinal) || (bool.TryParse(publishImages, out var result) && result));

        if (isFork && isPullRequest && shouldPublish)
        {
            throw new ArgumentException("Use 'feature/' or 'bugfix/' prefix for pull request branches.");
        }

        return new BuildInformation
        {
            Sha = sha,
            Branch = branch,
            SourceBranch = sourceBranch,
            TargetBranch = targetBranch,
            PullRequestId = pullRequestId,
            IsLocalBuild = isLocalBuild,
            IsReleaseBuild = isReleaseBuild,
            IsPullRequest = isPullRequest,
            ShouldPublish = shouldPublish
        };
    }

    private static bool GetIsReleaseBuild(string branch)
    {
        var branches = new[] { "master" };
        return branches.Any(b => StringComparer.OrdinalIgnoreCase.Equals(b, branch));
    }

    private static bool GetShouldPublish(string branch)
    {
        var branches = new[] { "master", "develop" };
        return branches.Any(b => StringComparer.OrdinalIgnoreCase.Equals(b, branch));
    }
}