using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Cake.BitDiffer
{
    /// <summary>
    ///     Runner for ButDiffer command line tool
    /// </summary>
    public sealed class BitDifferRunner : Tool<BitDifferSettings>
    {
        private readonly ICakeEnvironment _environment;
        private readonly IFileSystem _fileSystem;
        private readonly FilePath _rawFile = $"{Guid.NewGuid()}.xml";

        /// <inheritdoc />
        public BitDifferRunner(IFileSystem fileSystem, ICakeEnvironment environment, IProcessRunner processRunner,
            IToolLocator tools) : base(fileSystem, environment, processRunner, tools)
        {
            _fileSystem = fileSystem;
            _environment = environment;
        }

        /// <summary>
        ///     Execute the BitDiffer command line with given settings
        /// </summary>
        /// <param name="settings">Settings for BitDiffer execution</param>
        /// <returns>Result of the comparison</returns>
        public BitDifferResult Run(BitDifferSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            Run(settings, GetArguments(settings));

            var result = new BitDifferResult();

            if (_fileSystem.Exist(_rawFile))
            {
                result.RawResult = XDocument.Load(_rawFile.GetNormalizedAbsolutePath(_environment));
            }

            return result;
        }

        private ProcessArgumentBuilder GetArguments(BitDifferSettings settings)
        {
            // Check settings
            if (settings.PreviousAssemblyFile == null || !_fileSystem.Exist(settings.PreviousAssemblyFile))
            {
                throw new ArgumentException("Filename for previous version should be set and exists",
                    nameof(settings.PreviousAssemblyFile));
            }

            if (settings.CurrentAssemblyFile == null || !_fileSystem.Exist(settings.CurrentAssemblyFile))
            {
                throw new ArgumentException("Filename for current version should be set and exists",
                    nameof(settings.CurrentAssemblyFile));
            }

            var builder = new ProcessArgumentBuilder();

            // Show only changes
            if (!settings.ReportOnlyChanged)
            {
                builder.Append("-all");
            }

            // Result file
            if (!string.IsNullOrWhiteSpace(settings.ResultOutputFile?.FullPath))
            {
                builder.AppendQuoted("{0} {1}", "-out",
                    settings.ResultOutputFile.GetNormalizedAbsolutePath(_environment));
            }

            // Only public members
            if (settings.CompareOnlyPublic)
            {
                builder.Append("-publiconly");
            }

            // Compare implementation
            if (!settings.CompareImplementation)
            {
                builder.Append("-noimpl");
            }

            // Compare attribute changes
            if (!settings.CompareAssemblyAttributeChanges)
            {
                builder.Append("-noattrs");
            }

            // Isolation level
            builder.Append(settings.IsolationLevel.GetIsolationLevel());

            // GAC
            if (settings.PreferGacVersion)
            {
                builder.Append("-gacfirst");
            }

            // Asselbly load for reflection only
            if (!settings.ReflectionOnlyLoading)
            {
                builder.Append("-execution");
            }

            // Raw result (for final result)
            builder.Append("-raw")
                .AppendQuoted(_rawFile.GetNormalizedAbsolutePath(_environment));

            // Files to compare
            builder.AppendQuoted(settings.PreviousAssemblyFile.GetNormalizedAbsolutePath(_environment));
            builder.AppendQuoted(settings.CurrentAssemblyFile.GetNormalizedAbsolutePath(_environment));

            return builder;
        }

        #region Overrides of Tool<BitDifferSettings>

        /// <summary>
        ///     Get BitDiffer tool name.
        /// </summary>
        /// <returns></returns>
        protected override string GetToolName()
        {
            return "BitDiffer.Console";
        }

        /// <summary>
        ///     Gets the possible names of the BitDiffer tool executable.
        /// </summary>
        /// <returns>The tool executable names</returns>
        protected override IEnumerable<string> GetToolExecutableNames()
        {
            return new[] { "BitDiffer.Console.exe" };
        }

        #endregion
    }
}