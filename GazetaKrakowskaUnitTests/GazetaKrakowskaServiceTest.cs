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
            entries.Add(PrepareMockedEntryWithNoFeature(1));
            entries.Add(PrepareMockedEntryWithNoFeature(2));
            entries.Add(PrepareMockedEntryWithSingleFeature(3, 2016, 9001));
            entries.Add(PrepareMockedEntryWithTwoFeatures(4));
            entries.Add(PrepareMockedEntryWithThreeFeatures(5));

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
            entries.Add(PrepareMockedEntryWithNoFeature(1));
            entries.Add(PrepareMockedEntryWithNoFeature(2));
            entries.Add(PrepareMockedEntryWithNoFeature(3));
            entries.Add(PrepareMockedEntryWithNoFeature(4));
            entries.Add(PrepareMockedEntryWithNoFeature(5));

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
            entries.Add(PrepareMockedEntryWithSingleFeature(1, 10000, 2016));
            entries.Add(PrepareMockedEntryWithSingleFeature(2, 8999, 2010));

            IEnumerable<EntryDb> mockEntries = entries;

            mockRepository.Setup(m => m.GetEntries()).Returns(mockEntries);

            //Act
            IEnumerable<EntryDb> specialEntries = gazetaKrakowskaService.GetSpecialEntries();

            //Assert
            Assert.IsTrue(specialEntries.ToList().Count == 2);

            var result1 = specialEntries.ToList<EntryDb>().First();

            Assert.AreEqual(2, result1.Id);
            Assert.AreEqual(2010, result1.PropertyDetails.YearOfConstruction);
            Assert.AreEqual(null, result1.PropertyFeatures.IndoorParkingPlaces);
            Assert.AreEqual(8999, result1.PropertyPrice.PricePerMeter);
        }


        private EntryDb PrepareMockedEntryWithSingleFeature(int id, int price, int year)
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
                PropertyFeatures = new PropertyFeaturesDb(),
                PropertyPrice = new PropertyPriceDb()
                {
                    Id = id,
                    PricePerMeter = price
                },
                RawDescription = "someDescription"
            };
        }

        private EntryDb PrepareMockedEntryWithTwoFeatures(int id)
        {
            return new EntryDb()
            {
                Id = id,
                OfferDetails = new OfferDetailsDb(),
                PropertyAddress = new PropertyAddressDb(),
                PropertyDetails = new PropertyDetailsDb()
                {
                    Id = id,
                    YearOfConstruction = 2017

                },
                PropertyFeatures = new PropertyFeaturesDb(),
                PropertyPrice = new PropertyPriceDb()
                {
                    Id = id,
                    PricePerMeter = 8000
                },
                RawDescription = "someDescription"
            };
        }

        private EntryDb PrepareMockedEntryWithThreeFeatures(int id)
        {
            return new EntryDb()
            {
                Id = id,
                OfferDetails = new OfferDetailsDb(),
                PropertyAddress = new PropertyAddressDb(),
                PropertyDetails = new PropertyDetailsDb()
                {
                    Id = id,
                    YearOfConstruction = 2020

                },
                PropertyFeatures = new PropertyFeaturesDb()
                {
                    Id = id,
                    IndoorParkingPlaces = 1
                },
                PropertyPrice = new PropertyPriceDb()
                {
                    Id = id,
                    PricePerMeter = 8999
                },
                RawDescription = "someDescription"
            };
        }

        private EntryDb PrepareMockedEntryWithNoFeature(int id)
        {
            return new EntryDb()
            {
                Id = id,
                OfferDetails = new OfferDetailsDb(),
                PropertyAddress = new PropertyAddressDb(),
                PropertyDetails = new PropertyDetailsDb()
                {
                    Id = id,
                    YearOfConstruction = 2010

                },
                PropertyFeatures = new PropertyFeaturesDb()
                {
                    Id = id,
                    IndoorParkingPlaces = 0
                },
                PropertyPrice = new PropertyPriceDb()
                {
                    Id = id,
                    PricePerMeter = 15000
                },
                RawDescription = "someDescription"
            };
        }
    }
}
