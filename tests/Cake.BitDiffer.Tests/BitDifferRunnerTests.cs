using FluentAssertions;
using NUnit.Framework;
using System;

namespace Cake.BitDiffer.Tests
{
    [TestFixture]
    public class BitDifferRunnerTests
    {
        private const string _PREVIOUS = @"./TestFiles/Version1/Cake.BitDiffer.TestAssembly.dll";
        private const string _CURRENT = @"./TestFiles/Version2/Cake.BitDiffer.TestAssembly.dll";

        [Test]
        public void NoSettings_Throws()
        {
            // Arrange
            var sut = new BitDifferFixture { Settings = null };

            // Act
            var sutAction = new Action(() => sut.Run());

            // Assert
            sutAction.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("settings");
        }

        [Theory]
        [TestCase(null)]
        [TestCase("MyAssembly.dll")]
        public void NoPreviousAssemblyFileName_Throws(string assemblyFile)
        {
            // Arrange
            var settings = new BitDifferSettings
            {
                PreviousAssemblyFile = assemblyFile,
                CurrentAssemblyFile = _CURRENT
            };
            var sut = new BitDifferFixture { Settings = settings };

            // Act
            var sutAction = new Action(() => sut.Run());

            // Assert
            sutAction.Should().ThrowExactly<ArgumentException>()
                    .Which.ParamName.Should().Be(nameof(BitDifferSettings.PreviousAssemblyFile));
        }

        [Theory]
        [TestCase(null)]
        [TestCase("MyAssembly.dll")]
        public void NoCurrentAssemblyFileName_Throws(string assemblyFile)
        {
            // Arrange
            var settings = new BitDifferSettings
            {
                CurrentAssemblyFile = assemblyFile,
                PreviousAssemblyFile = _PREVIOUS
            };
            var sut = new BitDifferFixture { Settings = settings };

            // Act
            var sutAction = new Action(() => sut.Run());

            // Assert
            sutAction.Should().ThrowExactly<ArgumentException>()
                .Which.ParamName.Should().Be(nameof(BitDifferSettings.CurrentAssemblyFile));
        }

        [Test]
        public void AssemblyFilesSetAsParameters()
        {
            // Arrange
            var settings = new BitDifferSettings
            {
                CurrentAssemblyFile = _CURRENT,
                PreviousAssemblyFile = _PREVIOUS
            };
            var sut = new BitDifferFixture { Settings = settings };

            // Act
            var result = sut.Run();

            // Assert
            result.Args.Should().EndWith("\"/Working/TestFiles/Version1/Cake.BitDiffer.TestAssembly.dll\" \"/Working/TestFiles/Version2/Cake.BitDiffer.TestAssembly.dll\"");
        }
    }
}
