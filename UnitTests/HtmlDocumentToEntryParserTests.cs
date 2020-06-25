using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using Extensions;
using HtmlAgilityPack;
using Implementation;
using Models;
using NUnit.Framework;
using Utilities;

namespace UnitTests
{
    public class HtmlDocumentToEntryParserTests
    {
        private StreamReader _templateStream;
        readonly IParser<HtmlDocument, Entry> _parser = new HtmlDocumentToEntryParser();

        private readonly IFixture _fixture = new Fixture();
        [SetUp]
        public async Task Setup()
        {
            _templateStream = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "Templates/Template.html"), Encoding.UTF8, true);
        }


        [Test]
        public async Task ShouldLoadTemplate()
        {
            // Arrange
            var expectedEntry = _fixture.Create<EntryTemplateAdapter>();
            var htmlDocument = await GenerateHtmlDocument(expectedEntry);

            // Act
            var entry = _parser.Parse(htmlDocument);

            // Assert
            Assert.That(entry, Is.Not.Null);
        }

        [Test]
        public async Task ShouldParserReturnNullWhenGivenPageIsNotValid()
        {
            // Arrange
            var expectedEntry = _fixture.Create<EntryTemplateAdapter>();
            var htmlDocument = await GenerateHtmlDocument(expectedEntry);

            // Remove contact header
            htmlDocument.DocumentNode.SelectSingleNode("//div[@class='header']/h3").Remove();

            // Act
            var entry = _parser.Parse(htmlDocument);
            
            // Assert
            Assert.That(entry, Is.Null);
        }

        [Test]
        public async Task ShouldParserReturnNullWhenGivenPageIsEmpty()
        {
            // Arrange
            var expectedEntry = _fixture.Create<EntryTemplateAdapter>();

            // Act
            var entry = _parser.Parse(new HtmlDocument());

            // Assert
            Assert.That(entry, Is.Null);
        }

        [Test]
        public async Task ShouldParserReturnValidOfferFeatures()
        {
            // Arrange
            var expectedEntry = _fixture
                .Build<EntryTemplateAdapter>()
                .With(adapter => adapter.OfferDetails, _fixture
                    .Build<OfferDetails>()
                    .With(details => details.Url, "https://" + Guid.NewGuid())
                    .Create())
                .Create();
            
            var htmlDocument = await GenerateHtmlDocument(expectedEntry);

            // Act
            var actualEntry = _parser.Parse(htmlDocument);
            
            //Assert
            Assert.That(actualEntry, Is.Not.Null);
            Assert.That(actualEntry.OfferDetails, Is.Not.Null);
            Assert.That(actualEntry.OfferDetails.Url, Is.EqualTo(expectedEntry.OfferDetails.Url));
            Assert.That(actualEntry.OfferDetails.LastUpdateDateTime, Is.EqualTo(expectedEntry.OfferDetails.LastUpdateDateTime).Within(1).Days); // Offers does not show time
            Assert.That(actualEntry.OfferDetails.CreationDateTime, Is.EqualTo(expectedEntry.OfferDetails.CreationDateTime).Within(1).Days);
            Assert.That(actualEntry.OfferDetails.IsStillValid, Is.EqualTo(expectedEntry.OfferDetails.IsStillValid));
            Assert.That(actualEntry.OfferDetails.OfferKind, Is.EqualTo(expectedEntry.OfferDetails.OfferKind));
        }

        [Test]
        public async Task ShouldParserReturnValidPrice()
        {
            // Arrange
            var expectedEntry = _fixture
                .Build<EntryTemplateAdapter>()
                .Create();

            var htmlDocument = await GenerateHtmlDocument(expectedEntry);

            // Act
            var actualEntry = _parser.Parse(htmlDocument);

            //Assert
            Assert.That(actualEntry, Is.Not.Null);
            Assert.That(actualEntry.PropertyPrice, Is.Not.Null);
            Assert.That(actualEntry.PropertyPrice.TotalGrossPrice, Is.EqualTo(expectedEntry.PropertyPrice.TotalGrossPrice));
            Assert.That(actualEntry.PropertyPrice.PricePerMeter, Is.EqualTo(expectedEntry.PropertyPrice.PricePerMeter));
        }

        [Test]
        public async Task ShouldParserReturnValidDescription()
        {
            // Arrange
            var expectedEntry = _fixture
                .Build<EntryTemplateAdapter>()
                .Create();

            var htmlDocument = await GenerateHtmlDocument(expectedEntry);

            // Act
            var actualEntry = _parser.Parse(htmlDocument);

            //Assert
            Assert.That(actualEntry, Is.Not.Null);
            Assert.That(actualEntry.RawDescription, Is.EqualTo(actualEntry.RawDescription));
        }


        [TearDown]
        public void TearDown()
        {
            _templateStream.Dispose();
        }

        private async Task<HtmlDocument> GenerateHtmlDocument(EntryTemplateAdapter entry)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            var template = await _templateStream.ReadToEndAsync();

            await using var stream = await GenerateStreamFromString(template.FormatWith(entry));

            htmlDocument.Load(stream);
            return htmlDocument;
        }

        public static async Task<Stream> GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            await writer.WriteAsync(s);
            await writer.FlushAsync();
            stream.Position = 0;
            return stream;
        }
    }
}