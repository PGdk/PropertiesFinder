
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DatabaseConnection
{
    public class TrovitRepository : ITrovitRepository
    {
        public TrovitRepository()
        {
        }

        public void AddEntries(IEnumerable<Entry> entries)
        {
            using TrovitDbContext ctx = new TrovitDbContext();
            ctx.AddRange(entries);
            ctx.SaveChanges();
        }

        public Entry GetEntry(int id)
        {
            using TrovitDbContext ctx = new TrovitDbContext();
            return ctx.Entries
                .Include(e => e.OfferDetails).ThenInclude(od => od.SellerContact)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyFeatures)
                .FirstOrDefault(e => e.ID == id);
        }

        public IEnumerable<Entry> GetEntries()
        {
            using TrovitDbContext ctx = new TrovitDbContext();
            return ctx
                .Entries
                .Include(e => e.OfferDetails).ThenInclude(od => od.SellerContact)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyFeatures)
                .ToList();
        }

        public IEnumerable<Entry> GetEntries(int pageId, int pageLimit)
        {
            using TrovitDbContext ctx = new TrovitDbContext();
            return ctx
                .Entries
                .Where(e => e.ID > (pageId - 1) * pageLimit && e.ID <= ((pageId - 1) * pageLimit + pageLimit))
                .Include(e => e.OfferDetails).ThenInclude(od => od.SellerContact)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyFeatures)
                .ToList();
        }

        public Entry UpdateEntry(int id, Entry entryToUpdate)
        {
            using TrovitDbContext ctx = new TrovitDbContext();
            try
            {
                entryToUpdate.ID = id;
                ctx.Update(entryToUpdate);
                ctx.SaveChanges();
            }
            catch (Exception)
            {
                return null;
            }

            return entryToUpdate;
        }

        public void AddLog(string xRequestId)
        {
            using TrovitDbContext ctx = new TrovitDbContext();

            ctx.Logs.Add(new Logs()
            {
                DateTime = DateTime.Now,
                XRequestId = xRequestId
            });

            ctx.SaveChanges();
        }
    }
}