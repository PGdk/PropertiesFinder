using System;
using Application.Comparers;
using HtmlAgilityPack;
using Models;
using NUnit.Framework;
using static EchodniaEu.OfferPropertyLabel;

namespace EchodniaEu.Tests
{
    [TestFixture]
    public class PropertyPriceParserTests
    {
        [Test]
        public void Dump_AllFields_FullPriceInfo()
        {
            var totalGrossPrice = 500000;
            var pricePerMeter = 10000;
            var residentialRent = 700;

            var parentNode = HtmlNode.CreateNode("<div></div>");
            parentNode.AppendChild(HtmlNode.CreateNode($"<span class=\"priceInfo__value\">{totalGrossPrice}</span>"));
            parentNode.AppendChild(HtmlNode.CreateNode($"<span class=\"priceInfo__additional\">{pricePerMeter}</span>"));
            parentNode.AppendChild(HtmlNode.CreateNode($"<div><span>{ResidentalRent}</span><b>{residentialRent}</b></div>"));

            var document = new HtmlDocument();
            document.DocumentNode.AppendChild(parentNode);

            var result = new PropertyPriceParser(document).Dump();

            Assert.AreEqual(result, new PropertyPrice()
            {
                TotalGrossPrice = totalGrossPrice,
                PricePerMeter = pricePerMeter,
                ResidentalRent = residentialRent
            });
        }

        [Test]
        public void Dump_SomeFields_PartialPriceInfo()
        {
            var pricePerMeter = 10000;
            var residentialRent = 700;

            var parentNode = HtmlNode.CreateNode("<div></div>");
            parentNode.AppendChild(HtmlNode.CreateNode($"<span class=\"priceInfo__additional\">{pricePerMeter}</span>"));
            parentNode.AppendChild(HtmlNode.CreateNode($"<div><span>{ResidentalRent}</span><b>{residentialRent}</b></div>"));

            var document = new HtmlDocument();
            document.DocumentNode.AppendChild(parentNode);

            var result = new PropertyPriceParser(document).Dump();

            Assert.AreEqual(result, new PropertyPrice()
            {
                PricePerMeter = pricePerMeter,
                ResidentalRent = residentialRent
            });
        }

        [Test]
        public void Dump_NoFields_EmptyPriceInfo()
        {
            var parentNode = HtmlNode.CreateNode("<div></div>");

            var document = new HtmlDocument();
            document.DocumentNode.AppendChild(parentNode);

            var result = new PropertyPriceParser(document).Dump();

            Assert.AreEqual(result, new PropertyPrice());
        }
    }
}