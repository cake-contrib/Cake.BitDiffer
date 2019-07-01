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
            Testing.Fixtures.ToolFixtureResult result = sut.Run();

            // Assert
            result.Args.Should()
                .EndWith(
                    "\"/Working/TestFiles/Version1/Cake.BitDiffer.TestAssembly.dll\" \"/Working/TestFiles/Version2/Cake.BitDiffer.TestAssembly.dll\"");
        }

        [Test]
        public void CompareAssemblyAttributeChanges_NotSet()
        {
            CheckParameterSetting(s => s.CompareAssemblyAttributeChanges = false, "-noattrs", true);
        }

        [Test]
        public void CompareAssemblyAttributeChanges_Set()
        {
            CheckParameterSetting(s => s.CompareAssemblyAttributeChanges = true, "-noattrs", false);
        }

        [Test]
        public void CompareImplementation_NotSet()
        {
            CheckParameterSetting(s => s.CompareImplementation = false, "-noimpl", true);
        }

        [Test]
        public void CompareImplementation_Set()
        {
            CheckParameterSetting(s => s.CompareImplementation = true, "-noimpl", false);
        }

        [Test]
        public void CompareOnlyPublic_NotSet()
        {
            CheckParameterSetting(s => s.CompareOnlyPublic = false, "-publiconly", false);
        }

        [Test]
        public void CompareOnlyPublic_Set()
        {
            CheckParameterSetting(s => s.CompareOnlyPublic = true, "-publiconly", true);
        }

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

        [Test]
        public void ReportOnlyChanges_NotSet()
        {
            CheckParameterSetting(s => s.ReportOnlyChanged = false, "-all", true);
        }

        [Test]
        public void ReportOnlyChanges_Set()
        {
            CheckParameterSetting(s => s.ReportOnlyChanged = true, "-all", false);
        }

        [Test]
        public void ResultOutputFile_NotSet()
        {
            CheckParameterSetting(s => s.ResultOutputFile = string.Empty, "-out", false);
        }

        [Test]
        public void ResultOutputFile_Set()
        {
            CheckParameterSetting(s => s.ResultOutputFile = "./Result.xml", "-out \"/working/result.xml\"", true);
        }

        [Test]
        public void IsolationLevel_SetToAuto()
        {
            CheckParameterSetting(s => s.IsolationLevel = IsolationLevel.Auto, "-isolation", false);
        }

        [Test]
        public void IsolationLevel_SetToLow()
        {
            CheckParameterSetting(s => s.IsolationLevel = IsolationLevel.Low, "-isolation low", true);
        }

        [Test]
        public void IsolationLevel_SetToMedium()
        {
            CheckParameterSetting(s => s.IsolationLevel = IsolationLevel.Medium, "-isolation medium", true);
        }

        [Test]
        public void IsolationLevel_SetToHigh()
        {
            CheckParameterSetting(s => s.IsolationLevel = IsolationLevel.High, "-isolation high", true);
        }

        [Test]
        public void PreferGacVersion_Set()
        {
            CheckParameterSetting(s => s.PreferGacVersion = true, "-gacfirst", true);
        }

        [Test]
        public void PreferGacVersion_NotSet()
        {
            CheckParameterSetting(s => s.PreferGacVersion = false, "-gacfirst", false);
        }

        [Test]
        public void ReflectionOnlyLoading_Set()
        {
            CheckParameterSetting(s => s.ReflectionOnlyLoading = true, "-execution", false);
        }

        [Test]
        public void ReflectionOnlyLoading_NotSet()
        {
            CheckParameterSetting(s => s.ReflectionOnlyLoading = false, "-execution", true);
        }

        private void CheckParameterSetting(Action<BitDifferSettings> settingsChange, string expectedArgument,
            bool shouldContainArgument)
        {
            // Arrange
            var settings = new BitDifferSettings
            {
                CurrentAssemblyFile = _CURRENT,
                PreviousAssemblyFile = _PREVIOUS
            };
            settingsChange(settings); // Configure settings for test
            var sut = new BitDifferFixture { Settings = settings };

            // Act
            Testing.Fixtures.ToolFixtureResult result = sut.Run();

            // Assert
            if (shouldContainArgument)
            {
                result.Args.Should().ContainEquivalentOf(expectedArgument);
            }
            else
            {
                result.Args.Should().NotContainEquivalentOf(expectedArgument);
            }
        }
    }
}