using HtmlAgilityPack;
using Models;
using Moq;
using Morizon;
using System.Collections.Generic;
using System.IO;
using Utilities;
using Xunit;

namespace MorizonTests {
    public class MorizonIntegrationTests {

        [Fact]
        public void GetEntries_PageDoesNotExist_NoEntriesReturned() {
            //Arrange
            Mock<IMorizonParser> _MorizonParserMock = new Mock<IMorizonParser>();
            _MorizonParserMock.Setup(x => x.GetDocument(It.IsAny<string>())).Returns((HtmlDocument)null);

            MorizonIntegration Morizon = new MorizonIntegration(new DumpFileRepository(), new MorizonComparer());
            List<Entry> entries = new List<Entry>();

            //Act
            entries = Morizon.GetEntries(_MorizonParserMock.Object, "https://www.example.com", "", entries);

            //Assert
            Assert.Empty(entries);
        }

        [Fact]
        public void GetEntries_NoOffersOnPage_NoEntriesReturned() {
            //Arrange
            Mock<IMorizonParser> _MorizonParserMock = new Mock<IMorizonParser>();
            _MorizonParserMock.Setup(x => x.GetDocument(It.IsAny<string>())).Returns(new HtmlDocument());
            _MorizonParserMock.Setup(x => x.GetNextUrl(It.IsAny<HtmlDocument>())).Returns((string)null);

            MorizonIntegration Morizon = new MorizonIntegration(new DumpFileRepository(), new MorizonComparer());
            List<Entry> entries = new List<Entry>();

            //Act
            entries = Morizon.GetEntries(_MorizonParserMock.Object, "https://www.example.com", "", entries);

            //Assert
            Assert.Empty(entries);
        }

        [Fact]
        public void GetEntries_EntryDataReturned_EntryAddedToList() {
            //Arrange
            string html = File.ReadAllText("..\\..\\..\\HTMLPage.html");
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            List<string> propertiesUrls = new List<string>() { "https://www.example.com" };

            Mock<MorizonParser> _MorizonParserMock = new Mock<MorizonParser>();
            _MorizonParserMock.Setup(x => x.GetDocument(It.IsAny<string>())).Returns(new HtmlDocument());
            _MorizonParserMock.Setup(x => x.GetNextUrl(It.IsAny<HtmlDocument>())).Returns((string)null);
            _MorizonParserMock.Setup(x => x.GetPropertiesUrls(It.IsAny<HtmlDocument>())).Returns(propertiesUrls);
            _MorizonParserMock.Setup(x => x.GetProperty(It.IsAny<string>())).Returns(document.DocumentNode);
            _MorizonParserMock.CallBase = true;

            MorizonIntegration Morizon = new MorizonIntegration(new DumpFileRepository(), new MorizonComparer());
            List<Entry> entries = new List<Entry>();

            //Act
            entries = Morizon.GetEntries(_MorizonParserMock.Object, "https://www.example.com", "", entries);

            //Assert
            Assert.Single(entries);
        }

        [Fact]
        public void GetEntries_EntryDataReturned_BasementAndGardenParsedCorrectly() {
            //Arrange
            string html = File.ReadAllText("..\\..\\..\\HTMLPage.html");
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            List<string> propertiesUrls = new List<string>() { "https://www.example.com" };

            Mock<MorizonParser> _MorizonParserMock = new Mock<MorizonParser>();
            _MorizonParserMock.Setup(x => x.GetDocument(It.IsAny<string>())).Returns(new HtmlDocument());
            _MorizonParserMock.Setup(x => x.GetNextUrl(It.IsAny<HtmlDocument>())).Returns((string)null);
            _MorizonParserMock.Setup(x => x.GetPropertiesUrls(It.IsAny<HtmlDocument>())).Returns(propertiesUrls);
            _MorizonParserMock.Setup(x => x.GetProperty(It.IsAny<string>())).Returns(document.DocumentNode);
            _MorizonParserMock.CallBase = true;

            MorizonIntegration Morizon = new MorizonIntegration(new DumpFileRepository(), new MorizonComparer());
            List<Entry> entries = new List<Entry>();

            //Act
            entries = Morizon.GetEntries(_MorizonParserMock.Object, "https://www.example.com", "", entries);

            //Assert
            Assert.Equal(20.00M, entries[0].PropertyFeatures.GardenArea);
            Assert.Equal(50.00M, entries[0].PropertyFeatures.BasementArea);
        }


    }
}
