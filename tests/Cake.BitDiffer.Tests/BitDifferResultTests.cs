using System.Xml.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Cake.BitDiffer.Tests
{
    [TestFixture]
    public class BitDifferResultTests
    {
        #region Fields

        private static readonly string[] _DEFAULT_OUTPUT = {
            "Version 1.5.0.4 (29.05.2019)", @"Loading assembly C:\src\MyDll1.dll",
            @"Loading assembly C:\src\MyDll2.dll", @"Writing XML normal report to C:\src\comparison.xml",
            @"Writing XML raw report to C:\src\311fa9a8-e6eb-498d-9288-3f8a14fdef96.xml", "Done!"
        };

        #endregion
        [Test]
        public void HasChanges_WithNullXmlAndNonErrorOutput_False()
        {
            // Arrange
            var sut = new BitDifferResult(_DEFAULT_OUTPUT);

            // Act
            var result = sut.HasChanges();

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void HasChanges_WithNullXmlAndOutput_False()
        {
            // Arrange
            var sut = new BitDifferResult(null, null);

            // Act
            var result = sut.HasChanges();

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void HasChanges_WithEmptyXmlAndErrorOutput_True()
        {
            // Arrange
            var output = new[]
            {
                "Version 1.5.0.4 (29.05.2019)", @"Loading assembly C:\src\MyDll1.dll",
                @"Loading assembly C:\src\MyDll2.dll", @"Writing XML normal report to C:\src\comparison.xml",
                @"Writing XML raw report to C:\src\311fa9a8-e6eb-498d-9288-3f8a14fdef96.xml",
                @"ERROR : Die Datei oder Assembly ""Syncfusion.Licensing, Version=16.2.0.46, Culture=neutral, PublicKeyToken=null"" oder eine Abhängigkeit davon wurde nicht gefunden. Das System kann die angegebene Datei nicht finden.",
                "Done!"
            };
            var sut = new BitDifferResult(output, new XDocument());

            // Act
            var result = sut.HasChanges();
            var message = sut.GetChangeMessage();

            // Assert
            result.Should().BeTrue();
            message.Should().Contain("ERROR");
        }

        [Test]
        public void HasChanges_WithEmptyXml_False()
        {
            // Arrange
            var sut = new BitDifferResult(_DEFAULT_OUTPUT, new XDocument());

            // Act
            var result = sut.HasChanges();

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void HasChanges_WithMemberChangeBreakingXml_True()
        {
            // Arrange
            var rawXml = XDocument.Load("TestFiles/MemberChangeBreaking.xml");
            var sut = new BitDifferResult(_DEFAULT_OUTPUT, rawXml);

            // Act
            var result = sut.HasChanges();
            var message = sut.GetChangeMessage();

            // Assert
            result.Should().BeTrue();
            message.Should().Contain("MembersChangedBreaking");
        }

        [Test]
        public void HasChanges_WithNoChangesXml_False()
        {
            // Arrange
            var rawXml = XDocument.Load("TestFiles/NoChanges.xml");
            var sut = new BitDifferResult(_DEFAULT_OUTPUT, rawXml);

            // Act
            var result = sut.HasChanges();
            var message = sut.GetChangeMessage();

            // Assert
            result.Should().BeFalse();
            message.Should().BeNullOrEmpty();
        }
    }
}