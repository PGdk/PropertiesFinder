using nportal.pl;
using System;
using System.IO;
using HtmlAgilityPack;
using Utilities;
using Xunit;

namespace IntegrationApi.Tests
{
    public class nPortalIntegrationTests
    {
        private readonly NportalIntegration _integration;
        private readonly HtmlNode _templateHtmlNode;

        public nPortalIntegrationTests()
        {
            _integration = new NportalIntegration(new DumpFileRepository(), new NportalComparer());
            using (var fs = new FileStream("entry_template.html", FileMode.Open))
            {

                var htmlDocument = new HtmlDocument();
                htmlDocument.Load(fs);
                _templateHtmlNode = new HtmlNode(HtmlNodeType.Document, htmlDocument, 0);
            }
        }
        [Fact]
        public void GetPropertyDetails_ValidHtml_CorrectDetails()
        {
            var featuresTable = _templateHtmlNode.SelectNodes("/html/body/div[2]/div[3]/section[1]/div/section[3]/table/tr");
            var offerFeatures = _integration.FindOfferFeatures(featuresTable);
            var propertyDetails = _integration.GetPropertyDetails(offerFeatures);
            Assert.Equal(60, propertyDetails.Area);
            Assert.Equal(11, propertyDetails.FloorNumber);
            Assert.Equal(1975, propertyDetails.YearOfConstruction);
            Assert.Equal(2, propertyDetails.NumberOfRooms);
        }
        [Fact]
        public void GetPropertyDetails_InvalidHtml_NullReferenceException()
        {
            var featuresTable = _templateHtmlNode.SelectNodes("/html/body/div[2]/div[3]/section[1]");
            Assert.Throws<NullReferenceException>(() => _integration.FindOfferFeatures(featuresTable));
        }

        [Fact]
        public void GetPropertyPrice_ValidHtml_CorrectPrice()
        {
            var featuresTable = _templateHtmlNode.SelectNodes("/html/body/div[2]/div[3]/section[1]/div/section[3]/table/tr");
            var offerFeatures = _integration.FindOfferFeatures(featuresTable);
            var propertyPrice = _integration.GetPropertyPrice(offerFeatures);
            Assert.Equal(329000, propertyPrice.TotalGrossPrice);
        }

        [Fact]
        public void GetPropertyPrice_InvalidHtml_NullReferenceException()
        {
            var featuresTable = _templateHtmlNode.SelectNodes("/html/body/div[2]/div[3]/section[1]/div");
            Assert.Throws<NullReferenceException>(() => _integration.FindOfferFeatures(featuresTable));

        }
    }
}
