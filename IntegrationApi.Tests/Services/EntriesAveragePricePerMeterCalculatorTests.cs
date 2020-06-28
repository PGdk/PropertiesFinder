using System.Collections.Generic;
using IntegrationApi.Services;
using Models;
using NUnit.Framework;

namespace IntegrationApi.Tests.Services
{
    [TestFixture]
    class EntriesAveragePricePerMeterCalculatorTests
    {
        private EntriesAveragePricePerMeterCalculator _calculator;

        [SetUp]
        public void SetUp()
        {
            _calculator = new EntriesAveragePricePerMeterCalculator();
        }

        [Test]
        public void AveragePriceCalculation_GivenEmptyEntriesList_ReturnsZero()
        {
            // Arrange
            List<Entry> entries = new List<Entry>();

            // Act
            decimal averagePrice = _calculator.Calculate(entries);

            // Assert
            Assert.AreEqual(0.0m, averagePrice);
        }

        [Test]
        public void AveragePriceCalculation_GivenEntriesList_ReturnsAveragePrice()
        {
            // Arrange
            List<Entry> entries = new List<Entry>();

            entries.Add(CreateEntry(5));
            entries.Add(CreateEntry(3));

            // Act
            decimal averagePrice = _calculator.Calculate(entries);

            // Assert
            Assert.AreEqual(4, averagePrice);
        }

        private Entry CreateEntry(decimal pricePerMeter)
        {
            return new Entry
            {
                PropertyPrice = new PropertyPrice
                {
                    PricePerMeter = pricePerMeter
                }
            };
        }
    }
}
