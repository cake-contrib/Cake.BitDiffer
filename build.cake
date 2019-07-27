///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// TOOLS / ADDINS / LOADS
///////////////////////////////////////////////////////////////////////////////

#tool "nuget:?package=GitVersion.CommandLine&version=4.0.0"
#tool "nuget:?package=MSBuild.SonarQube.Runner.Tool&version=4.6.0"
#tool "nuget:?package=JetBrains.dotCover.CommandLineTools&version=2019.1.3"

#addin "nuget:?package=Cake.Sonar&version=1.1.22"

#load "./build/paths.cake"
#load "./build/names.cake"

///////////////////////////////////////////////////////////////////////////////
// ENVIRONMENT VARIABLES
///////////////////////////////////////////////////////////////////////////////

private var _sonarqubeApiToken = EnvironmentVariable(Names.SONARCUBE_API_TOKEN);
private var _sonarqubeUri = EnvironmentVariable(Names.SONARQUBE_URI);
private var _nugetUri = EnvironmentVariable(Names.NUGET_URI);
private var _nugetApiToken = EnvironmentVariable(Names.NUGET_API_TOKEN);

///////////////////////////////////////////////////////////////////////////////
// VERSION
///////////////////////////////////////////////////////////////////////////////

private var _version = GitVersion();

Information($"Current Version: {_version.SemVer}");

if(AppVeyor.IsRunningOnAppVeyor) {
    BuildSystem.AppVeyor.UpdateBuildVersion(_version.SemVer);
}

///////////////////////////////////////////////////////////////////////////////
// PRIVATE TASKS
///////////////////////////////////////////////////////////////////////////////

Task("_cleanup")
    .Description("Cleanup bin, obj and Build folders")
    .Does(() => {
        CleanDirectories("./src/**/bin");
        CleanDirectories("./src/**/obj");
        CleanDirectories("./tests/**/bin");
        CleanDirectories("./tests/**/obj");
        CleanDirectories(new [] {
            Paths.BUILD_OUTPUT,
            Paths.TEST_OUTPUT,
            Paths.ARTIFACTS_OUTPUT,
            Paths.SONARQUBE_OUTPUT
        });
    });

Task("_restore")
    .Description("Restore all nuget packages")
    .Does(() => {
        NuGetRestore(Paths.SOLUTION_FILE);
    });

Task("_fixVersion")
    .Description("Set new assembly version generated from Git history")
    .Does(() => {
        // Fix solution version data
        CreateAssemblyInfo(Paths.ASSEMBLY_INFO_FILE, new AssemblyInfoSettings {
                Company = "Eugen (WebDucer) Richter",
                Copyright = Names.PROJECT_COPYRIGHTS
            }
            .AddMetadataAttribute("git_hash", _version.Sha)
            .AddMetadataAttribute("sem_ver", _version.FullSemVer)
            .AddMetadataAttribute("build_date", DateTime.Now.ToString("yyyy-MM-dd"))
        );

        var versionSettings = new GitVersionSettings {
            UpdateAssemblyInfo = true,
            WorkingDirectory = "./src"
        };

        GitVersion(versionSettings);
    });

Task("_buildLibraries")
    .Description("Build library projects")
    .Does(() => {
        var libProjects = GetFiles("./src/**/*.csproj");
        var outputPath = MakeAbsolute(Directory(Paths.BUILD_OUTPUT)).FullPath.Quote();
        
        foreach(var libProject in libProjects) {
            MSBuild(libProject, buildOptions =>
                buildOptions
                    .SetConfiguration(configuration)
                    .WithTarget("Build")
                    .WithProperty("OutputPath", outputPath)
                    .WithProperty("ContinuousIntegrationBuild", "true")
                    .WithProperty("DeterministicSourcePaths", "true")
            );
        }
    });

Task("_buildLibraryForSonar")
    .Description("Build library projects for sonar analysis")
    .Does(() => {
        var outputPath = MakeAbsolute(Directory(Paths.BUILD_OUTPUT)).FullPath.Quote();
        
        MSBuild(Paths.SOLUTION_FILE_FOR_SONAR, buildOptions =>
            buildOptions
                .SetConfiguration(configuration)
                .WithTarget("Build")
                .WithProperty("OutputPath", outputPath)
        );
    });

