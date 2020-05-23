using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DatabaseConnection
{
    public class GazetaKrakowskaRepository : IGazetaKrakowskaRepository
    {
        private readonly GazetaKrakowskaContext gazetaKrakowskaContext;
        public GazetaKrakowskaRepository(GazetaKrakowskaContext GazetaKrakowskaContext)
        {
            this.gazetaKrakowskaContext = GazetaKrakowskaContext;
        }

        public void AddEntries(IEnumerable<EntryDb> entries)
        {
            gazetaKrakowskaContext.AddRange(entries);
            gazetaKrakowskaContext.SaveChanges();
        }

        public IEnumerable<EntryDb> GetEntries()
        {
            return gazetaKrakowskaContext
                .Entries
                .Include(e => e.OfferDetails).ThenInclude(od => od.SellerContact)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyFeatures)
                .ToList();
        }

        public IEnumerable<EntryDb> GetEntries(int pageId, int pageLimit)
        {
            int offset = (pageId - 1) * pageLimit;
            return gazetaKrakowskaContext
                .Entries
                .Where(e => e.Id > offset && e.Id <= (offset + pageLimit))
                .Include(e => e.OfferDetails).ThenInclude(od => od.SellerContact)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyFeatures)
                .ToList();
        }

        public EntryDb UpdateEntry(int id, EntryDb entryToUpdate)
        {
            try
            {
                entryToUpdate.Id = id;
                gazetaKrakowskaContext.Update(entryToUpdate);
                gazetaKrakowskaContext.SaveChanges();
            } catch (Exception)
            {
                return null;
            }

            return entryToUpdate;
        }

        public void AddLog(string xRequestId)
        {
            gazetaKrakowskaContext.Logs.Add(new Logs()
            {
                DateTime = DateTime.Now,
                XRequestId = xRequestId
            });

            gazetaKrakowskaContext.SaveChanges();
        }
    }
}
