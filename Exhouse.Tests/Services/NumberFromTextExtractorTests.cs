using System;
using Exhouse.Services;
using NUnit.Framework;

namespace Exhouse.Tests.Services
{
    [TestFixture]
    class NumberFromTextExtractorTests
    {
        [Test]
        public void NumberFromTextExtract_GivenEmptyString_ThrowsException()
        {
            Assert.Throws<FormatException>(() => NumberFromTextExtractor.Extract(""));
        }

        [Test]
        public void NumberFromTextExtract_GivenInvalidString_ThrowsException()
        {
            Assert.Throws<FormatException>(() => NumberFromTextExtractor.Extract("Foo"));
        }

        [Test]
        public void NumberFromTextExtract_GivenValidPositiveNumber_ReturnsValidDecimal()
        {
            decimal result = NumberFromTextExtractor.Extract("2,5");

            Assert.AreEqual(2.5, result);
        }

        [Test]
        public void NumberFromTextExtract_GivenValidNegativeNumber_ReturnsValidDecimal()
        {
            decimal result = NumberFromTextExtractor.Extract("-2,5");

            Assert.AreEqual(-2.5, result);
        }
    }
}
