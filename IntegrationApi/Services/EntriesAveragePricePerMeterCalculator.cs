using System.Collections.Generic;
using System.Linq;
using IntegrationApi.Interfaces;
using Models;

namespace IntegrationApi.Services
{
    public class EntriesAveragePricePerMeterCalculator : IEntriesAveragePricePerMeterCalculator
    {
        public decimal Calculate(List<Entry> entries)
        {
            return entries.Any()
                ? entries.Average(e => e.PropertyPrice.PricePerMeter)
                : 0.0m;
        }
    }
}
