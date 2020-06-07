using Microsoft.EntityFrameworkCore;
using Models;
using Models.Pomorska;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseConnection
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Entry> entries { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
    }
}