Task("_buildTests")
    .Description("Build test projects")
    .Does(() => {
        var testProjects = GetFiles("./tests/**/*.csproj");
        var outputPath = Paths.Quote(MakeAbsolute(Directory(Paths.TEST_OUTPUT)));

        foreach (var project in testProjects)
        {
            MSBuild(project, buildSettings => {
                buildSettings.SetConfiguration(Names.DEFAULT_CONFIGURATION)
                    .WithTarget("Build");
            });
        }
    });

Task("_runOnlyTests")
    .Description("Run all tests from test folder")
    .WithCriteria(IsRunningOnUnix())
    .Does(() => {
        var outputDirectory = MakeAbsolute(Directory(Paths.ARTIFACTS_OUTPUT));

        var testSettings = new DotNetCoreTestSettings {
            Logger = $"trx;logfilename={Paths.TEST_RESULT_FILE}",
            ResultsDirectory = outputDirectory,
            NoBuild = true,
            Configuration = Names.DEFAULT_CONFIGURATION
        };

        DotNetCoreTest(Paths.TEST_PROJECT_FILE, testSettings);
    });

Task("_runCodeCoverageTests")
    .Description("Run all unit tests with code coverage analysis")
    .WithCriteria(IsRunningOnWindows())
    .Does(() => {
        var outputDirectory = MakeAbsolute(Directory(Paths.ARTIFACTS_OUTPUT));

        var testSettings = new DotNetCoreTestSettings {
            Logger = $"trx;logfilename={Paths.TEST_RESULT_FILE}",
            ResultsDirectory = outputDirectory,
            NoBuild = true,
            Configuration = Names.DEFAULT_CONFIGURATION
        };

        var coverSettings = new DotCoverCoverSettings {}
            .WithFilter("+:assembly=" + Names.PROJECT_ID);

        DotCoverCover(tools => tools.DotNetCoreTest(Paths.TEST_PROJECT_FILE, testSettings),
            Paths.TEST_COVERAGE_RESULT_FILE,
            coverSettings
        );

        // Convert coverage files
        DotCoverReport(Paths.TEST_COVERAGE_RESULT_FILE, Paths.TEST_COVERAGE_RESULT_FILE_HTML, new DotCoverReportSettings { ReportType = DotCoverReportType.HTML });
        DotCoverReport(Paths.TEST_COVERAGE_RESULT_FILE, Paths.TEST_COVERAGE_RESULT_FILE_XML, new DotCoverReportSettings { ReportType = DotCoverReportType.XML });

        // Print Code Coverage result
        var coverage = System.Xml.Linq.XDocument.Load(Paths.TEST_COVERAGE_RESULT_FILE_XML)
            .Element("Root")
            .Attribute("CoveragePercent")
            .Value;
        Information(string.Format("Total code coverage: {0:0} %", coverage));
    });

Task("_sendTestResultsOnAppVeyor")
    .Description("Send unit tests result, if running on app veyor CI")
    .WithCriteria(AppVeyor.IsRunningOnAppVeyor)
    .Does(() => {
        var testResultFile = GetFiles(Paths.ARTIFACTS_OUTPUT + "*.trx").Single();
        BuildSystem.AppVeyor.UploadTestResults(testResultFile, AppVeyorTestResultsType.MSTest);
    });

Task("_runTests")
    .IsDependentOn("_runOnlyTests")
    .IsDependentOn("_runCodeCoverageTests")
    .IsDependentOn("_sendTestResultsOnAppVeyor");

Task("_startSonarQube")
    .Description("Start Sonarqube analysis")
    .WithCriteria(!string.IsNullOrEmpty(_sonarqubeUri))
    .WithCriteria(!string.IsNullOrEmpty(_sonarqubeApiToken))
    .Does(() => {
        var testResultFile = GetFiles(Paths.ARTIFACTS_OUTPUT + "*.trx").Single();
        var sonarSettings = new SonarBeginSettings {
            Key = Names.PROJECT_ID,
            Name = Names.PROJECT_ID,
            Version = BuildSystem.IsLocalBuild ? "ManualRun" : _version.SemVer,
            VsTestReportsPath = MakeAbsolute(testResultFile).FullPath,
            Login = _sonarqubeApiToken,
            Url = _sonarqubeUri,
            Organization = Names.SONARQUBE_ORGANISATION,
            Branch = _version.BranchName.Replace("/","_")
        };

        if(FileExists(Paths.TEST_COVERAGE_RESULT_FILE_HTML)) {
            sonarSettings.DotCoverReportsPath = Paths.TEST_COVERAGE_RESULT_FILE_HTML;
        }

        SonarBegin(sonarSettings);
    });

