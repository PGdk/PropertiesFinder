using DatabaseConnection.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseConnection.Services
{
    public class DatabaseService : IDatabaseService
    {
        public List<Entry> GetEntries()
        {
            using DatabaseContext databaseContext = new DatabaseContext();
            return databaseContext.Entries.Include(e => e.OfferDetails).ThenInclude(e => e.SellerContact)
                            .Include(e => e.PropertyAddress)
                            .Include(e => e.PropertyDetails)
                            .Include(e => e.PropertyFeatures)
                            .Include(e => e.PropertyPrice).ToList();
        }
    }
}
