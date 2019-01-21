using FluentAssertions;
using NUnit.Framework;

namespace Cake.BitDiffer.Tests
{
    [TestFixture]
    public class ExtensionTests
    {
        [Theory]
        [TestCase(IsolationLevel.Auto, "")]
        [TestCase(IsolationLevel.Low, "-isolation low")]
        [TestCase(IsolationLevel.Medium, "-isolation medium")]
        [TestCase(IsolationLevel.High, "-isolation high")]
        public void GetIsolationLevel_ConvertInStringParameter(IsolationLevel level, string argument)
        {
            // Arrange

            // Act
            var result = level.GetIsolationLevel();

            // Assert
            result.Should().BeEquivalentTo(argument);
        }
    }
}