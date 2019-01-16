using Cake.Core;
using Cake.Core.IO;

namespace Cake.BitDiffer
{
    public static class Extensions
    {
        public static string GetNormalizedAbsolutePath(this FilePath path, ICakeEnvironment environment)
        {
            var absolutePath = path.MakeAbsolute(environment).FullPath;
            return environment.Platform.Family == PlatformFamily.Windows
                ? absolutePath.Replace('/', '\\')
                : absolutePath.Replace('\\', '/');
        }

        public static string GetIsolationLevel(this IsolationLevel level)
        {
            switch (level)
            {
                case IsolationLevel.Low:
                    return "-isolation low";

                case IsolationLevel.Medium:
                    return "-isolation medium";

                case IsolationLevel.High:
                    return "-isolation high";

                default:
                    return string.Empty;
            }
        }
    }
}
