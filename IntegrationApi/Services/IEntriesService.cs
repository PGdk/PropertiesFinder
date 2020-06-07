using DatabaseConnection.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using MEntry = Models.Entry;

namespace IntegrationApi.Services
{
    public interface IEntriesService
    {
        void AddEntriesFromPage(int page);
        List<Entry> GetEntries();
        List<Entry> GetEntries(int pageId, int pageLimit);
        Entry GetEntry(long id);
        void UpdateEntry(long id, MEntry entry);
    }
}
