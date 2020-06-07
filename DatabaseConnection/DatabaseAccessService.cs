using Microsoft.EntityFrameworkCore.Storage;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseConnection
{
    public class DatabaseAccessService
    {
        private readonly DatabaseContext _databaseContext;
        public DatabaseAccessService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public Entry getEntry(int id)
        {
            return _databaseContext.entries.Where(e => e.ID == id).FirstOrDefault();
        }
        public List<Entry> getEntries()
        {
            return _databaseContext.entries.ToList();
        }

        public int putEntry(Entry entry)
        {

            Entry entryToEdit = _databaseContext.entries.Find(entry.ID);
            if(entryToEdit != null)
            {
                entryToEdit.OfferDetails = entry.OfferDetails;
                entryToEdit.PropertyAddress = entry.PropertyAddress;
                entryToEdit.PropertyDetails = entry.PropertyDetails;
                entryToEdit.PropertyFeatures = entry.PropertyFeatures;
                entryToEdit.PropertyPrice = entry.PropertyPrice;
                entryToEdit.RawDescription = entry.RawDescription;
            }
            return _databaseContext.SaveChanges();
        }
    }
}
