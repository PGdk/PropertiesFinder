using System;
using System.Collections.Generic;
using System.Text;
using IntegrationApi;
using Microsoft.EntityFrameworkCore.Internal;
using Models;
using Xunit;
using System.Linq;

namespace IntegrationApi.Tests
{
    public class BestEntriesTests
    {
        [Fact]
        public void Should_NotContainMostExpensiveOffer_When_FiveAreAlreadyPicked()
        {
            // Arrange
            var entries = new List<Entry>
            {
                CreateTestingEntry(0, 55, 0, PolishCity.WARSZAWA, OfferKind.RENTAL),
                CreateTestingEntry(1, 110, 0, PolishCity.WARSZAWA, OfferKind.RENTAL),
                CreateTestingEntry(2, 300, 0, PolishCity.WARSZAWA, OfferKind.RENTAL),
                CreateTestingEntry(3, 9000, 0, PolishCity.WARSZAWA, OfferKind.RENTAL),
                CreateTestingEntry(4, 8999, 0, PolishCity.WARSZAWA, OfferKind.RENTAL),
                CreateTestingEntry(5, 15, 0, PolishCity.WARSZAWA, OfferKind.RENTAL)
            };

            var bestEntryPicker = new BestEntryPicker(entries, PolishCity.WARSZAWA, OfferKind.RENTAL, 9999999);

            // Act
            var bestEntries = bestEntryPicker.GetBestFiveEntries();

            // Assert
            Assert.DoesNotContain(bestEntries, e => e.Id == 3);
        }

        [Fact]
        public void Should_GetOfferOnlyWithCorrectCity()
        {
            // Arrange
            var validEntry = CreateTestingEntry(0, 0, 0, PolishCity.WARSZAWA, OfferKind.RENTAL);
            var invalidEntry = CreateTestingEntry(1, 0, 0, PolishCity.GDANSK, OfferKind.RENTAL);

            var entries = new List<Entry> { validEntry, invalidEntry };
            var bestEntryPicker = new BestEntryPicker(entries, PolishCity.WARSZAWA, OfferKind.RENTAL, 9999999);

            // Act
            var bestEntries = bestEntryPicker.GetBestFiveEntries();

            // Assert
            Assert.Contains(bestEntries, e => e == validEntry);
            Assert.DoesNotContain(bestEntries, e => e == invalidEntry);
        }

        [Fact]
        public void Should_NotContainEntryAboveMaximumPrice()
        {
            // Arrange
            var expensiveEntry = CreateTestingEntry(0, 0, 9999999, PolishCity.WARSZAWA, OfferKind.RENTAL);
            var entries = new List<Entry> { expensiveEntry };
            var bestEntryPicker = new BestEntryPicker(entries, PolishCity.WARSZAWA, OfferKind.RENTAL, 0);

            // Act
            var bestEntries = bestEntryPicker.GetBestFiveEntries();

            // Assert
            Assert.DoesNotContain(bestEntries, e => e == expensiveEntry);
        }

        [Fact]
        public void Should_ReturnEmptyList_When_ListOfEntriesIsEmpty()
        {
            // Arrange
            var entries = new List<Entry> { null };
            var bestEntryPicker = new BestEntryPicker(entries, PolishCity.WARSZAWA, OfferKind.RENTAL, 0);

            // Act
            var bestEntries = bestEntryPicker.GetBestFiveEntries();

            // Asserts
            Assert.Empty(bestEntries);
        }

        private Entry CreateTestingEntry(int id, decimal pricePerMeter, decimal totalGrossPrice, PolishCity city, OfferKind offerKind)
        {
            return new Entry
            {
                Id = id,
                PropertyPrice = new PropertyPrice
                {
                    PricePerMeter = pricePerMeter,
                    TotalGrossPrice = totalGrossPrice
                },
                PropertyAddress = new PropertyAddress
                {
                    City = city
                },
                OfferDetails = new OfferDetails
                {
                    OfferKind = offerKind
                }
            };
        }
    }
}