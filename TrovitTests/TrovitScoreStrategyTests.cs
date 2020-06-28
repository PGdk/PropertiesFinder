using Application.Trovit;
using DatabaseConnection;
using Moq;
using NUnit.Framework;
using System;

namespace TrovitTests
{
    public class TrovitScoreStrategyTests
    {

        private TrovitScoreStrategy strategy;

        [SetUp]
        public void Setup()
        {
            strategy = new TrovitScoreStrategy();
        }

        [Test]
        public void TestOfferWithoutStreetName()
        {
            var result = strategy.Score(new Models.Entry { });
            Assert.AreEqual(0, result);
            Assert.Pass();
        }

        [Test]
        public void TestOfferOutdated()
        {
            var result = strategy.Score(new Models.Entry
            {
                PropertyAddress = new Models.PropertyAddress
                {
                    StreetName = "test",
                },
                OfferDetails = new Models.OfferDetails
                {
                    CreationDateTime = DateTime.Now.AddDays(-15)
                }
            });
            Assert.AreEqual(0, result);
            Assert.Pass();
        }

        [Test]
        public void TestOfferFresh()
        {
            var result = strategy.Score(new Models.Entry
            {
                PropertyAddress = new Models.PropertyAddress
                {
                    StreetName = "test",
                },
                OfferDetails = new Models.OfferDetails { 
                    CreationDateTime = DateTime.Now,
                }
            });
            Assert.AreEqual(50, result);
            Assert.Pass();
        }

        [Test]
        public void TestOfferHasBalconies()
        {
            int result = strategy.Score(new Models.Entry
            {
                PropertyAddress = new Models.PropertyAddress
                {
                    StreetName = "test",
                },
                PropertyFeatures = new Models.PropertyFeatures
                {
                    Balconies = 1
                }
            });
            Assert.AreEqual(10, result);
        }


        [Test]
        public void TestOfferHasNoBalconies()
        { 
            var result = strategy.Score(new Models.Entry
            {
                PropertyAddress = new Models.PropertyAddress
                {
                    StreetName = "test",
                },
                PropertyFeatures = new Models.PropertyFeatures
                {
                    Balconies = 0
                }
            });
            Assert.AreEqual(0, result);

            Assert.Pass();
        }

        [Test]
        public void TestOfferFloorOver()
        {
            var result = strategy.Score(new Models.Entry
            {
                PropertyAddress = new Models.PropertyAddress {
                    StreetName = "test",
                },
                PropertyDetails = new Models.PropertyDetails
                {
                    FloorNumber = 4
                }
            });
            Assert.AreEqual(0, result);
        }

        [Test]
        public void TestOfferFloorBelow()
        { 
            var result = strategy.Score(new Models.Entry
            {
                PropertyAddress = new Models.PropertyAddress
                {
                    StreetName = "test",
                },
                PropertyDetails = new Models.PropertyDetails
                {
                    FloorNumber = 3
                }
            });
            Assert.AreEqual(20, result);

            Assert.Pass();
        }

        [Test]
        public void TestOfferPriceRangeFrom()
        {
            var result = strategy.Score(new Models.Entry
            {
                PropertyAddress = new Models.PropertyAddress
                {
                    StreetName = "test",
                },
                PropertyPrice = new Models.PropertyPrice
                {
                    PricePerMeter = 0
                }
            });

            Assert.AreEqual(0, result);
        }

        [Test]
        public void TestOfferPriceRangeTo()
        {
            var result = strategy.Score(new Models.Entry
            {
                PropertyAddress = new Models.PropertyAddress
                {
                    StreetName = "test",
                },
                PropertyPrice = new Models.PropertyPrice
                {
                    PricePerMeter = 8000
                }
            });
            Assert.AreEqual(0, result);
        }

        [Test]
        public void TestOfferPriceInRange()
        { 
            var result = strategy.Score(new Models.Entry
            {
                PropertyAddress = new Models.PropertyAddress
                {
                    StreetName = "test",
                },
                PropertyPrice = new Models.PropertyPrice
                {
                    PricePerMeter = 4000
                }
            });
            Assert.AreEqual(20, result);

            Assert.Pass();
        }
    }
}