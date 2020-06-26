using Models;
using System.Collections.Generic;

namespace DatabaseConnection.Interfaces
{
    public interface IDatabaseService
    {
        public List<Entry> GetEntries();
    }
}
