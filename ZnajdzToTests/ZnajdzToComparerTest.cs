using Models;
using NUnit.Framework;
using ZnajdzTo;

namespace ZnajdzToTests
{
    [TestFixture]
    public class ZnajdzToComparerTest
    {
        private ZnajdzToComparer _znajdzToComparer;

        private Entry CreateEntry(PolishCity city, string streetName, string district, int area, int floorNumber, int numberOfRooms, int yearOfConstruction, int totalGrossPrice)
        {
            return new Entry
            {
                PropertyAddress = new PropertyAddress
                {
                    City = city,
                    StreetName = streetName,
                    District = district
                },
                PropertyDetails = new PropertyDetails
                {
                    Area = area,
                    FloorNumber = floorNumber,
                    NumberOfRooms = numberOfRooms,
                    YearOfConstruction = yearOfConstruction
                },
                PropertyPrice = new PropertyPrice
                {
                    TotalGrossPrice = totalGrossPrice
                }
            };
        }

        [Test]
        public void Equals_InvokeWhenSameProperties_ReturnsTrue()
        {
            // Arrange
            _znajdzToComparer = new ZnajdzToComparer();
            Entry firstEntry = CreateEntry(PolishCity.GDANSK, "test", "test", 10, 4, 2, 2020, 10);
            Entry secondEntry = CreateEntry(PolishCity.GDANSK, "test", "test", 10, 4, 2, 2020, 10);

            // Act
            bool result = _znajdzToComparer.Equals(firstEntry, secondEntry);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Equals_InvokeWhenTwoDifferentCitiesInEntries_ReturnsFalse()
        {
            // Arrange
            _znajdzToComparer = new ZnajdzToComparer();
            Entry firstEntry = CreateEntry(PolishCity.WROCLAW, "test", "test", 10, 4, 2, 2020, 10);
            Entry secondEntry = CreateEntry(PolishCity.GDANSK, "test", "test", 10, 4, 2, 2020, 10);

            // Act
            bool result = _znajdzToComparer.Equals(firstEntry, secondEntry);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_InvokeWhenTwoDifferentStreetsInEntries_ReturnsFalse()
        {
            // Arrange
            _znajdzToComparer = new ZnajdzToComparer();
            Entry firstEntry = CreateEntry(PolishCity.GDANSK, "test", "test", 10, 4, 2, 2020, 10);
            Entry secondEntry = CreateEntry(PolishCity.GDANSK, "test2", "test", 10, 4, 2, 2020, 10);

            // Act
            bool result = _znajdzToComparer.Equals(firstEntry, secondEntry);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_InvokeWhenTwoDifferentDistrictsInEntries_ReturnsFalse()
        {
            // Arrange
            _znajdzToComparer = new ZnajdzToComparer();
            Entry firstEntry = CreateEntry(PolishCity.GDANSK, "test", "test", 10, 4, 2, 2020, 10);
            Entry secondEntry = CreateEntry(PolishCity.GDANSK, "test", "test2", 10, 4, 2, 2020, 10);

            // Act
            bool result = _znajdzToComparer.Equals(firstEntry, secondEntry);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_InvokeWhenTwoDifferentAreasInEntries_ReturnsFalse()
        {
            // Arrange
            _znajdzToComparer = new ZnajdzToComparer();
            Entry firstEntry = CreateEntry(PolishCity.GDANSK, "test", "test", 10, 4, 2, 2020, 10);
            Entry secondEntry = CreateEntry(PolishCity.GDANSK, "test", "test", 11, 4, 2, 2020, 10);

            // Act
            bool result = _znajdzToComparer.Equals(firstEntry, secondEntry);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_InvokeWhenTwoDifferentFloorsInEntries_ReturnsFalse()
        {
            // Arrange
            _znajdzToComparer = new ZnajdzToComparer();
            Entry firstEntry = CreateEntry(PolishCity.GDANSK, "test", "test", 10, 4, 2, 2020, 10);
            Entry secondEntry = CreateEntry(PolishCity.GDANSK, "test", "test", 10, 5, 2, 2020, 10);

            // Act
            bool result = _znajdzToComparer.Equals(firstEntry, secondEntry);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_InvokeWhenTwoDifferentRoomsInEntries_ReturnsFalse()
        {
            // Arrange
            _znajdzToComparer = new ZnajdzToComparer();
            Entry firstEntry = CreateEntry(PolishCity.GDANSK, "test", "test", 10, 4, 2, 2020, 10);
            Entry secondEntry = CreateEntry(PolishCity.GDANSK, "test", "test", 10, 4, 3, 2020, 10);

            // Act
            bool result = _znajdzToComparer.Equals(firstEntry, secondEntry);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_InvokeWhenTwoDifferentYearsInEntries_ReturnsFalse()
        {
            // Arrange
            _znajdzToComparer = new ZnajdzToComparer();
            Entry firstEntry = CreateEntry(PolishCity.GDANSK, "test", "test", 10, 4, 2, 2020, 10);
            Entry secondEntry = CreateEntry(PolishCity.GDANSK, "test", "test", 10, 4, 2, 2021, 10);

            // Act
            bool result = _znajdzToComparer.Equals(firstEntry, secondEntry);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_InvokeWhenTwoDifferentGrossPricesInEntries_ReturnsFalse()
        {
            // Arrange
            _znajdzToComparer = new ZnajdzToComparer();
            Entry firstEntry = CreateEntry(PolishCity.GDANSK, "test", "test", 10, 4, 2, 2020, 10);
            Entry secondEntry = CreateEntry(PolishCity.GDANSK, "test", "test", 10, 4, 2, 2020, 11);

            // Act
            bool result = _znajdzToComparer.Equals(firstEntry, secondEntry);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
