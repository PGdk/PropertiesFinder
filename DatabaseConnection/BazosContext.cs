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
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=Bazos;Trusted_Connection=True"); //b => b.MigrationsAssembly("Application"));
        }

        public DbSet<EntryId> Entries { get; set; }

    }
}
