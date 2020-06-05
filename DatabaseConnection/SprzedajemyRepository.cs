using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DatabaseConnection
{
    public class SprzedajemyRepository : ISprzedajemyRepository
    {
        public SprzedajemyRepository()
        {
        }

        public void AddEntries(IEnumerable<Entry> entries)
        {
            using SprzedajemyDbContext ctx = new SprzedajemyDbContext();
            ctx.AddRange(entries);
            ctx.SaveChanges();
        }

        public Entry GetEntry(int id)
        {
            using SprzedajemyDbContext ctx = new SprzedajemyDbContext();
            return ctx.Entries
                .Include(e => e.OfferDetails).ThenInclude(od => od.SellerContact)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyFeatures)
                .First(e => e.ID == id);
        }

        public IEnumerable<Entry> GetEntries()
        {
            using SprzedajemyDbContext ctx = new SprzedajemyDbContext();
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
            using SprzedajemyDbContext ctx = new SprzedajemyDbContext();
            int offset = (pageId - 1) * pageLimit;
            return ctx
                .Entries
                .Where(e => e.ID > offset && e.ID <= (offset + pageLimit))
                .Include(e => e.OfferDetails).ThenInclude(od => od.SellerContact)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyFeatures)
                .ToList();
        }

        public Entry UpdateEntry(int id, Entry entryToUpdate)
        {
            using SprzedajemyDbContext ctx = new SprzedajemyDbContext();
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
            using SprzedajemyDbContext ctx = new SprzedajemyDbContext();

            ctx.Logs.Add(new Logs() {
                DateTime = DateTime.Now,
                XRequestId = xRequestId
            });

            ctx.SaveChanges();
        }
    }
}