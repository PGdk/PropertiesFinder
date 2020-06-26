using DatabaseConnection.Interfaces;
using IntegrationApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Models;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace IntegrationApiTests
{
    [TestFixture]
    public class BargainTest
    {
        private EntriesController _entriesController;
        private Mock<IDatabaseService> _databaseService;
        private Entry _bestBargainWroclaw;

        [SetUp]
        public void Setup()
        {
            _databaseService = new Mock<IDatabaseService>();

            _bestBargainWroclaw = CreateEntry(PolishCity.WROCLAW, 200, 2020, 4, 2, 1);
            List<Entry> entries = new List<Entry>
            {
                CreateEntry(PolishCity.WROCLAW, 200, 2020, 4, 1, 1),
                CreateEntry(PolishCity.ALEKSANDROW_LODZKI, 1, 2020, 4, 1, 1),
                _bestBargainWroclaw,
                CreateEntry(PolishCity.WROCLAW, 200, 2020, 2, 2, 2),
                CreateEntry(PolishCity.WROCLAW, 500, 2019, 3, 100, 1),
                CreateEntry(PolishCity.WROCLAW, 300, 2020, 5, 1, 1),
                CreateEntry(PolishCity.WROCLAW, 300, 2020, 1, 1, 1),
                CreateEntry(PolishCity.WROCLAW, 300, 2020, 1, 1, 1),
                CreateEntry(PolishCity.WROCLAW, 300, 2020, 3, 1, 1),
                CreateEntry(PolishCity.WROCLAW, 300, 2020, 5, 1, 1),
                CreateEntry(PolishCity.WROCLAW, 300, 2020, 6, 1, 1),
                CreateEntry(PolishCity.WROCLAW, 800, 2020, 7, 1, 1)
            };

            _databaseService.Setup(x => x.GetEntries()).Returns(entries);
        }

        private Entry CreateEntry(PolishCity city, int pricePerMeter, int yearOfConstruction, int numberOfRooms, int balconies, int basementArea)
        {
            return new Entry
            {
                PropertyAddress = new PropertyAddress
                {
                    City = city
                },
                PropertyDetails = new PropertyDetails()
                {
                    NumberOfRooms = numberOfRooms,
                    YearOfConstruction = yearOfConstruction
                },
                PropertyPrice = new PropertyPrice()
                {
                    PricePerMeter = pricePerMeter
                },
                PropertyFeatures = new PropertyFeatures()
                {
                    Balconies = balconies,
                    BasementArea = basementArea
                }
            };
        }

        [Test]
        public void GetBargains_InvokeWhenBadParam_ReturnsBadRequest()
        {
            // Arrange
            _entriesController = new EntriesController(_databaseService.Object);
            int cityId = -1;

            // Act
            ActionResult<List<Entry>> result = _entriesController.GetBargains(cityId);

            // Assert
            Assert.AreEqual(400, (result.Result as StatusCodeResult).StatusCode);
        }

        [Test]
        public void GetBargains_InvokeWhenCityNoBargainsForCity_ReturnsNotFound()
        {
            // Arrange
            _entriesController = new EntriesController(_databaseService.Object);
            int cityId = 2;

            // Act
            ActionResult<List<Entry>> result = _entriesController.GetBargains(cityId);

            // Assert
            Assert.AreEqual(404, (result.Result as StatusCodeResult).StatusCode);
        }

        [Test]
        public void GetBargains_InvokeWhenExistsBargainForCity_ReturnsSuccess()
        {
            // Arrange
            _entriesController = new EntriesController(_databaseService.Object);
            int cityId = 1;

            // Act
            ActionResult<List<Entry>> result = _entriesController.GetBargains(cityId);

            // Assert
            Assert.AreEqual(200, (result.Result as ObjectResult).StatusCode);
        }

        [Test]
        public void GetBargains_InvokeWhenExistsOneBargainForCity_ReturnsOneEntry()
        {
            // Arrange
            _entriesController = new EntriesController(_databaseService.Object);
            int cityId = 1;

            // Act
            ActionResult<List<Entry>> result = _entriesController.GetBargains(cityId);
            var okResult = result.Result as OkObjectResult;
            var resultEntries = okResult.Value as List<Entry>;

            // Assert
            Assert.NotNull(resultEntries);
            Assert.AreEqual(1, resultEntries.Count);
        }

        [Test]
        public void GetBargains_InvokeWhenExistsMoreThanTenBargainsForCity_ReturnsTenEntries()
        {
            // Arrange
            _entriesController = new EntriesController(_databaseService.Object);
            int cityId = 852;

            // Act
            ActionResult<List<Entry>> result = _entriesController.GetBargains(cityId);
            var okResult = result.Result as OkObjectResult;
            var resultEntries = okResult.Value as List<Entry>;

            // Assert
            Assert.NotNull(resultEntries);
            Assert.AreEqual(10, resultEntries.Count);
        }

        [Test]
        public void GetBargains_InvokeWhenExistsMoreThanTenBargainsForCity_ReturnsFirstEntryAsBestBargain()
        {
            // Arrange
            _entriesController = new EntriesController(_databaseService.Object);
            int cityId = 852;
            Entry bestEntry = _bestBargainWroclaw;

            // Act
            ActionResult<List<Entry>> result = _entriesController.GetBargains(cityId);
            var okResult = result.Result as OkObjectResult;
            var resultEntries = okResult.Value as List<Entry>;
            var firstResult = resultEntries[0];

            // Assert
            Assert.NotNull(firstResult);
            Assert.AreEqual(bestEntry.PropertyAddress.City, firstResult.PropertyAddress.City);
            Assert.AreEqual(bestEntry.PropertyPrice.PricePerMeter, firstResult.PropertyPrice.PricePerMeter);
            Assert.AreEqual(bestEntry.PropertyDetails.YearOfConstruction, firstResult.PropertyDetails.YearOfConstruction);
            Assert.AreEqual(bestEntry.PropertyDetails.NumberOfRooms, firstResult.PropertyDetails.NumberOfRooms);
            Assert.AreEqual(bestEntry.PropertyFeatures.Balconies, firstResult.PropertyFeatures.Balconies);
            Assert.AreEqual(bestEntry.PropertyFeatures.BasementArea, firstResult.PropertyFeatures.BasementArea);
        }
    }
}