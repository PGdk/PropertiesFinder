using System.Collections.Generic;
using System.Linq;
using DatabaseConnection;
using IntegrationSprzedajemyService;
using Models;
using Moq;
using NUnit.Framework;

namespace SprzedajemyTests
{
    public class SprzedajemyServiceTests
    {
        private SprzedajemyService sprzedajemyService;
        private Mock<ISprzedajemyRepository> dbRepositoryMock;

        [SetUp]
        public void Setup()
        {
            dbRepositoryMock = new Mock<ISprzedajemyRepository>();
            sprzedajemyService = new SprzedajemyService(dbRepositoryMock.Object);
        }

        [Test]
        public void GetSpecialOffers_CalculateProperOrder_WhenEntriesHaveSpecialProperties()
        {
            //Arrange
            List<Entry> offers = new List<Entry>();

            offers.Add(PrepareMockOffer("street1", 10001, 0, 0));
            offers.Add(PrepareMockOffer(null, 9999, 1, 0));
            offers.Add(PrepareMockOffer(null, 8000, 0, 2));
            offers.Add(PrepareMockOffer("street2", 8001, 1, 1));
            offers.Add(PrepareMockOffer("street3", 0, 1, 1));

            IEnumerable<Entry> mockEntries = offers;

            dbRepositoryMock.Setup(m => m.GetEntries()).Returns(mockEntries);

            //Act
            IEnumerable<Entry> specialEntries = sprzedajemyService.GetSpecialOffers();

            //Assert
            var results = specialEntries.ToList<Entry>();

            Assert.IsTrue(results.Count == 5);

            var result1 = results.First();
            var result2 = results.ElementAt(1);
            var result5 = results.ElementAt(4);

            Assert.AreEqual("street2", result1.PropertyAddress.StreetName);
            Assert.AreEqual(1, result1.PropertyFeatures.Balconies);
            Assert.AreEqual(1, result1.PropertyFeatures.OutdoorParkingPlaces);
            Assert.AreEqual(8001, result1.PropertyPrice.PricePerMeter);

            Assert.AreEqual(null, result2.PropertyAddress.StreetName);
            Assert.AreEqual(2, result2.PropertyFeatures.OutdoorParkingPlaces);
            Assert.AreEqual(8000, result2.PropertyPrice.PricePerMeter);

            Assert.AreEqual("street1", result5.PropertyAddress.StreetName);
            Assert.AreEqual(0, result5.PropertyFeatures.Balconies);
            Assert.AreEqual(0, result5.PropertyFeatures.OutdoorParkingPlaces);
            Assert.AreEqual(10001, result5.PropertyPrice.PricePerMeter);

        }

        [Test]
        public void CalculatePoints__ReturnZero__WhenEntryDoesNotHaveExtraFeatures()
        {
            //Act
            var points = sprzedajemyService.CalculatePoints(PrepareMockOffer(null, 0, 0, 0));

            //Assert
            Assert.Zero(points);

        }

        [Test]
        public void CalculatePoints__CalculateProperly__AlthoughPropertiesAreNull()
        {
            //Act
            var points = sprzedajemyService.CalculatePoints(new Entry());

            //Assert
            Assert.Zero(points);
        }

        [Test]
        public void GetSpecialOffers__ReturnNull__WhenNoEntriesInDb()
        {
            //Act
            IEnumerable<Entry> specialEntries = sprzedajemyService.GetSpecialOffers();

            //Assert
            Assert.IsNull(specialEntries);
        }


        private Entry PrepareMockOffer(string streetName, decimal pricePerMeter, int? balconiesNumber, int? outdoorParkingPlaces)
        {
            return new Entry()
            {
                PropertyAddress = CreatePropertyAddress(streetName),
                PropertyFeatures = CreatePropertyFeatures(balconiesNumber, outdoorParkingPlaces),
                PropertyPrice = CreatePropertyPrice(pricePerMeter)
            };
        }

        private PropertyAddress CreatePropertyAddress(string streetName)
        {
            return new PropertyAddress()
            {
                StreetName = streetName
            };
        }

        private PropertyFeatures CreatePropertyFeatures(int? balconiesNumber, int? outdoorParkingPlacesNumber)
        {
            return new PropertyFeatures()
            {
                Balconies = balconiesNumber,
                OutdoorParkingPlaces = outdoorParkingPlacesNumber
            };
        }

        private PropertyPrice CreatePropertyPrice(decimal pricePerMeter)
        {
            return new PropertyPrice()
            {
                PricePerMeter = pricePerMeter
            };
        }
    }
}