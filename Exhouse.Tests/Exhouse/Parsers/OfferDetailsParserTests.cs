using Exhouse.Exhouse.Parsers;
using HtmlAgilityPack;
using Models;
using NUnit.Framework;

namespace Exhouse.Tests.Exhouse.Parsers
{
    [TestFixture]
    class OfferDetailsParserTests
    {
        [Test]
        public void OfferDetailsParsing_GivenEmptyContent_ReturnsObjectWithNoValues()
        {
            // Arrange
            HtmlNode documentNode = HtmlNode.CreateNode("<div></div>");

            // Act
            OfferDetails offerDetails = OfferDetailsParser.Parse(documentNode);

            // Assert
            Assert.AreEqual(OfferKind.RENTAL, offerDetails.OfferKind);
            Assert.IsNull(offerDetails.SellerContact.Telephone);
            Assert.IsNull(offerDetails.SellerContact.Name);
        }

        [Test]
        public void OfferDetailsParsing_GivenValidHtml_ReturnsObjectWithNoValues()
        {
            // Arrange
            HtmlNode documentNode = HtmlNode.CreateNode("<div><div class=\"col-md-8 offer-head-top\"><h2 class=\"small\">sprzedaż</h2></div><a class=\"agent-mobile\">Foo</a><div class=\"agent-name\">Bar</div></div>");

            // Act
            OfferDetails offerDetails = OfferDetailsParser.Parse(documentNode);

            // Assert
            Assert.AreEqual(OfferKind.SALE, offerDetails.OfferKind);
            Assert.AreEqual("Foo", offerDetails.SellerContact.Telephone);
            Assert.AreEqual("Bar", offerDetails.SellerContact.Name);
        }
    }
}
