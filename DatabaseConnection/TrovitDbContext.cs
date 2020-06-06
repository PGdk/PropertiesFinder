using Microsoft.EntityFrameworkCore;
using Models;

namespace DatabaseConnection
{
    public class TrovitDbContext : DbContext
    {
        public DbSet<Entry> Entries { get; set; }
        public DbSet<Logs> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=MaciejK169013;Trusted_Connection=True;");
        }
    }
}