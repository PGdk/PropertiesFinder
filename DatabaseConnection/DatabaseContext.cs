using DatabaseConnection.Models;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DatabaseConnection
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Entry> Entries { get; set; }
        public DbSet<Logs> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=.\SQLEXPRESS;Initial Catalog=PawelN170633;Integrated Security=True");
        }
    }
}
