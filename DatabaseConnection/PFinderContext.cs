using Microsoft.EntityFrameworkCore;
using Models;
using System;

namespace DatabaseConnection
{

    public class PFinderContext : DbContext
    {
        public const string connectionString = "Data Source=DESKTOP-8R68T6P\\SQLEXPRESS;Initial Catalog=MaciejS160519;Integrated Security=True";
        public virtual DbSet<Entry> Entries { get; set; }
        public DbSet<Tuple<string,DateTime>> Logs { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}