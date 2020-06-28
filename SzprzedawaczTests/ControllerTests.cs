using DatabaseConnection;
using IntegrationApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SzprzedawaczTests
{
    [TestFixture]
    public class ControllerTests
    {
        PFinderContext emptyContext;
        PFinderContext context;
        [SetUp]
        public void Setup()
        {
            var emptyData = new List<Entry>().AsQueryable();
            var mockEmptySet = new Mock<DbSet<Entry>>();
            mockEmptySet.As<IQueryable<Entry>>().Setup(m => m.Provider).Returns(emptyData.Provider);
            mockEmptySet.As<IQueryable<Entry>>().Setup(m => m.Expression).Returns(emptyData.Expression);
            mockEmptySet.As<IQueryable<Entry>>().Setup(m => m.ElementType).Returns(emptyData.ElementType);
            mockEmptySet.As<IQueryable<Entry>>().Setup(m => m.GetEnumerator()).Returns(emptyData.GetEnumerator());
            var memptyContext = new Mock<PFinderContext>();
            memptyContext.Setup(c => c.Entries).Returns(mockEmptySet.Object);
            emptyContext = memptyContext.Object;

            var list = new List<Entry>();

            for (int i = 1; i < 10; i++)
            {
                list.Add(new Entry
                {
                    OfferDetails = new OfferDetails
                    {
                        OfferKind = OfferKind.RENTAL
                    },
                    PropertyAddress = new PropertyAddress
                    {
                        City = PolishCity.GDANSK
                    },
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = i
                    }
                });
                list.Add(new Entry
                {
                    OfferDetails = new OfferDetails
                    {
                        OfferKind = OfferKind.SALE
                    },
                    PropertyAddress = new PropertyAddress
                    {
                        City = PolishCity.GDANSK
                    },
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = i
                    }
                });
            }
            var data = list.AsQueryable();

            var mockSet = new Mock<DbSet<Entry>>();
            mockSet.As<IQueryable<Entry>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Entry>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Entry>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Entry>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            var mContext = new Mock<PFinderContext>();
            mContext.Setup(c => c.Entries).Returns(mockSet.Object);
            context = mContext.Object;
        }

        [Test]
        public async Task TestNoEntries()
        {
            var controller = new PropertyController(emptyContext);
            var best = controller.GetBestEntries("gdansk").Result.Result as NotFoundResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, best.StatusCode);
        }
        [Test]
        public async Task TestWrongCity()
        {
            var controller = new PropertyController(emptyContext);

            var wrongCity = controller.GetBestEntries("fdgfg").Result.Result as BadRequestResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, wrongCity.StatusCode);
        }

        [Test]
        public async Task TestHappy()
        {
            var controller = new PropertyController(context);

            var entries = controller.GetBestEntries("gdansk").Result.Value;

            Assert.AreEqual(10, entries.Count());
        }
        [Test]
        public async Task TestOrder()
        {
            var controller = new PropertyController(context);

            var entries = controller.GetBestEntries("gdansk").Result.Value;

            Assert.LessOrEqual(entries.First().PropertyPrice.PricePerMeter, entries.Skip(1).First().PropertyPrice.PricePerMeter);
        }
    }
}