using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace DatabaseConnection.Interfaces
{
    public interface IEntriesRepository
    {
        public Task<List<Entry>> FindAll(Nullable<int> offset, Nullable<int> limit);

        public Task<List<Entry>> FindByCityForSale(PolishCity city);

        public Task<Entry> Find(int id);

        public Task<int> Save(Entry entry);

        public bool Exists(int id);

        public Task<int> CountAll();
    }
}
