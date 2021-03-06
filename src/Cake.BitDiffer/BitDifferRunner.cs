﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;

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

            // Issue #4: Redirect the standard output for error detection
            var executionResult = RunProcess(settings, GetArguments(settings),
                new ProcessSettings {RedirectStandardOutput = true});
            executionResult.WaitForExit();

            var output = executionResult.GetStandardOutput()?.ToArray();

            var result = new BitDifferResult(output);

            if (_fileSystem.Exist(_rawFile))
            {
                result = new BitDifferResult(output,
                    XDocument.Load(_rawFile.GetNormalizedAbsolutePath(_environment)));
            }

            RemoveTemporaryFiles(settings);

            return result;
        }

        private void RemoveTemporaryFiles(BitDifferSettings settings)
        {
            // Remove row result
            if (_fileSystem.GetFile(_rawFile).Exists)
            {
                _fileSystem.GetFile(_rawFile).Delete();
            }

            // Remove result file, if not defined by user
            if (string.IsNullOrWhiteSpace(settings.ResultOutputFile?.FullPath))
            {
                _fileSystem.GetFile("./comparison.xml").Delete();
            }
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
                builder.Append("-out")
                    .AppendQuoted(settings.ResultOutputFile.GetNormalizedAbsolutePath(_environment));
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
            return new[] {"BitDiffer.Console.exe"};
        }

        #endregion
    }
}