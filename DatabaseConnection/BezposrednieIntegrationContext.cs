using DatabaseConnection.Models;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseConnection
{
    public class BezposrednieIntegrationContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=.\SQLEXPRESS;Initial Catalog=PropertiesFinder;Integrated Security=True");
        }

        public DbSet<Log> Logs { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<OfferDetails> OfferDetails { get; set; }
        public DbSet<SellerContact> SellerContacts { get; set; }
        public DbSet<PropertyFeatures> PropertyFeatures { get; set; }
        public DbSet<PropertyAddress> PropertyAddresses { get; set; }
        public DbSet<PropertyDetails> PropertyDetails { get; set; }
        public DbSet<PropertyPrice> PropertyPrices { get; set; }

    }
}
