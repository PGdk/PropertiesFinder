using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace IntegrationApi.Interfaces
{
    public interface IOccasionalEntriesProvider
    {
        public Task<List<Entry>> GetByCity(PolishCity city, int limit);
    }
}
