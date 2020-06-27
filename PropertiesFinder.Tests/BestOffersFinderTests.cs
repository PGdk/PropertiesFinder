using Models;
using NUnit.Framework;
using System.Collections.Generic;
using Bazos;

namespace PropertiesFinder.Tests
{
    [TestFixture]
    public class BestOffersFinderTests
    {
        List<Entry> lessThan5EntriesList;
        List<Entry> moreThan5EntriesList;
        List<Entry> badEntriesList;

        [SetUp]
        public void Setup()
        {
            lessThan5EntriesList = Bazos.MockEntriesLists.MakeASmallList();
            moreThan5EntriesList = Bazos.MockEntriesLists.MakeALongList();
            badEntriesList = Bazos.MockEntriesLists.MakeABadList();
        }

        [Test]
        public void GetBestOffers_ExtractingBestOffersFromProvidedList_ReturnedBestEntries()
        {
            //Act
            var bestEntries = BestOffersFinder.GetBestOffers(lessThan5EntriesList);

            //Assert
            Assert.IsTrue(bestEntries.Count>0);
            Assert.IsTrue(bestEntries[0].OfferDetails.OfferKind == OfferKind.RENTAL);
            Assert.IsTrue(bestEntries[0].PropertyFeatures.IndoorParkingPlaces > 0 || bestEntries[0].PropertyFeatures.OutdoorParkingPlaces > 0);
            Assert.IsTrue(bestEntries[0].PropertyFeatures.Balconies > 0);
        }

        [Test]
        public void GetBestOffers_ExtractingBestOffersFromProvidedList_ReturnedLessThan6Entries()
        {
            //Act
            var bestEntries = BestOffersFinder.GetBestOffers(moreThan5EntriesList);

            //Assert
            Assert.IsTrue(bestEntries.Count < 6);
        }

        [Test]
        public void GetBestOffers_ProvidedEmptyList_ReturnNull()
        {
            //Arrange
            List<Entry> entries = new List<Entry>();

            //Act
            var bestEntries = BestOffersFinder.GetBestOffers(entries);

            //Assert
            Assert.IsTrue(bestEntries==null);
        }

        [Test]
        public void GetBestOffers_NoMatchingOffersFound_ReturnNull()
        {
            //Act
            var bestEntries = BestOffersFinder.GetBestOffers(badEntriesList);

            //Assert
            Assert.IsTrue(bestEntries == null);
        }

       
    }
}