Task("_endSonarQube")
    .Description("Finish Sonarqube analysis")
    .WithCriteria(!string.IsNullOrEmpty(_sonarqubeUri))
    .WithCriteria(!string.IsNullOrEmpty(_sonarqubeApiToken))
    .Does(() => {
        var sonarSettings = new SonarEndSettings {
            Login = _sonarqubeApiToken
        };

        SonarEnd(sonarSettings);
    });

Task("_createNuGetPackage")
    .Description("Create NuGet package of the libraray")
    .Does(() => {
        var netStandardTarget = @"lib\netstandard2.0";

        var nugetSettings = new NuGetPackSettings {
            Id = Names.PROJECT_ID,
            Version = _version.NuGetVersion,
            Title = Names.PROJECT_TITLE,
            Authors = Names.PROJECT_AUTHORS,
            Owners = Names.PROJECT_OWNERS,
            Description = Names.PROJECT_DESCRIPTION,
            Summary = Names.PROJECT_DESCRIPTION,
            Copyright = Names.PROJECT_COPYRIGHTS,
            Tags = Names.PROJECT_TAGS,
            LicenseUrl = Paths.LICENSE_URL,
            ProjectUrl = Paths.PROJECT_URL,
            OutputDirectory = Paths.ARTIFACTS_OUTPUT,
            BasePath = Paths.BUILD_OUTPUT,
            IconUrl = new Uri("https://cdn.jsdelivr.net/gh/cake-contrib/graphics/png/cake-contrib-medium.png"),
            Repository = new NuGetRepository {
                Type = "git",
                Commit = _version.Sha,
                Branch = _version.BranchName,
                Url = Paths.SOURCE_URL
            },
            Files = new [] {
                new NuSpecContent {Source = Names.PROJECT_ID + ".dll", Target = netStandardTarget},
                new NuSpecContent {Source = Names.PROJECT_ID + ".pdb", Target = netStandardTarget},
                new NuSpecContent {Source = Names.PROJECT_ID + ".xml", Target = netStandardTarget},
            }
        };

        // Release notes
        if(FileExists(Paths.RELEASE_NOTES_FILE)){
            var releaseNote = ParseReleaseNotes(Paths.RELEASE_NOTES_FILE);
            var notes = releaseNote.Notes.ToList();
            notes.Insert(0, "# " + releaseNote.Version);
            nugetSettings.ReleaseNotes = notes;
        }

        NuGetPack(nugetSettings);
    });

Task("_deployNugetPackage")
    .Description("Deploy created nuget package to repository")
    .WithCriteria(!string.IsNullOrEmpty(_nugetApiToken))
    .WithCriteria(!string.IsNullOrEmpty(_nugetUri))
    .Does(() => {
        var nugetFiles = GetFiles(Paths.ARTIFACTS_OUTPUT + "*.nupkg");

        var pushSettings = new NuGetPushSettings {
            ApiKey = _nugetApiToken,
            Source = _nugetUri
        };

        NuGetPush(nugetFiles, pushSettings);

    });

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("RunTests")
    .IsDependentOn("_cleanup")
    .IsDependentOn("_restore")
    .IsDependentOn("_buildTests")
    .IsDependentOn("_runTests")
    .IsDependentOn("_startSonarQube")
    .IsDependentOn("_buildLibraryForSonar")
    .IsDependentOn("_endSonarQube");

Task("BuildAndPackLibrary")
    .IsDependentOn("_cleanup")
    .IsDependentOn("_restore")
    .IsDependentOn("_fixVersion")
    .IsDependentOn("_buildLibraries")
    .IsDependentOn("_createNuGetPackage");

Task("DeployLibrary")
    .IsDependentOn("_deployNugetPackage");

Task("AppVeyor")
    .IsDependentOn("RunTests")
    .IsDependentOn("BuildAndPackLibrary");

Task("Default")
    .IsDependentOn("RunTests")
    .IsDependentOn("BuildAndPackLibrary");

RunTarget(target);