using Microsoft.EntityFrameworkCore;

namespace DatabaseConnection
{
    public class GazetaKrakowskaContext: DbContext
    {
        public DbSet<EntryDb> Entries { get; set; }
        public DbSet<Logs> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=KamilD170100;Trusted_Connection=True;");
        }
    }
}
