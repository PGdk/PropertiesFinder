using AngleSharp;
using AngleSharp.Dom;
using Moq;
using NUnit.Framework;
using SprzedawaczIntegration;
using System;
using System.Collections.Generic;
using System.Text;

namespace SzprzedawaczTests
{
    [TestFixture]
    public class IntegrationTests
    {
        private AngleHelper emptyHelper;
        [SetUp]
        public void Setup()
        {
            var source = "";
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var mockHelper = new Mock<AngleHelper>();
            mockHelper.Setup(h => h.GetParsedHtmlFromUrl("")).Returns(context.OpenAsync(req => req.Content(source)).Result);
            emptyHelper = mockHelper.Object;
        }

        [Test]
        public void TestEmpty()
        {
            var parser = new EntryParser(emptyHelper);
            var entry = parser.GetEntryFromUrl("", Models.OfferKind.SALE);
            Assert.IsTrue(entry == null);
        }
    }
}
