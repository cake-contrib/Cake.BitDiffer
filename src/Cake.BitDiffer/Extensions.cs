using Cake.Core;
using Cake.Core.IO;

namespace Cake.BitDiffer
{
    /// <summary>
    ///     Extensions for Add-in
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        ///     Normalization of the file path, depending on OS
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="environment">Cake environment</param>
        /// <returns>Normalized full path of the file</returns>
        public static string GetNormalizedAbsolutePath(this FilePath path, ICakeEnvironment environment)
        {
            string absolutePath = path.MakeAbsolute(environment).FullPath;
            return environment.Platform.Family == PlatformFamily.Windows
                ? absolutePath.Replace('/', '\\')
                : absolutePath.Replace('\\', '/');
        }

        /// <summary>
        ///     Convert isolation level in command line representation
        /// </summary>
        /// <param name="level">Isolation level</param>
        /// <returns>Command line parameter for the level</returns>
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