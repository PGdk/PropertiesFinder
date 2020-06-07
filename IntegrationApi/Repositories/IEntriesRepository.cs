using DatabaseConnection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationApi.Repositories
{
    public interface IEntriesRepository
    {
        void AddEntries(IEnumerable<Entry> entries);

        List<Entry> GetEntriesFromSellerWithNumber(string phoneNumber);

        List<Entry> GetEntries();

        Entry GetEntry(long id);

        List<Entry> GetEntries(long idFrom, long idTo);

        void UpdateEntry(long id, Entry entry);
    }
}
