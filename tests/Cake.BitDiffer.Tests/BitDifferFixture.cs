using Cake.Core.Tooling;
using Cake.Testing;
using Cake.Testing.Fixtures;

namespace Cake.BitDiffer.Tests
{
    public sealed class BitDifferFixture : ToolFixture<BitDifferSettings>
    {
        /// <inheritdoc />
        public BitDifferFixture() : base("BitDiffer.Console.exe")
        {
        }

        #region Overrides of ToolFixture<BitDifferSettings,ToolFixtureResult>

        /// <inheritdoc />
        protected override void RunTool()
        {
            FileSystem.CreateFile("TestFiles/Version1/Cake.BitDiffer.TestAssembly.dll");
            FileSystem.CreateFile("TestFiles/Version2/Cake.BitDiffer.TestAssembly.dll");
            var toolLocator = new ToolLocator(Environment, new ToolRepository(Environment),
                new ToolResolutionStrategy(FileSystem, Environment, Globber, new FakeConfiguration()));
            var tool = new BitDifferRunner(FileSystem, Environment, ProcessRunner, toolLocator);
            tool.Run(Settings);
        }

        #endregion
    }
}