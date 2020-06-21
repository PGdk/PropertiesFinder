using System.Collections.Generic;
using System.Linq;
using GazetaKrakowska;
using Models;
using Moq;
using NUnit.Framework;

namespace GazetaKrakowskaUnitTests
{
    [TestFixture]
    public class GazetaKrakowskaIntegrationTest
    {
        private GazetaKrakowskaIntegration gazetaKrakowskaIntegration;
        private Mock<IDumpsRepository> mockDumpRepository;
        private Mock<IEqualityComparer<Entry>> mockEqualityComparer;

        [SetUp]
        public void Setup()
        {
            //Arrange
            mockDumpRepository = new Mock<IDumpsRepository>();
            mockEqualityComparer = new Mock<IEqualityComparer<Entry>>();
            gazetaKrakowskaIntegration = new GazetaKrakowskaIntegration(mockDumpRepository.Object, mockEqualityComparer.Object);
        }

        [Test]
        public void FetchOfferFromGivenPage__WhenGivenFirstAndExistingPage__ReturnAllEntriesFromPage()
        {
            //Act
            IEnumerable<Entry> mockEntries = gazetaKrakowskaIntegration.FetchOfferFromGivenPage(1);

            //Assert
            Assert.AreEqual(50, mockEntries.ToList().Count);
            var result1 = mockEntries.ToList<Entry>().First();
            Assert.IsNotNull(result1.OfferDetails.CreationDateTime);
            Assert.IsNotNull(result1.OfferDetails.LastUpdateDateTime);
        }

        [Test]
        public void FetchOfferFromGivenPage__WhenGivenPageNumberIsTooBig__ReturnEmptySet()
        {
            //Act
            IEnumerable<Entry> mockEntries = gazetaKrakowskaIntegration.FetchOfferFromGivenPage(10000);

            //Assert
            Assert.AreEqual(0, mockEntries.ToList().Count);
        }

        [Test]
        public void GetOffersFromSinglePage__WhenGivenExistingAddress__ReturnAllEntriesFromPage()
        {
            //Act
            List<GazetaKrakowskaOffer> gazetaKrakowskaOffers = gazetaKrakowskaIntegration.GetOffersFromSinglePage("http://gazetakrakowska.pl/ogloszenia/28733,8437,fm,pk.html");

            //Assert
            Assert.AreEqual(50, gazetaKrakowskaOffers.ToList().Count);
            var result1 = gazetaKrakowskaOffers.ToList<GazetaKrakowskaOffer>().First();
            Assert.IsNotNull(result1.UrlDetails);
            Assert.IsNotNull(result1.CreationDateTime);
            Assert.IsNotNull(result1.LastUpdateDateTime);
        }

        [Test]
        public void GetOffersFromSinglePage__WhenGivenNonExistingAddress__ReturnEmptySet()
        {
            //Act
            List<GazetaKrakowskaOffer> mockEntries = gazetaKrakowskaIntegration.GetOffersFromSinglePage("http://gazetakrakowska.pl/ogloszenia/noexist");

            //Assert
            Assert.AreEqual(0, mockEntries.ToList().Count);
        }
    }
}
