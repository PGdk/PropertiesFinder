using Models;
using System.Collections.Generic;

namespace DatabaseConnection
{
    public interface ITrovitRepository
    {
        void AddEntries(IEnumerable<Entry> entries);
        IEnumerable<Entry> GetEntries();
        IEnumerable<Entry> GetEntries(int pageId, int pageLimit);
        Entry UpdateEntry(int id, Entry entryToUpdate);
        void AddLog(string xRequestId);
        Entry GetEntry(int id);
    }
}
