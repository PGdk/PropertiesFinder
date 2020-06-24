using System.Collections.Generic;
using Models;

namespace IntegrationSprzedajemyService
{
    public interface ISprzedajemyService
    {
        IEnumerable<Entry> GetSpecialOffers();
    }
}
