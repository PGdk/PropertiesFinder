using IntegrationApi.Services;
using Models;
using NUnit.Framework;

namespace IntegrationApi.Tests.Services
{
    [TestFixture]
    class EntryPointsCalculatorTest
    {
        private EntryPointsCalculator _calculator;

        [SetUp]
        public void SetUp()
        {
            _calculator = new EntryPointsCalculator();
        }

        [Test]
        public void PointsCalculation_GivenEmptyEntry_ReturnsZero()
        {
            // Arrange
            Entry entry = new Entry();

            // Act
            decimal points = _calculator.Calculate(entry, 0);

            // Assert
            Assert.AreEqual(0.0m, points);
        }

        [Test]
        public void PointsCalculation_GivenEntryWithPricePerMeter_ReturnsPoints()
        {
            // Arrange
            Entry entry = new Entry
            {
                PropertyPrice = new PropertyPrice
                {
                    PricePerMeter = 1000
                }
            };

            // Act
            decimal points = _calculator.Calculate(entry, 2000);

            // Assert
            Assert.AreEqual(2, points);
        }

        [Test]
        public void PointsCalculation_GivenEntryWithResidentalRent_ReturnsPoints()
        {
            // Arrange
            Entry entry = new Entry
            {
                PropertyPrice = new PropertyPrice
                {
                    ResidentalRent = 500
                }
            };

            // Act
            decimal points = _calculator.Calculate(entry, 0);

            // Assert
            Assert.AreEqual(2, points);
        }

        [Test]
        public void PointsCalculation_GivenEntryWithArea_ReturnsPoints()
        {
            // Arrange
            Entry entry = new Entry
            {
                PropertyDetails = new PropertyDetails
                {
                    Area = 100
                }
            };

            // Act
            decimal points = _calculator.Calculate(entry, 0);

            // Assert
            Assert.AreEqual(5, points);
        }

        [Test]
        public void PointsCalculation_GivenEntryWithGardenArea_ReturnsPoints()
        {
            // Arrange
            Entry entry = new Entry
            {
                PropertyFeatures = new PropertyFeatures
                {
                    GardenArea = 200
                }
            };

            // Act
            decimal points = _calculator.Calculate(entry, 0);

            // Assert
            Assert.AreEqual(10, points);
        }

        [Test]
        public void PointsCalculation_GivenEntryWithGardenAreaLargerThanMax_ReturnsPoints()
        {
            // Arrange
            Entry entry = new Entry
            {
                PropertyFeatures = new PropertyFeatures
                {
                    GardenArea = 2000
                }
            };

            // Act
            decimal points = _calculator.Calculate(entry, 0);

            // Assert
            Assert.AreEqual(20, points);
        }

        [Test]
        public void PointsCalculation_GivenEntryWithBasement_ReturnsPoints()
        {
            // Arrange
            Entry entry = new Entry
            {
                PropertyFeatures = new PropertyFeatures
                {
                    BasementArea = 15
                }
            };

            // Act
            decimal points = _calculator.Calculate(entry, 0);

            // Assert
            Assert.AreEqual(3, points);
        }

        [Test]
        public void PointsCalculation_GivenEntryWithBalconies_ReturnsPoints()
        {
            // Arrange
            Entry entry = new Entry
            {
                PropertyFeatures = new PropertyFeatures
                {
                    Balconies = 2
                }
            };

            // Act
            decimal points = _calculator.Calculate(entry, 0);

            // Assert
            Assert.AreEqual(6, points);
        }

        [Test]
        public void PointsCalculation_GivenEntryWithOutdoorParkingPlaces_ReturnsPoints()
        {
            // Arrange
            Entry entry = new Entry
            {
                PropertyFeatures = new PropertyFeatures
                {
                    OutdoorParkingPlaces = 3
                }
            };

            // Act
            decimal points = _calculator.Calculate(entry, 0);

            // Assert
            Assert.AreEqual(6, points);
        }

        [Test]
        public void PointsCalculation_GivenEntryWithIndoorParkingPlaces_ReturnsPoints()
        {
            // Arrange
            Entry entry = new Entry
            {
                PropertyFeatures = new PropertyFeatures
                {
                    IndoorParkingPlaces = 2
                }
            };

            // Act
            decimal points = _calculator.Calculate(entry, 0);

            // Assert
            Assert.AreEqual(8, points);
        }

        [Test]
        public void PointsCalculation_GivenEntryWithData_ReturnsPoints()
        {
            // Arrange
            Entry entry = new Entry
            {
                PropertyPrice = new PropertyPrice
                {
                    PricePerMeter = 1000,
                    ResidentalRent = 500
                },
                PropertyDetails = new PropertyDetails
                {
                    Area = 100
                },
                PropertyFeatures = new PropertyFeatures
                {
                    GardenArea = 200,
                    BasementArea = 15,
                    Balconies = 2,
                    OutdoorParkingPlaces = 3,
                    IndoorParkingPlaces = 2
                }
            };

            // Act
            decimal points = _calculator.Calculate(entry, 2000);

            // Assert
            Assert.AreEqual(42, points);
        }
    }
}
