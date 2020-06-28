using System.Collections.Generic;
using Models;

namespace IntegrationApi.Interfaces
{
    public interface IEntriesAveragePricePerMeterCalculator
    {
        public decimal Calculate(List<Entry> entries);
    }
}
