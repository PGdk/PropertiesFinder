using DatabaseConnection.Models;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DatabaseConnection
{
    public class DatabaseContext : DbContext
    {
        public static readonly string ConnectionString = @"Data Source=DESKTOP-1B3Q6U7\SQLEXPRESS;Initial Catalog=KamilW167950;Integrated Security=True";

        public DbSet<Entry> Entries { get; set; }

        public DbSet<PropertyFeatures> PropertiesFeatures { get; set; }

        public DbSet<OfferDetails> OffersDetails { get; set; }

        public DbSet<PropertyAddress> PropertiesAddresses { get; set; }

        public DbSet<PropertyDetails> PropertiesDetails { get; set; }

        public DbSet<PropertyPrice> PropertiesPrices { get; set; }

        public DbSet<SellerContact> SellersContacts { get; set; }

        public DbSet<Log> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
    }
}
