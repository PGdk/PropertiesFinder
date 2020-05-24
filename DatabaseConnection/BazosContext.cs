using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Interfaces;
using Models;

namespace DatabaseConnection
{
    public class BazosContext : DbContext
    {
        //public BazosContext() { }
        public BazosContext(DbContextOptions<BazosContext> options) : base(options)
        { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=stanislawk171978;Trusted_Connection=True"); //b => b.MigrationsAssembly("Application"));
        }

        public DbSet<Entry> Entries { get; set; }

    }
}
