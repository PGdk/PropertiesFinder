using System;
using DatabaseConnection.Interfaces;
using IntegrationApi.Interfaces;
using IntegrationApi.Services;
using Models;
using Moq;
using NUnit.Framework;

namespace IntegrationApi.Tests.Services
{
    [TestFixture]
    class OccasionalEntriesProviderTests
    {
        private Mock<IEntriesRepository> _repository;

        private Mock<IEntriesAveragePricePerMeterCalculator> _averagePricePerMeterCalculator;

        private Mock<IEntryPointsCalculator> _pointsCalculator;

        private OccasionalEntriesProvider _provider;

        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<IEntriesRepository>();
            _averagePricePerMeterCalculator = new Mock<IEntriesAveragePricePerMeterCalculator>();
            _pointsCalculator = new Mock<IEntryPointsCalculator>();

            _provider = new OccasionalEntriesProvider(
                _repository.Object,
                _averagePricePerMeterCalculator.Object,
                _pointsCalculator.Object
            );
        }

        [Test]
        public void GetEntries_GivenLimitLowerThanOne_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _provider.GetByCity(PolishCity.ALWERNIA, 0));
        }
    }
}
