using Microsoft.EntityFrameworkCore;

namespace DatabaseConnection
{
    public class DatabaseContext : DbContext
    {
        public static readonly string ConnectionString = @"Data Source=DESKTOP-1B3Q6U7\SQLEXPRESS;Initial Catalog=KamilW167950;Integrated Security=True";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
    }
}
