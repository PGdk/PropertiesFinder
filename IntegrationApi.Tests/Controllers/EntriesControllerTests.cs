using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using DatabaseConnection;
using IntegrationApi.Controllers;
using Microsoft.EntityFrameworkCore;
using Models;
using Moq;
using NUnit.Framework;

namespace IntegrationApi.Tests.Controllers
{
    [TestFixture]
    public class EntriesControllerTests
    {
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void DealSearch_NoOffersInCity_EmptyList()
        {
            var prices = new List<PropertyPrice>()
            {
                _fixture.Build<PropertyPrice>()
                    .With(p => p.PricePerMeter, 10000)
                    .Create(),
                _fixture.Build<PropertyPrice>()
                    .With(p => p.PricePerMeter, 10000)
                    .Create(),
                _fixture.Build<PropertyPrice>()
                    .With(p => p.PricePerMeter, 10000)
                    .Create(),
                _fixture.Build<PropertyPrice>()
                    .With(p => p.PricePerMeter, 5000)
                    .Create()
            };

            var addresses = new List<PropertyAddress>()
            {
                _fixture.Build<PropertyAddress>()
                    .With(a => a.City, PolishCity.CEDYNIA)
                    .Create(),
                _fixture.Build<PropertyAddress>()
                    .With(a => a.City, PolishCity.CEDYNIA)
                    .Create(),
                _fixture.Build<PropertyAddress>()
                    .With(a => a.City, PolishCity.CEDYNIA)
                    .Create(),
                _fixture.Build<PropertyAddress>()
                    .With(a => a.City, PolishCity.CEDYNIA)
                    .Create(),
            };

            var entries = new List<Entry>()
            {
                _fixture.Build<Entry>()
                    .With(e => e.PropertyPrice, prices.ElementAt(0))
                    .With(e => e.PropertyAddress, addresses.ElementAt(0))
                    .Create(),
                _fixture.Build<Entry>()
                    .With(e => e.PropertyPrice, prices.ElementAt(1))
                    .With(e => e.PropertyAddress, addresses.ElementAt(1))
                    .Create(),
                _fixture.Build<Entry>()
                    .With(e => e.PropertyPrice, prices.ElementAt(2))
                    .With(e => e.PropertyAddress, addresses.ElementAt(2))
                    .Create(),
                _fixture.Build<Entry>()
                    .With(e => e.PropertyPrice, prices.ElementAt(3))
                    .With(e => e.PropertyAddress, addresses.ElementAt(3))
                    .Create(),
            };

            var entriesMock = NewDbSetMock(entries);
            var contextMock = new Mock<IDatabaseContext>();
            contextMock.Setup(x => x.Entries).Returns(entriesMock.Object);

            var controller = new EntriesController(contextMock.Object);

            var result = controller.GetDeal(PolishCity.ALWERNIA);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void DealSearch_MatchingOffer_ListOfOffers()
        {
            var prices = new List<PropertyPrice>()
            {
                _fixture.Build<PropertyPrice>()
                    .With(p => p.PricePerMeter, 10000)
                    .Create(),
                _fixture.Build<PropertyPrice>()
                    .With(p => p.PricePerMeter, 10000)
                    .Create(),
                _fixture.Build<PropertyPrice>()
                    .With(p => p.PricePerMeter, 10000)
                    .Create(),
                _fixture.Build<PropertyPrice>()
                    .With(p => p.PricePerMeter, 5000)
                    .Create()
            };

            var addresses = new List<PropertyAddress>()
            {
                _fixture.Build<PropertyAddress>()
                    .With(a => a.City, PolishCity.CEDYNIA)
                    .Create(),
                _fixture.Build<PropertyAddress>()
                    .With(a => a.City, PolishCity.CEDYNIA)
                    .Create(),
                _fixture.Build<PropertyAddress>()
                    .With(a => a.City, PolishCity.CEDYNIA)
                    .Create(),
                _fixture.Build<PropertyAddress>()
                    .With(a => a.City, PolishCity.CEDYNIA)
                    .Create(),
            };

            var entries = new List<Entry>()
            {
                _fixture.Build<Entry>()
                    .With(e => e.PropertyPrice, prices.ElementAt(0))
                    .With(e => e.PropertyAddress, addresses.ElementAt(0))
                    .Create(),
                _fixture.Build<Entry>()
                    .With(e => e.PropertyPrice, prices.ElementAt(1))
                    .With(e => e.PropertyAddress, addresses.ElementAt(1))
                    .Create(),
                _fixture.Build<Entry>()
                    .With(e => e.PropertyPrice, prices.ElementAt(2))
                    .With(e => e.PropertyAddress, addresses.ElementAt(2))
                    .Create(),
                _fixture.Build<Entry>()
                    .With(e => e.PropertyPrice, prices.ElementAt(3))
                    .With(e => e.PropertyAddress, addresses.ElementAt(3))
                    .Create(),
            };

            var entriesMock = NewDbSetMock(entries);
            var contextMock = new Mock<IDatabaseContext>();
            contextMock.Setup(x => x.Entries).Returns(entriesMock.Object);

            var controller = new EntriesController(contextMock.Object);

            var result = controller.GetDeal(PolishCity.CEDYNIA);

            Assert.That(result, Is.EqualTo(new List<Entry>()
            {
                entries.ElementAt(3)
            }));
        }

        [Test]
        public void DealSearch_NoDealsInCity_EmptyList()
        {
            var prices = new List<PropertyPrice>()
            {
                _fixture.Build<PropertyPrice>()
                    .With(p => p.PricePerMeter, 10000)
                    .Create(),
                _fixture.Build<PropertyPrice>()
                    .With(p => p.PricePerMeter, 10000)
                    .Create(),
                _fixture.Build<PropertyPrice>()
                    .With(p => p.PricePerMeter, 10000)
                    .Create(),
                _fixture.Build<PropertyPrice>()
                    .With(p => p.PricePerMeter, 9000)
                    .Create()
            };

            var addresses = new List<PropertyAddress>()
            {
                _fixture.Build<PropertyAddress>()
                    .With(a => a.City, PolishCity.CEDYNIA)
                    .Create(),
                _fixture.Build<PropertyAddress>()
                    .With(a => a.City, PolishCity.CEDYNIA)
                    .Create(),
                _fixture.Build<PropertyAddress>()
                    .With(a => a.City, PolishCity.CEDYNIA)
                    .Create(),
                _fixture.Build<PropertyAddress>()
                    .With(a => a.City, PolishCity.CEDYNIA)
                    .Create(),
            };

            var entries = new List<Entry>()
            {
                _fixture.Build<Entry>()
                    .With(e => e.PropertyPrice, prices.ElementAt(0))
                    .With(e => e.PropertyAddress, addresses.ElementAt(0))
                    .Create(),
                _fixture.Build<Entry>()
                    .With(e => e.PropertyPrice, prices.ElementAt(1))
                    .With(e => e.PropertyAddress, addresses.ElementAt(1))
                    .Create(),
                _fixture.Build<Entry>()
                    .With(e => e.PropertyPrice, prices.ElementAt(2))
                    .With(e => e.PropertyAddress, addresses.ElementAt(2))
                    .Create(),
                _fixture.Build<Entry>()
                    .With(e => e.PropertyPrice, prices.ElementAt(3))
                    .With(e => e.PropertyAddress, addresses.ElementAt(3))
                    .Create(),
            };

            var entriesMock = NewDbSetMock(entries);
            var contextMock = new Mock<IDatabaseContext>();
            contextMock.Setup(x => x.Entries).Returns(entriesMock.Object);

            var controller = new EntriesController(contextMock.Object);

            var result = controller.GetDeal(PolishCity.CEDYNIA);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void DealSearch_DealsInMixedCities_ListOfOffers()
        {
            var prices = new List<PropertyPrice>()
            {
                _fixture.Build<PropertyPrice>()
                    .With(p => p.PricePerMeter, 10000)
                    .Create(),
                _fixture.Build<PropertyPrice>()
                    .With(p => p.PricePerMeter, 10000)
                    .Create(),
                _fixture.Build<PropertyPrice>()
                    .With(p => p.PricePerMeter, 3000)
                    .Create(),
                _fixture.Build<PropertyPrice>()
                    .With(p => p.PricePerMeter, 10000)
                    .Create()
            };

            var addresses = new List<PropertyAddress>()
            {
                _fixture.Build<PropertyAddress>()
                    .With(a => a.City, PolishCity.CEDYNIA)
                    .Create(),
                _fixture.Build<PropertyAddress>()
                    .With(a => a.City, PolishCity.CEDYNIA)
                    .Create(),
                _fixture.Build<PropertyAddress>()
                    .With(a => a.City, PolishCity.ALWERNIA)
                    .Create(),
                _fixture.Build<PropertyAddress>()
                    .With(a => a.City, PolishCity.ALWERNIA)
                    .Create(),
            };

            var entries = new List<Entry>()
            {
                _fixture.Build<Entry>()
                    .With(e => e.PropertyPrice, prices.ElementAt(0))
                    .With(e => e.PropertyAddress, addresses.ElementAt(0))
                    .Create(),
                _fixture.Build<Entry>()
                    .With(e => e.PropertyPrice, prices.ElementAt(1))
                    .With(e => e.PropertyAddress, addresses.ElementAt(1))
                    .Create(),
                _fixture.Build<Entry>()
                    .With(e => e.PropertyPrice, prices.ElementAt(2))
                    .With(e => e.PropertyAddress, addresses.ElementAt(2))
                    .Create(),
                _fixture.Build<Entry>()
                    .With(e => e.PropertyPrice, prices.ElementAt(3))
                    .With(e => e.PropertyAddress, addresses.ElementAt(3))
                    .Create(),
            };

            var entriesMock = NewDbSetMock(entries);
            var contextMock = new Mock<IDatabaseContext>();
            contextMock.Setup(x => x.Entries).Returns(entriesMock.Object);

            var controller = new EntriesController(contextMock.Object);

            var result = controller.GetDeal(PolishCity.ALWERNIA);

            Assert.That(result, Is.EqualTo(new List<Entry>()
            {
                entries.ElementAt(2)
            }));
        }

        private static Mock<DbSet<T>> NewDbSetMock<T>(IEnumerable<T> items) where T : class
        {
            var queryableItems = items.AsQueryable();

            var mock = new Mock<DbSet<T>>();
            mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryableItems.Provider);
            mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableItems.Expression);
            mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableItems.ElementType);
            mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryableItems.GetEnumerator());

            return mock;
        }
    }


}