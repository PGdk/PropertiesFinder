using Exhouse.Services;
using NUnit.Framework;

namespace Exhouse.Tests.Services
{
    [TestFixture]
    class DiacriticsRemoverTests
    {
        [Test]
        public void RemoveDiacritics_GivenEmptyString_ReturnsEmptyString()
        {
            // Act
            string result = DiacriticsRemover.Remove(string.Empty);

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void RemoveDiacritics_GivenNormalString_ReturnsEmptyString()
        {
            // Arrange
            string text = "Foo bar";

            // Act
            string result = DiacriticsRemover.Remove(text);

            // Assert
            Assert.AreEqual(text, result);
        }

        [Test]
        public void RemoveDiacritics_GivenStringWithDiacritics_ReturnsStringWithNoDiacritics()
        {
            // Act
            string result = DiacriticsRemover.Remove("Idę drogą tupiąc nogą");

            // Assert
            Assert.AreEqual("Ide droga tupiac noga", result);
        }
    }
}
