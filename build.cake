#load ".cake-scripts/parameters.cake"

readonly var parameters = BuildParameters.Instance(Context);

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
            NoBuild = true,
            NoRestore = true,
            Collectors = new[] { "XPlat Code Coverage;Format=opencover" },
            Loggers = new[] { "trx" },
            Filter = parameters.TestFilter,
            ResultsDirectory = parameters.Paths.Directories.TestResultsDirectoryPath,
            ArgumentCustomization = args => args
        });
    });

Task("Default")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("Build");
    //.IsDependentOn("Test");

RunTarget(parameters.Target);