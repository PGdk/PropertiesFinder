using Microsoft.EntityFrameworkCore; // doinstalowalem
using Models;

namespace DatabaseConnection
{
    public class DatabaseContext : DbContext // bazodanowy kontekst
    //public bo domyslnie jest internal a to powoduje ze widoczna jest w obrebie projektu
    {
        public DbSet<Entry> Entries { get; set; }  //tworzymy tabele ogloszen

        public DbSet<Log> Logs { get; set; } 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=.\SQLEXPRESS;Initial Catalog=DariuszK170624;Integrated Security=True");        
        }
    }
}
