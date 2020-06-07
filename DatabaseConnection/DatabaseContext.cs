using DatabaseConnection.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseConnection
{
    public class DatabaseContext : DbContext
    {
        public const string ConnectionString = @"Server=.\SQLEXPRESS;Database=MartaP159924;Trusted_Connection=True;";

        public DbSet<Entry> Entries { get; set; }
        public DbSet<Log> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }

    }
}
