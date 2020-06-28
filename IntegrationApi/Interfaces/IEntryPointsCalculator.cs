using Models;

namespace IntegrationApi.Interfaces
{
    public interface IEntryPointsCalculator
    {
        public decimal Calculate(Entry entry, decimal averagePricePerMeter);
    }
}
