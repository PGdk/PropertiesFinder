using NUnit.Framework;
using IntegrationApi.Controllers;
using DatabaseConnection;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PolskaTimes.Tests
{
    [TestFixture]
    public class CheapestOfferControllerTests
    {
        private PolskaTimesDBContext _context;
        private PolskaTimesController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PolskaTimesDBContext>().UseInMemoryDatabase(databaseName: "TestDB").Options;
            _context = new PolskaTimesDBContext(options);
            _controller = new PolskaTimesController(_context);

            var entries = new List<Entry>(){
                new Entry
                {
                    OfferDetails = new OfferDetails{},
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = 10183.67M,
                    },
                    PropertyDetails = new PropertyDetails{},
                    PropertyAddress = new PropertyAddress
                    {
                        District = "Wola",
                        StreetName = "Jana Pawła",
                    },
                    PropertyFeatures = new PropertyFeatures{},
                },
                new Entry
                {
                    OfferDetails = new OfferDetails{},
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = 20183.67M,
                    },
                    PropertyDetails = new PropertyDetails{},
                    PropertyAddress = new PropertyAddress
                    {
                        District = "Wola",
                        StreetName = "Pana Kazimierza",
                    },
                    PropertyFeatures = new PropertyFeatures{},
                },
                new Entry
                {
                    OfferDetails = new OfferDetails{},
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = 1234.67M,
                    },
                    PropertyDetails = new PropertyDetails{},
                    PropertyAddress = new PropertyAddress
                    {
                        District = "Wola",
                        StreetName = "Jana Kazimierza",
                    },
                    PropertyFeatures = new PropertyFeatures{},
                },
                new Entry
                {
                    OfferDetails = new OfferDetails{},
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = 222.67M,
                    },
                    PropertyDetails = new PropertyDetails{},
                    PropertyAddress = new PropertyAddress
                    {
                        District = "Wola",
                        StreetName = "Stefana",
                    },
                    PropertyFeatures = new PropertyFeatures{},
                },
                new Entry
                {
                    OfferDetails = new OfferDetails{},
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = 111.67M,
                    },
                    PropertyDetails = new PropertyDetails{},
                    PropertyAddress = new PropertyAddress
                    {
                        District = "Wola",
                        StreetName = "Jana Mariusza",
                    },
                    PropertyFeatures = new PropertyFeatures{},
                },
                new Entry
                {
                    OfferDetails = new OfferDetails{},
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = 151.67M,
                    },
                    PropertyDetails = new PropertyDetails{},
                    PropertyAddress = new PropertyAddress
                    {
                        District = "Wola",
                        StreetName = "Szklana",
                    },
                    PropertyFeatures = new PropertyFeatures{},
                }
            };
            _context.entries.AddRange(entries);
            _context.SaveChanges();
        }
        [TearDown]
        public void Teardown()
        {
            _context.Database.EnsureDeleted();
        }
        [Test]
        public void Take_cheapest_offers__If_no_results__Then_there_are_no_offers()
        {
            //Arrange
            _context.entries.RemoveRange();
            _context.SaveChanges();
            //Act
            var bestEntries = _controller.GetBestEntries(null);

            //Assert
            Assert.IsEmpty(bestEntries.Result.Value);
        }
        [Test]
        public void Take_cheapest_offers__If_no_offers_with_a_given_district__Return_null()
        {
            //Arrange
            var district = "Wilanów";
        
            //Act
            var bestOffers = _controller.GetBestEntries(district);

            //Assert  
            Assert.IsNull(bestOffers.Result.Result);
        }
        [Test]
        public void Take_cheapest_offers__If_there_are_more_than_5_offers_with_a_given_district__Return_cheapest_entries_limited_to_5()
        {

            //Arrange
            var district = "Wola";
            var maxOfferNumbers = 5;

            //Act
            var bestOffers = _controller.GetBestEntries(district);

            //Assert
            Assert.LessOrEqual(bestOffers.Result.Value.Count, maxOfferNumbers);
            Assert.That(bestOffers.Result.Value, Has.All.With.Property(nameof(Entry.PropertyAddress)).Property(nameof(PropertyAddress.District)).EqualTo(district));
        }
        [Test]
        public void Take_cheapest_offers__If_district_does_not_exist__Return_null()
        {
            //arrange
            var district = "ThatDistrictNotExist";

            //Act
            var bestOffers = _controller.GetBestEntries(district);

            //Assert
            Assert.IsNull(bestOffers.Result.Result);
        }
    }
}