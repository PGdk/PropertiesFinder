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
            List<Entry> entries = new List<Entry>();
            using var context = new DatabaseContext();
            entries = context.Entries
                .ToList();
            entries = context.Entries
                .Include(c => c.OfferDetails)
                .ToList();
            entries = context.Entries
                .Include(c => c.PropertyPrice)
                .ToList();
            entries = context.Entries
                .Include(c => c.PropertyDetails)
                .ToList();
            entries = context.Entries
                .Include(c => c.PropertyAddress)
                .ToList();
            entries = context.Entries
                .Include(c => c.PropertyFeatures)
                .ToList();
            return entries;
        }
    }
}
