using System.Collections.Generic;

namespace DatabaseConnection
{
    public interface IGazetaKrakowskaRepository
    {
        void AddEntries(IEnumerable<EntryDb> entries);
        IEnumerable<EntryDb> GetEntries();
        IEnumerable<EntryDb> GetEntries(int pageId, int pageLimit);
        EntryDb UpdateEntry(int id, EntryDb entryToUpdate);
        void AddLog(string xRequestId);
        EntryDb GetEntry(int id);
    }
}