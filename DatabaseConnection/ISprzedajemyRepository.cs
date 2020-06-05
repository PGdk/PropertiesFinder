using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseConnection
{
    public interface ISprzedajemyRepository
    {
        void AddEntries(IEnumerable<Entry> entries);
        IEnumerable<Entry> GetEntries();
        IEnumerable<Entry> GetEntries(int pageId, int pageLimit);
        Entry UpdateEntry(int id, Entry entryToUpdate);
        void AddLog(string xRequestId);
        Entry GetEntry(int id);
    }
}
