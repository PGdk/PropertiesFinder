using DatabaseConnection.Models;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseConnection {
    public class MorizonContext : DbContext {
        public MorizonContext(DbContextOptions<MorizonContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer(@"Data Source=.\SQLEXPRESS;Initial Catalog=joannal165459;Integrated Security=True");
        }

        public DbSet<Entry> Entries { get; set; }

        public DbSet<Log> Logs{ get; set; }
    }
}
