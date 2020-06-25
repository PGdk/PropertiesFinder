using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Dsl;
using DatabaseConnection;
using IntegrationApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using NUnit.Framework;

namespace UnitTests
{
    public class BestOffersTests
    {
        private ApplicationDbContext _context;
        private EntriesController _controller;
        private readonly IFixture _fixture = new Fixture();
        private readonly Random _random = new Random();

        [SetUp]
        public void Setup()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase(nameof(ApplicationDbContext));
            _context = new ApplicationDbContext(builder.Options);
            _context.Database.IsInMemory();
            _controller = new EntriesController(_context);
        }

        [Test]
        public async Task BestOffersShouldReturnEmptyArrayIfThereAreNoEntries()
        {
            //Arrange

            //Act
            var result = await _controller.GetBestEntry(_fixture.Create<PolishCity>());

            //Assert
            CollectionAssert.IsEmpty(result.Value);
        }

        [Test]
        public async Task BestOffersShouldReturnEmptyArrayIfThereAreNoEntriesInGivenCity()
        {
            var city = _fixture.Create<PolishCity>();

            //Arrange
            var entries = _fixture
                .Build<Entry>()
                .Without(entry => entry.Id)
                .CreateMany(100)
                .Where(entry => entry.PropertyAddress.City != city);

            _context.AddRange(entries);
            await _context.SaveChangesAsync();

            //Act
            var result = await _controller.GetBestEntry(city);

            //Assert
            CollectionAssert.IsEmpty(result.Value);
        }

