using System;
using Exhouse.Exhouse.Parsers;
using HtmlAgilityPack;
using Models;
using NUnit.Framework;

namespace Exhouse.Tests.Exhouse.Parsers
{
    [TestFixture]
    class PropertyPriceParserTests
    {
        [Test]
        public void PropertyPriceParsing_GivenEmptyContent_ReturnsObjectWithNoValues()
        {
            // Arrange
            HtmlNode documentNode = HtmlNode.CreateNode("<div></div>");

            // Act
            PropertyPrice propertyPrice = PropertyPriceParser.Parse(documentNode);

            // Assert
            Assert.Zero(propertyPrice.TotalGrossPrice);
            Assert.Zero(propertyPrice.PricePerMeter);
            Assert.IsNull(propertyPrice.ResidentalRent);
        }

        [Test]
        public void PropertyPriceParsing_GivenContentWithInvalidValues_ThrowsFormatException()
        {
            // Arrange
            HtmlNode documentNode = HtmlNode.CreateNode("<div class=\"property-box\"><p class=\"cena\">Foo</p></div>");

            // Assert
            Assert.Throws<FormatException>(() => PropertyPriceParser.Parse(documentNode));
        }

        [Test]
        public void PropertyPriceParsing_GivenValidHtml_ReturnsObjectWithValues()
        {
            // Arrange
            HtmlNode documentNode = HtmlNode.CreateNode("<div><div class=\"property-box\"><p class=\"cena\">150,50</p><p class=\"params-short\"><span>25,25</span></p></div><div class=\"propsRow vir_oferta_czynszletni\"><span class=\"propValue\">52,32</span></div></div>");

            // Act
            PropertyPrice propertyPrice = PropertyPriceParser.Parse(documentNode);

            // Assert
            Assert.AreEqual(150.50m, propertyPrice.TotalGrossPrice);
            Assert.AreEqual(25.25m, propertyPrice.PricePerMeter);
            Assert.AreEqual(52.32m, propertyPrice.ResidentalRent);
        }
    }
}
