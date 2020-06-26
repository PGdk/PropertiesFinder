using HtmlAgilityPack;
using NUnit.Framework;

namespace EchodniaEu.Tests
{
    [TestFixture]
    public class PropertyFeaturesParserTests
    {
        [TestCase("brak ogrodu")]
        [TestCase("bez ogrodu")]
        [TestCase("bez ogródka")]
        [TestCase("bez ogrutka")]
        public void Dump_WithNoGardenArea_0Area(string noGardenString)
        {
            var rawDescriptionNode =
                HtmlNode.CreateNode(
                    $"<div><div class=\"description__rolled\">Lorem ipsum{noGardenString}lorem lorem</div></div>");

            var document = new HtmlDocument();
            document.DocumentNode.AppendChild(rawDescriptionNode);

            var result = new PropertyFeaturesParser(document).Dump();

            Assert.AreEqual(0, result.GardenArea);
        }

        [TestCase("ogrod o powierzchni 20,34m2")]
        [TestCase("ogrod [m2]: 20,34m2")]
        [TestCase("ogrod o powieszhni 20,34m2")]
        [TestCase("ogrod 20,34m2")]
        public void Dump_WithGardenArea_CorrectGardenArea(string gardenString)
        {
            var rawDescriptionNode =
                HtmlNode.CreateNode(
                    $"<div><div class=\"description__rolled\">Lorem ipsum{gardenString}lorem lorem</div></div>");

            var document = new HtmlDocument();
            document.DocumentNode.AppendChild(rawDescriptionNode);

            var result = new PropertyFeaturesParser(document).Dump();

            Assert.AreEqual(20.34m, result.GardenArea);
        }
    }
}