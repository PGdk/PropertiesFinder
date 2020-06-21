using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseConnection;
using IntegrationApi.Services;
using Models;
using Moq;
using NUnit.Framework;

namespace GazetaKrakowskaUnitTests
{
    [TestFixture]
    public class GazetaKrakowskaServiceTest
    {
        private GazetaKrakowskaService gazetaKrakowskaService;
        private Mock<IGazetaKrakowskaRepository> mockRepository;

        [SetUp]
        public void Setup()
        {
            //Arrange
            mockRepository = new Mock<IGazetaKrakowskaRepository>();
            gazetaKrakowskaService = new GazetaKrakowskaService(mockRepository.Object);
        }

        [Test]
        public void GetSpecialEntries__ReturnOnlySpecialEntries__WhenSpecialAndOrdinaryEntriesExist()
        {
            //Arrange
            List<EntryDb> entries = new List<EntryDb>();
            entries.Add(PrepareMockedEntryWithGivenFeatures(1, 10000, null, null));
            entries.Add(PrepareMockedEntryWithGivenFeatures(2, 10000, null, null));
            entries.Add(PrepareMockedEntryWithGivenFeatures(3, 9001, 2016, null));
            entries.Add(PrepareMockedEntryWithGivenFeatures(4, 8001, 2017, 0));
            entries.Add(PrepareMockedEntryWithGivenFeatures(5, 8999, 2020, 1));

            IEnumerable<EntryDb> mockEntries = entries;

            mockRepository.Setup(m => m.GetEntries()).Returns(mockEntries);

            //Act
            IEnumerable<EntryDb> specialEntries = gazetaKrakowskaService.GetSpecialEntries();

            //Assert
            Assert.IsTrue(specialEntries.ToList().Count == 3);

            var result1 = specialEntries.ToList<EntryDb>().First();

            Assert.AreEqual(5, result1.Id);
            Assert.AreEqual(2020, result1.PropertyDetails.YearOfConstruction);
            Assert.AreEqual(1, result1.PropertyFeatures.IndoorParkingPlaces);
            Assert.AreEqual(8999, result1.PropertyPrice.PricePerMeter);

        }

        [Test]
        public void GetSpecialEntries__ReturnNull__WhenOnlyOrdinaryEntriesExist()
        {
            //Arrange
            List<EntryDb> entries = new List<EntryDb>();
            entries.Add(PrepareMockedEntryWithGivenFeatures(1, 10000, null, null));
            entries.Add(PrepareMockedEntryWithGivenFeatures(2, 10000, null, null));
            entries.Add(PrepareMockedEntryWithGivenFeatures(3, 10000, null, null));
            entries.Add(PrepareMockedEntryWithGivenFeatures(4, 10000, null, null));
            entries.Add(PrepareMockedEntryWithGivenFeatures(5, 10000, null, null));

            IEnumerable<EntryDb> mockEntries = entries;

            mockRepository.Setup(m => m.GetEntries()).Returns(mockEntries);

            //Act
            IEnumerable<EntryDb> specialEntries = gazetaKrakowskaService.GetSpecialEntries();

            //Assert
            Assert.IsNull(specialEntries);

        }

        [Test]
        public void GetSpecialEntries__ReturnNull__WhenNoListOfEntryFromDbIsEmpty()
        {
            //Arrange
            List<EntryDb> entries = new List<EntryDb>();

            IEnumerable<EntryDb> mockEntries = entries;

            mockRepository.Setup(m => m.GetEntries()).Returns(mockEntries);

            //Act
            IEnumerable<EntryDb> specialEntries = gazetaKrakowskaService.GetSpecialEntries();

            //Assert
            Assert.IsNull(specialEntries);
        }

        [Test]
        public void GetSpecialEntries__ReturnCorrectOrder__WhenTwoEntriesHaveOneFeatureAndOtherTypeOfFeature()
        {
            //Arrange
            List<EntryDb> entries = new List<EntryDb>();
            entries.Add(PrepareMockedEntryWithGivenFeatures(1, 10000, 2016, 0));
            entries.Add(PrepareMockedEntryWithGivenFeatures(2, 8999, 2010, 0));

            IEnumerable<EntryDb> mockEntries = entries;

            mockRepository.Setup(m => m.GetEntries()).Returns(mockEntries);

            //Act
            IEnumerable<EntryDb> specialEntries = gazetaKrakowskaService.GetSpecialEntries();

            //Assert
            Assert.IsTrue(specialEntries.ToList().Count == 2);

            var result1 = specialEntries.ToList<EntryDb>().First();

            Assert.AreEqual(2, result1.Id);
            Assert.AreEqual(2010, result1.PropertyDetails.YearOfConstruction);
            Assert.AreEqual(0, result1.PropertyFeatures.IndoorParkingPlaces);
            Assert.AreEqual(8999, result1.PropertyPrice.PricePerMeter);
        }


        private EntryDb PrepareMockedEntryWithGivenFeatures(int id, int price, int? year, int? indoorParkingPlaces)
        {
            return new EntryDb()
            {
                Id = id,
                OfferDetails = new OfferDetailsDb(),
                PropertyAddress = new PropertyAddressDb(),
                PropertyDetails = new PropertyDetailsDb()
                {
                    Id = id,
                    YearOfConstruction = year

                },
                PropertyFeatures = new PropertyFeaturesDb()
                {
                    Id = id,
                    IndoorParkingPlaces = indoorParkingPlaces
                },
                PropertyPrice = new PropertyPriceDb()
                {
                    Id = id,
                    PricePerMeter = price
                },
                RawDescription = "someDescription"
            };
        }
    }
}
