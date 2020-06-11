using System.Collections.Generic;
using DatabaseConnection;
using IntegrationApi.Models;

namespace IntegrationApi.Services
{
    public interface IGazetaKrakowskaService
    {
        IEnumerable<EntryDb> GetSpecialEntries();
    }
}
