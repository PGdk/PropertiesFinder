using DatabaseConnection.Models;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DatabaseConnection
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {}
        public DbSet<Entry> Entries { get; set; }
        public DbSet<Info> Info { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=.\SQLEXPRESS;Initial Catalog=BartoszW137434;Integrated Security=True");
        }
    }
}
