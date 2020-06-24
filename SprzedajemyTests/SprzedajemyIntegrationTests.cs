using System;
using System.Collections.Generic;
using System.Linq;
using IntegrationSprzedajemy;
using Models;
using Moq;
using NUnit.Framework;

namespace SprzedajemyTests
{
    public class SprzedajemyIntegrationTests
    {
        private Integration sprzedajemyIntegration;
        private Mock<IDumpsRepository> mockDumpRepository;
        private Mock<IEqualityComparer<Entry>> mockEqualityComparer;

        [SetUp]
        public void Setup()
        {
            //Arrange
            mockDumpRepository = new Mock<IDumpsRepository>();
            mockEqualityComparer = new Mock<IEqualityComparer<Entry>>();
            sprzedajemyIntegration = new Integration(mockDumpRepository.Object, mockEqualityComparer.Object);
        }

        [Test]
        public void RetriveEntry_ReturnEntry__WhenOfferUrlAddresIsCorrect()
        {
            //Arrange
            sprzedajemyIntegration.RetrieveOffersByPage(1);


            //Act
            Entry entry = sprzedajemyIntegration.RetriveEntry(sprzedajemyIntegration.offers.First());

            //Assert
            Assert.IsNotNull(entry.OfferDetails);
            Assert.IsNotNull(entry.OfferDetails.SellerContact);
            Assert.IsNotNull(entry.PropertyAddress);
            Assert.IsNotNull(entry.PropertyPrice.TotalGrossPrice);
            Assert.IsNotNull(entry.RawDescription);
        }

        [Test]
        public void GetOffersByPageNum_ReturnEntriesFromLastPage_WhenGivenPageNumberIsOutOfRange()
        {
            //Act
            IEnumerable<Entry> offers = sprzedajemyIntegration.GetOffersByPageNum(1000000);

            //Assert
            Assert.AreNotEqual(0, offers.ToList().Count);
        }

        [Test]
        public void GetOffersByPageNum_ReturnOffersFromPageWithFeatures_WhenGivenPageExists()
        {
            //Act
            IEnumerable<Entry> offers = sprzedajemyIntegration.GetOffersByPageNum(2);

            //Assert
            var results = offers.ToList<Entry>();
            Assert.Greater(results.Count, 0);

            var offer1 = results.First();
            Assert.IsNotNull(offer1.PropertyPrice);
            Assert.IsNotNull(offer1.PropertyAddress);
            Assert.IsNotNull(offer1.PropertyFeatures);
            Assert.IsNotNull(offer1.PropertyDetails);
            Assert.IsNotNull(offer1.OfferDetails);
        }

        [Test]
        public void RetriveEntry_ReturnEmptyObject_WhenOfferUrlAddresIsWrong()
        {
            //Act
            Entry offer = sprzedajemyIntegration.RetriveEntry("http://gazetakrakowska.pl/ogloszenia/noexist");

            //Assert
            Assert.AreEqual(null, offer.OfferDetails);
            Assert.AreEqual(null, offer.PropertyAddress);
            Assert.AreEqual(null, offer.PropertyPrice);
        }
    }
}
