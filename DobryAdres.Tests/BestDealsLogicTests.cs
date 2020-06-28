using NUnit.Framework;
using Models;
using DobryAdres;
using System.Collections.Generic;

namespace DobryAdres.Tests
{
    [TestFixture]
    public class BestDealsLogicTests
    {
        private List<Entry> _allSzczecinEntries;
        private string _citySzczecin;
        private BestDealsLogic _sut;

        [SetUp]
        public void Setup()
        {
            _allSzczecinEntries = new List<Entry>
            {
                new Entry { 
                    PropertyPrice = new PropertyPrice { PricePerMeter = 1300.0m },
                    PropertyAddress = new PropertyAddress { City = PolishCity.SZCZECIN } },
                new Entry { 
                    PropertyPrice = new PropertyPrice { PricePerMeter = 1400.0m },
                    PropertyAddress = new PropertyAddress { City = PolishCity.SZCZECIN } },
                new Entry { 
                    PropertyPrice = new PropertyPrice { PricePerMeter = 1100.0m },
                    PropertyAddress = new PropertyAddress { City = PolishCity.SZCZECIN } },
                new Entry { 
                    PropertyPrice = new PropertyPrice { PricePerMeter = 1000.0m },
                    PropertyAddress = new PropertyAddress { City = PolishCity.SZCZECIN } },
                new Entry { 
                    PropertyPrice = new PropertyPrice { PricePerMeter = 1200.0m },
                    PropertyAddress = new PropertyAddress { City = PolishCity.SZCZECIN } },
                new Entry { 
                    PropertyPrice = new PropertyPrice { PricePerMeter = 1500.0m },
                    PropertyAddress = new PropertyAddress { City = PolishCity.SZCZECIN } }
            };
            _citySzczecin = PolishCity.SZCZECIN.ToString();

            _sut = new BestDealsLogic();
        }

        [Test]
        public void GetBestDeals_NoEntries_ReturnNull()
        {
            // Arrange
            var _emptyEntries = new List<Entry>();

            // Act
            var result = _sut.FindBestDeals(_emptyEntries, _citySzczecin);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetBestDeals_CityDoesntExist_ReturnNull()
        {
            // Arrange
            var _madeUpCity = "neverland";

            // Act
            var result = _sut.FindBestDeals(_allSzczecinEntries, _madeUpCity);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetBestDeals_SixOffersFromTheSameCity_ReturnMaxFiveBestDeals()
        {
            // Act
            var result = _sut.FindBestDeals(_allSzczecinEntries, _citySzczecin);

            // Assert
            Assert.LessOrEqual(result.Count, 5);
        }

        [Test]
        public void GetBestDeals_SixOffersFromTheSameCity_ReturnBestDealsInOrder()
        {
            // Act
            var result = _sut.FindBestDeals(_allSzczecinEntries, _citySzczecin);

            // Assert
            Assert.IsTrue(result[0].PropertyPrice.PricePerMeter == 1000.0m);
            Assert.IsTrue(result[1].PropertyPrice.PricePerMeter == 1100.0m);
            Assert.IsTrue(result[2].PropertyPrice.PricePerMeter == 1200.0m);
            Assert.IsTrue(result[3].PropertyPrice.PricePerMeter == 1300.0m);
            Assert.IsTrue(result[4].PropertyPrice.PricePerMeter == 1400.0m);
        }

        [Test]
        public void GetBestDeals_MissingPricePerMeter_ReturnOnlyOffersWithKnownPricePerMeter()
        {
            // Arrange
            var _missingPricePerMeterEntries = new List<Entry>
            {
                new Entry {
                    PropertyPrice = new PropertyPrice { PricePerMeter = 1000.0m },
                    PropertyAddress = new PropertyAddress { City = PolishCity.SZCZECIN } },
                new Entry {
                    PropertyPrice = new PropertyPrice { PricePerMeter = 0.0m },
                    PropertyAddress = new PropertyAddress { City = PolishCity.SZCZECIN } },
                new Entry {
                    PropertyPrice = new PropertyPrice { PricePerMeter = 2000.0m },
                    PropertyAddress = new PropertyAddress { City = PolishCity.SZCZECIN } },
                new Entry {
                    PropertyPrice = new PropertyPrice { PricePerMeter = 0.0m },
                    PropertyAddress = new PropertyAddress { City = PolishCity.SZCZECIN } },
                new Entry {
                    PropertyPrice = new PropertyPrice { PricePerMeter = 4000.0m },
                    PropertyAddress = new PropertyAddress { City = PolishCity.SZCZECIN } },
                new Entry {
                    PropertyPrice = new PropertyPrice { PricePerMeter = 5000.0m },
                    PropertyAddress = new PropertyAddress { City = PolishCity.SZCZECIN } }
            };

            // Act
            var result = _sut.FindBestDeals(_missingPricePerMeterEntries, _citySzczecin);

            // Assert
            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(result[0].PropertyPrice.PricePerMeter == 1000.0m);
            Assert.IsTrue(result[1].PropertyPrice.PricePerMeter == 2000.0m);
            Assert.IsTrue(result[2].PropertyPrice.PricePerMeter == 4000.0m);
            Assert.IsTrue(result[3].PropertyPrice.PricePerMeter == 5000.0m);
        }
    }
}