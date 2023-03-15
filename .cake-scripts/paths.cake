internal sealed class BuildPaths
{
  private BuildPaths()
  {
  }

  public BuildDirectories Directories { get; private set; }

  public static BuildPaths Instance(ICakeContext context)
  {
    var baseDir = (DirectoryPath) context.Directory(".");

    var testResultsDir = baseDir.Combine("test-results");
    var testCoverageDir = baseDir.Combine("test-coverage");

    return new BuildPaths
    {
      Directories = new BuildDirectories(
        testResultsDir,
        testCoverageDir
      )
    };
  }
}

internal sealed class BuildDirectories
{
  public DirectoryPath TestResultsDirectoryPath { get; }
  public DirectoryPath TestCoverageDirectoryPath { get; }
  public ICollection<DirectoryPath> ToClean { get; }

  public BuildDirectories(
    DirectoryPath testResultsDir,
    DirectoryPath testCoverageDir)
  {
    TestResultsDirectoryPath = testResultsDir;
    TestCoverageDirectoryPath = testCoverageDir;
    ToClean = new List<DirectoryPath>()
    {
      testResultsDir,
      testCoverageDir,
      new DirectoryPath(".sonarqube")
    };
  }
}