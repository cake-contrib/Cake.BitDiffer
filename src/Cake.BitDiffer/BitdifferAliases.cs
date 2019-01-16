using Cake.Core;
using Cake.Core.Annotations;
using System;

namespace Cake.BitDiffer
{
    /// <summary>
    ///     <para>
    ///         Contains functionality for running a BitDiffer analysis on assemblies
    ///     </para>
    ///     <para>
    ///         In order to use commands for this addin, inclide the vollowing in you <c>build.cake</c> file.
    ///     </para>
    ///     <code>
    ///     #addin "nuget:?package=Cake.BitDiffer"
    ///     #tool "nuget:?package=BitDiffer"
    /// </code>
    /// </summary>
    [CakeAliasCategory("BitDiffer")]
    public static class BitDifferAliases
    {
        /// <summary>
        ///     <para>
        ///         Compare assemblies with BitDiffer
        ///     </para>
        /// </summary>
        /// <example>
        /// <code>
        /// var settings = new BitDifferSettings {
        ///     PreviousAssemblyFile = "./Version1/MyAsembly.dll",
        ///     CurrentAssemblyFile = "./Version2/MyAssembly.dll"
        /// };
        /// BitDiffer(settings);
        /// </code>
        /// </example>
        /// <param name="context">The Cake context</param>
        /// <param name="settings">Settings for BitDiffer</param>
        [CakeMethodAlias]
        public static BitDifferResult BitDiffer(this ICakeContext context, BitDifferSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            var runner = new BitDifferRunner(context.FileSystem, context.Environment, context.ProcessRunner,
                context.Tools);
            return runner.Run(settings);
        }

        /// <summary>
        ///     <para>
        ///         Compare assemblies with BitDiffer
        ///     </para>
        /// </summary>
        /// <example>
        /// <code>
        /// BitDiffer(options => {
        ///     options.PreviousAssemblyFile = "./Version1/MyAsembly.dll";
        ///     options.CurrentAssemblyFile = "./Version2/MyAssembly.dll"
        ///  });
        /// </code>
        /// </example>
        /// <param name="context">The Cake context</param>
        /// <param name="settings">Action to generate settings</param>
        [CakeMethodAlias]
        public static BitDifferResult BitDiffer(this ICakeContext context, Action<BitDifferSettings> settings)
        {
            var defaultSettings = new BitDifferSettings();
            settings(defaultSettings);
            return BitDiffer(context, defaultSettings);
        }
    }
}