        [Test]
        public async Task BestOffersShouldShouldReturnOffersFromGivenCity()
        {
            //Arrange
            var city = _fixture.Create<PolishCity>();
            
            var entriesFromOtherCities = _fixture
                .Build<Entry>()
                .Without(entry => entry.Id)
                .CreateMany()
                .Where(entry => entry.PropertyAddress.City != city);

            var entriesFromGivenCity = GetEntryBuilder(city)
                .CreateMany();

            _context.AddRange(entriesFromOtherCities);
            _context.AddRange(entriesFromGivenCity);
            await _context.SaveChangesAsync();

            //Act
            var result = await _controller.GetBestEntry(city);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Has.All.With.Property(nameof(Entry.PropertyAddress)).Property(nameof(PropertyAddress.City)).EqualTo(city));
            Assert.That(result.Value, Is.EquivalentTo(entriesFromGivenCity));
            Assert.That(result.Value, Is.Not.EquivalentTo(entriesFromOtherCities));
        }

        [Test]
        public async Task BestOffersShouldShouldReturnOffersOnlyWithBalcony()
        {
            //Arrange
            PolishCity city = _fixture.Create<PolishCity>();
            
            var entriesWithBalcony = GetEntryBuilder(city)
                .With(entry => entry.PropertyFeatures, () => _fixture
                    .Build<PropertyFeatures>()
                    .With(features => features.HasBalcony, true)
                    .Create())
                .CreateMany(_random.Next(5, 100));

            var entriesWithoutBalcony = GetEntryBuilder(city)
                .With(entry => entry.PropertyFeatures, () => _fixture
                    .Build<PropertyFeatures>()
                    .With(features => features.HasBalcony, false)
                    .Create())
                .CreateMany();

            _context.AddRange(entriesWithBalcony);
            _context.AddRange(entriesWithoutBalcony);
            await _context.SaveChangesAsync();

            //Act
            var result = await _controller.GetBestEntry(city);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Has.All.With.Property(nameof(Entry.PropertyFeatures)).Property(nameof(PropertyFeatures.HasBalcony)).EqualTo(true));
            Assert.That(result.Value, Is.SubsetOf(entriesWithBalcony));
            Assert.That(result.Value, Is.Not.SubsetOf(entriesWithoutBalcony));
        }

        [Test]
        public async Task BestOffersShouldReturnOffersOnlyWithBalconyAndLowestPricePerMeter()
        {
            //Arrange
            PolishCity city = _fixture.Create<PolishCity>();
            
            var entriesWithBalcony = GetEntryBuilder(city)
                .With(entry => entry.PropertyFeatures, () => _fixture
                    .Build<PropertyFeatures>()
                    .With(features => features.HasBalcony, true)
                    .With(features => features.HasElevator, false)
                    .With(features => features.HasBasementArea, false)
                    .With(features => features.ParkingPlaces, 0)
                    .Create())
                .CreateMany(_random.Next(5, 100));

            var lowestPrices = entriesWithBalcony
                .Select(entry => entry.PropertyPrice.PricePerMeter)
                .OrderBy(arg => arg)
                .Take(5);

            var entriesWithoutBalcony = GetEntryBuilder(city)
                .With(entry => entry.PropertyFeatures, () => _fixture
                    .Build<PropertyFeatures>()
                    .With(features => features.HasBalcony, false)
                    .Create())
                .CreateMany();

            _context.AddRange(entriesWithBalcony);
            _context.AddRange(entriesWithoutBalcony);
            await _context.SaveChangesAsync();

            //Act
            var result = await _controller.GetBestEntry(city);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Has.All.With.Property(nameof(Entry.PropertyFeatures)).Property(nameof(PropertyFeatures.HasBalcony)).EqualTo(true));
            Assert.That(result.Value.Select(entry => entry.PropertyPrice.PricePerMeter), Is.EquivalentTo(lowestPrices));
            Assert.That(result.Value, Is.SubsetOf(entriesWithBalcony));
        }

        [Test]
        public async Task BestOffersShouldReturnOffersOnlyWithAtLeastOneFeature()
        {
            //Arrange
            PolishCity city = _fixture.Create<PolishCity>();
            
            var entriesWithBalcony = GetEntryBuilder(city)
                .With(entry => entry.PropertyFeatures, () => _fixture
                    .Build<PropertyFeatures>()
                    .With(features => features.HasBalcony, true)
                    .Create())
                .Create();

            var entriesWithBalconyAndElevator = GetEntryBuilder(city)
                .With(entry => entry.PropertyFeatures, () => _fixture
                    .Build<PropertyFeatures>()
                    .With(features => features.HasBalcony, true)
                    .With(features => features.HasElevator, true)
                    .Create())
                .Create();

            var entriesWithElevator = GetEntryBuilder(city)
                .With(entry => entry.PropertyFeatures, () => _fixture
                    .Build<PropertyFeatures>()
                    .With(features => features.HasElevator, true)
                    .Create())
                .Create();

            var entriesWithBasementArea = GetEntryBuilder(city)
                .With(entry => entry.PropertyFeatures, () => _fixture
                    .Build<PropertyFeatures>()
                    .With(features => features.HasBasementArea, true)
                    .Create())
                .Create();

            var entriesWithParking = GetEntryBuilder(city)
                .With(entry => entry.PropertyFeatures, () => _fixture
                    .Build<PropertyFeatures>()
                    .With(features => features.ParkingPlaces)
                    .Create())
                .Create();

            var entriesWithoutFeatures = GetEntryBuilder(city)
                .With(entry => entry.PropertyFeatures, () => _fixture
                    .Build<PropertyFeatures>()
                    .With(features => features.HasBalcony, false)
                    .With(features => features.HasElevator, false)
                    .With(features => features.HasBasementArea, false)
                    .With(features => features.ParkingPlaces, (int?) null)
                    .Create())
                .CreateMany();

            _context.Add(entriesWithBalcony);
            _context.Add(entriesWithBalconyAndElevator);
            _context.Add(entriesWithElevator);
            _context.Add(entriesWithBasementArea);
            _context.Add(entriesWithParking);
            _context.Add(entriesWithBalcony);
            _context.AddRange(entriesWithoutFeatures);
            await _context.SaveChangesAsync();

            //Act
            var result = await _controller.GetBestEntry(city);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value,
                Has.All.With
                    .Property(nameof(Entry.PropertyFeatures)).Property(nameof(PropertyFeatures.HasBalcony)).True
                    .Or.Property(nameof(Entry.PropertyFeatures)).Property(nameof(PropertyFeatures.HasBasementArea)).True
                    .Or.Property(nameof(Entry.PropertyFeatures)).Property(nameof(PropertyFeatures.HasElevator)).True
                    .Or.Property(nameof(Entry.PropertyFeatures)).Property(nameof(PropertyFeatures.ParkingPlaces)).Not.Zero);

            Assert.That(result.Value, Is.Not.SubsetOf(entriesWithoutFeatures));
        }

        private IPostprocessComposer<Entry> GetEntryBuilder(PolishCity city) =>
            _fixture
                .Build<Entry>()
                .Without(entry => entry.Id)
                .With(entry => entry.PropertyAddress, () => _fixture
                    .Build<PropertyAddress>()
                    .With(address => address.City, city)
                    .Create());

        [TearDown]
        public async Task Teardown()
        {
            await _context.Database.EnsureDeletedAsync();
        }
    }
}