using Microsoft.EntityFrameworkCore;
using Models;

namespace DatabaseConnection
{
    public class EntryContext : DbContext
    {
        public EntryContext(DbContextOptions<EntryContext> options) : base(options)
        {

        }
        public DbSet<Entry> Entries { get; set; }
    }
}
