using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseConnection
{
    static public class DatabaseManager
    {
        static public void AddEntriesToDatabase(List<Entry> entries)
        {
            using var context = new DatabaseContext();
            foreach (var entry in entries)
            {
                context.Entries.Add(entry);
            }
            context.SaveChanges();
        }

        static public List<Entry> GetAllEntries()
        {
            var entries = new List<Entry>();
            using var context = new DatabaseContext();
            entries = context.Entries
                .Include(c => c.OfferDetails)
                    .ThenInclude(c => c.SellerContact)
                .Include(c => c.PropertyPrice)
                .Include(c => c.PropertyDetails)
                .Include(c => c.PropertyAddress)
                .Include(c => c.PropertyFeatures)
                .ToList();
            return entries;
        }

        static public List<Entry> GetEntriesForGivenRange(int first, int count)
        {
            var entries = new List<Entry>();
            using var context = new DatabaseContext();
            entries = context.Entries
                .Skip(first)
                .Take(count)
                .Include(c => c.OfferDetails)
                    .ThenInclude(c => c.SellerContact)
                .Include(c => c.PropertyPrice)
                .Include(c => c.PropertyDetails)
                .Include(c => c.PropertyAddress)
                .Include(c => c.PropertyFeatures)
                .ToList();
            return entries;
        }
    }
}
