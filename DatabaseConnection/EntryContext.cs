using Microsoft.EntityFrameworkCore;
using Models;

namespace DatabaseConnection
{
    public class EntryContext : DbContext
    {
        public const string ConnectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=FilipB137248;Integrated Security=True";
        public EntryContext(DbContextOptions<EntryContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlServer(ConnectionString);

        public DbSet<Entry> Entries { get; set; }
    }
}
