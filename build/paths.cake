public static class Paths {
    private const string _BASE_PATH = "./BuildOutput/";
    public const string BUILD_OUTPUT = _BASE_PATH + "Build/";
    public const string TEST_OUTPUT = _BASE_PATH + "Tests/";
    public const string ARTIFACTS_OUTPUT = _BASE_PATH + "Artifacts/";
    public const string SONARQUBE_OUTPUT = "./.sonarqube/";

    public const string SOLUTION_FILE = "./Cake.BitDiffer.sln";
    public const string SOLUTION_FILE_FOR_SONAR = "./Cake.BitDiffer.Sonar.sln";

    public const string TEST_PROJECT_FILE = "./tests/Cake.BitDiffer.Tests/Cake.BitDiffer.Tests.csproj";
    public const string TEST_RESULT_FILE = "TestResults.trx";
    public const string TEST_COVERAGE_RESULT_FILE = ARTIFACTS_OUTPUT + "TestCoverage.dcvr";
    public const string TEST_COVERAGE_RESULT_FILE_XML = ARTIFACTS_OUTPUT + "TestCoverage.xml";
    public const string TEST_COVERAGE_RESULT_FILE_HTML = ARTIFACTS_OUTPUT + "TestCoverage.html";
    public const string RELEASE_NOTES_FILE = "./CHANGELOG";
    public const string ASSEMBLY_INFO_FILE = "./src/GlobalAssemblyInfo.cs";

    public static readonly Uri LICENSE_URL = new Uri("https://github.com/WebDucer/Cake.BitDiffer/blob/develop/LICENSE.md");
    public static readonly Uri PROJECT_URL = new Uri("https://github.com/WebDucer/Cake.BitDiffer");
    public const string SOURCE_URL = "https://github.com/WebDucer/Cake.BitDiffer.git";

        /* HELPER */
    public static string Quote(Cake.Core.IO.Path path) {
        if(path == null) {
            return string.Empty;
        }

        var pathString = path.ToString();

        return string.Format("\"{0}\"", pathString.Trim('"'));
    }

}