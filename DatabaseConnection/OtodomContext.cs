using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DatabaseConnection
{
    public class OtodomContext : DbContext
    {
        public DbSet<Entry> Entries { get; set; }
        public DbSet<Logs> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=projekt;Trusted_Connection=True;");
        }
    }
}
