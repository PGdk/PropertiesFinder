using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseConnection
{
    public class DatabaseContext : DbContext
    {
        public static string connectionString = @"Data Source=DESKTOP-QTN7ND1\SQLEXPRESS;Initial Catalog=PropertiesFinder;Integrated Security=True";
        public DbSet<Entry> Entries { get; set; }
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
