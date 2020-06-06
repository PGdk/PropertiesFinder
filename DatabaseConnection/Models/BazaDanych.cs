using Microsoft.EntityFrameworkCore;
using Models;

namespace DatabaseConnection
{
    public class BazaDanych : DbContext
    {
        public DbSet<Entry> Entries { get; set; }
        public DbSet<Log> Logs { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source =.\\SQLEXPRESS; Initial Catalog = Jan_M_170630; Integrated Security = True"); 
        }
    }
}
