using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DatabaseConnection.Repositories
{
    public class EntriesRepository : IEntriesRepository
    {
        private readonly DatabaseContext _context;

        public EntriesRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<Entry>> FindAll(Nullable<int> offset, Nullable<int> limit)
        {
            return await GetFullyIncludedEntries()
                .Skip(offset ?? 0)
                .Take(limit ?? await CountAll())
                .ToListAsync();
        }

        public async Task<List<Entry>> FindByCityForSale(PolishCity city)
        {
            return await GetFullyIncludedEntries()
                .Where(e => city == e.PropertyAddress.City
                    && OfferKind.SALE == e.OfferDetails.OfferKind
                    && e.PropertyDetails.NumberOfRooms > 0
                )
                .ToListAsync();
        }

        public async Task<Entry> Find(int id)
        {
            return await GetFullyIncludedEntries()
                .SingleOrDefaultAsync(e => id == e.Id);
        }

        public async Task<int> Save(Entry entry)
        {
            _context.Entry(entry).State = EntityState.Modified;

            return await _context.SaveChangesAsync();
        }

        public bool Exists(int id)
        {
            return _context.Entries.Any(e => id == e.Id);
        }

        public async Task<int> CountAll()
        {
            return await _context.Entries.CountAsync();
        }

        private IQueryable<Entry> GetFullyIncludedEntries()
        {
            return _context.Entries
                .Include(e => e.OfferDetails)
                .ThenInclude(of => of.SellerContact)
                .Include(e => e.PropertyPrice)
                .Include(e => e.PropertyDetails)
                .Include(e => e.PropertyAddress)
                .Include(e => e.PropertyFeatures);
        }
    }
}
