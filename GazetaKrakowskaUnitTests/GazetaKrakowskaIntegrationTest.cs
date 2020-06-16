using System.Collections.Generic;
using System.Linq;
using GazetaKrakowska;
using Models;
using Moq;
using NUnit.Framework;

namespace GazetaKrakowskaUnitTests
{
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
        public void FetchOfferFromGivenPage__ReturnAllEntriesFromPage__WhenGivenFirstAndExistingPage()
        {
            //Arrange

            //Act
            IEnumerable<Entry> mockEntries = gazetaKrakowskaIntegration.FetchOfferFromGivenPage(1);

            //Assert
            Assert.AreEqual(50, mockEntries.ToList().Count);
            var result1 = mockEntries.ToList<Entry>().First();
            Assert.IsNotNull(result1.OfferDetails.CreationDateTime);
            Assert.IsNotNull(result1.OfferDetails.LastUpdateDateTime);
        }

        [Test]
        public void FetchOfferFromGivenPage__ReturnEmptySet__WhenGivenPageNumberIsTooBig()
        {
            //Arrange

            //Act
            IEnumerable<Entry> mockEntries = gazetaKrakowskaIntegration.FetchOfferFromGivenPage(10000);

            //Assert
            Assert.AreEqual(0, mockEntries.ToList().Count);
        }
    }
}
