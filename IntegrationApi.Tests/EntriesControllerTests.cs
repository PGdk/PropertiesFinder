using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseConnection;
using IntegrationApi.Controllers;
using Microsoft.EntityFrameworkCore;
using Models;
using Moq;
using NUnit.Framework;

namespace IntegrationApi.Tests
{
    [TestFixture]
    class EntriesControllerTests
    {
        private List<Entry> list;

        [SetUp]
        public void SetUp()
        {
            list = new List<Entry>();
            for (int i = 30; i > 0; i--)
            {
                list.Add(new Entry
                {
                    OfferDetails = new OfferDetails(),
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = 120 + i,
                    },
                    PropertyDetails = new PropertyDetails(),
                    PropertyAddress = new PropertyAddress
                    {
                        City = PolishCity.GDANSK,
                    },
                    PropertyFeatures = new PropertyFeatures(),
                    RawDescription = ""
                });
            }
        }

        [Test]
        public void GetBarginShould10BestOffersInCityWithMoreOffers()
        {
            List<Entry> list2 = new List<Entry>();
            for(int i = 29; i >= 20; i--)
            {
                list2.Add(list[i]);
            }
            var entriesMock = NewDbSetMock(list);
            var contextMock = new Mock<IDatabaseContext>();
            contextMock.Setup(x => x.Entries).Returns(entriesMock.Object);
            var controller = new EntriesController(contextMock.Object);

            var result = controller.GetBargain(PolishCity.GDANSK);

            Assert.That(result, Is.EqualTo(list2));
        }

        [Test]
        public void GetBarginShould10BestOffersInCityWithMOreOffers_MixedCitiesEdition()
        {
            List<Entry> list2 = new List<Entry>();
            for (int i = 29; i >= 20; i--)
            {
                list2.Add(list[i]);
            }
            for (int i = 0; i < 10; i++)
            {
                list.Add(new Entry
                {
                    OfferDetails = new OfferDetails(),
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = 100 + i,
                    },
                    PropertyDetails = new PropertyDetails(),
                    PropertyAddress = new PropertyAddress
                    {
                        City = PolishCity.GDYNIA,
                    },
                    PropertyFeatures = new PropertyFeatures(),
                    RawDescription = ""
                });
            }
            var entriesMock = NewDbSetMock(list);
            var contextMock = new Mock<IDatabaseContext>();
            contextMock.Setup(x => x.Entries).Returns(entriesMock.Object);
            var controller = new EntriesController(contextMock.Object);

            var result = controller.GetBargain(PolishCity.GDANSK);

            Assert.That(result, Is.EqualTo(list2));
        }

        [Test]
        public void GetBarginShouldFindNoOffersInCityWithNoOffers()
        {
            var entriesMock = NewDbSetMock(list);
            var contextMock = new Mock<IDatabaseContext>();
            contextMock.Setup(x => x.Entries).Returns(entriesMock.Object);
            var controller = new EntriesController(contextMock.Object);

            var result = controller.GetBargain(PolishCity.KARTUZY);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetBarginShouldFindNoOffersWhenThereAreNoOffersAtAll()
        {
            var entriesMock = NewDbSetMock(new List<Entry>());
            var contextMock = new Mock<IDatabaseContext>();
            contextMock.Setup(x => x.Entries).Returns(entriesMock.Object);
            var controller = new EntriesController(contextMock.Object);

            var result = controller.GetBargain(PolishCity.KARTUZY);

            Assert.That(result, Is.Empty);
        }

        private static Mock<DbSet<T>> NewDbSetMock<T>(IEnumerable<T> list) where T : class
        {
            var query = list.AsQueryable();

            var mock = new Mock<DbSet<T>>();
            mock.As<IQueryable<T>>().Setup(x => x.Provider).Returns(query.Provider);
            mock.As<IQueryable<T>>().Setup(x => x.Expression).Returns(query.Expression);
            mock.As<IQueryable<T>>().Setup(x => x.ElementType).Returns(query.ElementType);
            mock.As<IQueryable<T>>().Setup(x => x.GetEnumerator()).Returns(query.GetEnumerator());

            return mock;
        }
    }
}
