using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection.Interfaces;
using IntegrationApi.Interfaces;
using Models;

namespace IntegrationApi.Services
{
    public class OccasionalEntriesProvider : IOccasionalEntriesProvider
    {
        private readonly IEntriesRepository _repository;

        private readonly IEntriesAveragePricePerMeterCalculator _averagePricePerMeterCalculator;

        private readonly IEntryPointsCalculator _pointsCalculator;

        public OccasionalEntriesProvider(
            IEntriesRepository repository,
            IEntriesAveragePricePerMeterCalculator averagePricePerMeterCalculator,
            IEntryPointsCalculator pointsCalculator
        ) {
            _repository = repository;
            _averagePricePerMeterCalculator = averagePricePerMeterCalculator;
            _pointsCalculator = pointsCalculator;
        }

        public async Task<List<Entry>> GetByCity(PolishCity city, int limit)
        {
            if (limit < 1)
            {
                throw new ArgumentException();
            }

            List<Entry> entries = await _repository.FindByCityForSale(city);

            decimal averagePricePerMeter = _averagePricePerMeterCalculator.Calculate(entries);

            return entries
                .OrderByDescending(e => _pointsCalculator.Calculate(e, averagePricePerMeter))
                .Take(limit)
                .ToList();
        }
    }
}
