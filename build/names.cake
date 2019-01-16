public static class Names {
    public const string DEFAULT_CONFIGURATION = "Release";
    public const string SONARCUBE_API_TOKEN = "CI_SONAR_TOKEN";
    public const string SONARQUBE_URI = "CI_SONAR_URL";
    public const string NUGET_URI = "NUGET_REPO_URL";
    public const string NUGET_API_TOKEN = "NUGET_API_KEY";

    public const string PROJECT_ID = "Cake.BitDiffer";

    public const string PROJECT_TITLE = "Cake Addin for using BitDiffer coimmand line tool";
    public static readonly string[] PROJECT_AUTHORS = {"Eugen [WebDucer] Richter"};
    public static readonly string[] PROJECT_OWNERS = {"Eugen [WebDucer] Richter"};
    public const string PROJECT_DESCRIPTION = @"Cake Addin for using BitDiffer coimmand line tool";
    public static readonly string PROJECT_COPYRIGHTS = string.Format("MIT - (c) {0} Eugen [WebDucer] Richter", DateTime.Now.Year);
    public static readonly string[] PROJECT_TAGS = {"Cake", "Addin", "BitDiffer", "WebDucer"};

    public const string SONARQUBE_ORGANISATION = "webducer-oss";
}