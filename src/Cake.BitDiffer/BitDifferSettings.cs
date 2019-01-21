using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.BitDiffer
{
    /// <summary>
    ///     Settings used by <see cref="BitDifferRunner" />
    ///     Read more about configuration options on BitDiffer project
    ///     (https://github.com/bitdiffer/bitdiffer/wiki/ConsoleApplication)
    /// </summary>
    public sealed class BitDifferSettings : ToolSettings
    {
        /// <summary>
        ///     File name for the previous version of the assembly to compare
        /// </summary>
        public FilePath PreviousAssemblyFile { get; set; }

        /// <summary>
        ///     File name for the current version of the assembly to compare
        /// </summary>
        public FilePath CurrentAssemblyFile { get; set; }

        /// <summary>
        ///     Report only assemblies with changes (DEFAULT: true)
        /// </summary>
        public bool ReportOnlyChanged { get; set; } = true;

        /// <summary>
        ///     Compare only public members (DEFAULT: false)
        /// </summary>
        public bool CompareOnlyPublic { get; set; } = false;

        /// <summary>
        ///     Compare implementation of methods and properties (DEFAULT: true)
        /// </summary>
        public bool CompareImplementation { get; set; } = true;

        /// <summary>
        ///     Compare for changes in assembly attribute values
        /// </summary>
        public bool CompareAssemblyAttributeChanges { get; set; } = false;

        /// <summary>
        ///     Result file name (could be HTML or XML file)
        /// </summary>
        public FilePath ResultOutputFile { get; set; }

        /// <summary>
        ///     Isolation level to load assemblies <see cref="IsolationLevel" /> (DEFAULT: Auto)
        /// </summary>
        public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.Auto;

        /// <summary>
        ///     Prefer to load dependent assemblies from GAC instead from current directory (DEFAULT: false)
        /// </summary>
        public bool PreferGacVersion { get; set; } = false;

        /// <summary>
        ///     Load assemblies in reflection context (false: load assembly) (DEFAULT: true)
        /// </summary>
        public bool ReflectionOnlyLoading { get; set; } = true;
    }

    /// <summary>
    ///     Reflection isolation level
    /// </summary>
    public enum IsolationLevel
    {
        /// <summary>
        ///     The automatic mode isolation level will scan the assemblies before loading them, and select the Medium isolation
        ///     level if possible, or revert to the High isolation level if you have multiple assemblies with the same strong name
        ///     in the same directory.
        /// </summary>
        Auto = 0,

        /// <summary>
        ///     The low option creates a single AppDomain to host all external assemblies.
        /// </summary>
        Low = 1,

        /// <summary>
        ///     In medium isolation level, each directory from which assemblies are loaded has it's own AppDomain.
        /// </summary>
        Medium = 2,

        /// <summary>
        ///     In high isolation level, each assembly is loaded into it's own private AppDomain.
        /// </summary>
        High = 3
    }
}