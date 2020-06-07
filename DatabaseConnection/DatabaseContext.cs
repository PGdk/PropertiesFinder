using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseConnectionNew
{
    public class DatabaseContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=.\SQLEXPRESS; Initial Catalog=MonikaK142691; integrated security=true");
        }

        public DbSet<Entry> Entries { get; set; }
    }
}
