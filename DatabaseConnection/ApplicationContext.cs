using Microsoft.EntityFrameworkCore;
using System;
using DatabaseConnection.Configurations;
using DatabaseConnection.Models;
using Models;

namespace DatabaseConnection
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {

        }

        public ApplicationDbContext(string connectionString): base(GetOptions(connectionString))
        {
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new EntryConfiguration());
            modelBuilder.ApplyConfiguration(new LogConfiguration());
        }
        private static DbContextOptions GetOptions(string connectionString)
        {
            return new DbContextOptionsBuilder().UseSqlServer(connectionString).Options;
        }

        public DbSet<Entry> Entries { get; set; }
        public DbSet<Log> Logs { get; set; }
    }
}
