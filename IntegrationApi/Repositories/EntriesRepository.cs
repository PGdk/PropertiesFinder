using DatabaseConnection;
using DatabaseConnection.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace IntegrationApi.Repositories
{
    public class EntriesRepository : IEntriesRepository
    {
        private readonly DatabaseContext databaseContext;

        public EntriesRepository(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public void AddEntries(IEnumerable<Entry> entries)
        {
            this.databaseContext.AddRange(entries);
            this.databaseContext.SaveChanges();
        }
        public List<Entry> GetEntriesFromSellerWithNumber(string phoneNumber)
            => this.databaseContext
                .Entries
                .Where(e => e.OfferDetails.SellerContact.Telephone == phoneNumber)
                .Include(e => e.OfferDetails)
                .ThenInclude(od => od.SellerContact)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyFeatures)
                .ToList();

        public List<Entry> GetEntries()
            => this.databaseContext
                .Entries
                .Include(e => e.OfferDetails)
                .ThenInclude(od => od.SellerContact)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyFeatures)
                .ToList();

        public Entry GetEntry(long id)
            => this.databaseContext
                .Entries
                .Include(e => e.OfferDetails)
                .ThenInclude(od => od.SellerContact)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyFeatures)
                .FirstOrDefault(e => e.Id == id);

        public List<Entry> GetEntries(long idFrom, long idTo)
            => this.databaseContext
                .Entries
                .Where(e => e.Id > idFrom && e.Id <= idTo)
                .Include(e => e.OfferDetails)
                .ThenInclude(od => od.SellerContact)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyFeatures)
                .ToList();



        public void UpdateEntry(long id, Entry entry)
        {
            if (!this.databaseContext.Entries.Any(e => e.Id == id))
            {
                throw new KeyNotFoundException();
            }
            else
            {
                entry.Id = id;

                this.databaseContext.Update(entry);
                this.databaseContext.SaveChanges();
            }
        }
    }
}
