using IntegrationApi;
using Models;
using System.Collections.Generic;
using Xunit;

namespace MorizonTests {
    public class OccasionsFinderTests {

        [Fact]
        public void FindOccasions_EntriesForEachCity_OccasionsFound() {
            //Arrange
            List<Entry> entries = new List<Entry> {
                new Entry {
                    PropertyAddress = new PropertyAddress{City=PolishCity.WARSZAWA},
                    PropertyPrice = new PropertyPrice{PricePerMeter = 9000.00m }
                },
                new Entry {
                    PropertyAddress = new PropertyAddress{City=PolishCity.KRAKOW},
                    PropertyPrice = new PropertyPrice{PricePerMeter = 9500.00m }
                },
                new Entry {
                    PropertyAddress = new PropertyAddress{City=PolishCity.GDANSK},
                    PropertyPrice = new PropertyPrice{PricePerMeter = 10000.00m }
                },
            };

            List<string> cities = new List<string> { "Warszawa", "Krakow", "Gdansk" };
            OccasionsFinder Finder = new OccasionsFinder(cities);

            //Act
            List<Entry> occasions = Finder.FindOccasions(entries);

            //Assert
            Assert.Equal(3, occasions.Count);
            Assert.Contains(entries[0], occasions);
            Assert.Contains(entries[1], occasions);
            Assert.Contains(entries[2], occasions);

        }

        [Fact]
        public void FindOccasions_ManyEntriesForCity_OccasionsFound() {
            //Arrange
            List<Entry> entries = new List<Entry> {
                new Entry {
                    PropertyAddress = new PropertyAddress{City=PolishCity.WARSZAWA},
                    PropertyPrice = new PropertyPrice{PricePerMeter = 9900.00m }
                },
                new Entry {
                    PropertyAddress = new PropertyAddress{City=PolishCity.WARSZAWA},
                    PropertyPrice = new PropertyPrice{PricePerMeter = 9500.00m }
                },
                new Entry {
                    PropertyAddress = new PropertyAddress{City=PolishCity.WARSZAWA},
                    PropertyPrice = new PropertyPrice{PricePerMeter = 10000.00m }
                },
            };

            List<string> cities = new List<string> { "Warszawa" };
            OccasionsFinder Finder = new OccasionsFinder(cities);

            //Act
            List<Entry> occasions = Finder.FindOccasions(entries);

            //Assert
            Assert.Single(occasions);
            Assert.Contains(entries[1], occasions);
        }


        [Fact]
        public void FindOccasions_ManyEntries_OccasionsForCitiesInListReturned() {
            //Arrange
            List<Entry> entries = new List<Entry> {
                new Entry {
                    PropertyAddress = new PropertyAddress{City=PolishCity.WARSZAWA},
                    PropertyPrice = new PropertyPrice{PricePerMeter = 9900.00m }
                },
                new Entry {
                    PropertyAddress = new PropertyAddress{City=PolishCity.KRAKOW},
                    PropertyPrice = new PropertyPrice{PricePerMeter = 9500.00m }
                },
                new Entry {
                    PropertyAddress = new PropertyAddress{City=PolishCity.GDANSK},
                    PropertyPrice = new PropertyPrice{PricePerMeter = 10000.00m }
                },
            };

            List<string> cities = new List<string> { "Gdansk", "Warszawa" };
            OccasionsFinder Finder = new OccasionsFinder(cities);

            //Act
            List<Entry> occasions = Finder.FindOccasions(entries);

            //Assert
            Assert.Equal(2, occasions.Count);
            Assert.Contains(entries[0], occasions);
            Assert.Contains(entries[2], occasions);
            Assert.DoesNotContain(entries[1], occasions);
        }


        [Fact]
        public void FindOccasions_CityDoesNotExist_OccasionNotFound() {
            //Arrange
            List<Entry> entries = new List<Entry> {
                new Entry {
                    PropertyAddress = new PropertyAddress{City=PolishCity.WARSZAWA},
                    PropertyPrice = new PropertyPrice{PricePerMeter = 9000.00m }
                },
            };

            List<string> cities = new List<string> { "Warszawa", "Imaginary City" };
            OccasionsFinder Finder = new OccasionsFinder(cities);

            //Act
            List<Entry> occasions = Finder.FindOccasions(entries);

            //Assert
            Assert.Single(occasions);

        }

        [Fact]
        public void FindOccasions_NoOccasionsForCity_OccasionNotFound() {
            //Arrange
            List<Entry> entries = new List<Entry> {
                new Entry {
                    PropertyAddress = new PropertyAddress{City=PolishCity.WARSZAWA},
                    PropertyPrice = new PropertyPrice{PricePerMeter = 9000.00m }
                },
            };

            List<string> cities = new List<string> { "Warszawa" };
            OccasionsFinder Finder = new OccasionsFinder(cities);

            //Act
            List<Entry> occasions = Finder.FindOccasions(entries);

            //Assert
            Assert.Single(occasions);
        }

        [Fact]
        public void FindOccasions_NoEntriesInDatabase_OccasionNotFound() {
            //Arrange
            List<Entry> entries = new List<Entry> { };

            List<string> cities = new List<string> { "Warszawa" };
            OccasionsFinder Finder = new OccasionsFinder(cities);

            //Act
            List<Entry> occasions = Finder.FindOccasions(entries);

            //Assert
            Assert.Empty(occasions);
        }
    }
}
