using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DatabaseConnection
{
    public class PolskaTimesDBContext : DbContext
    {
        public DbSet<InfoStatus> infoStatus { get; set; }
        public DbSet<Entry> entries { get; set; }
        public PolskaTimesDBContext(DbContextOptions<PolskaTimesDBContext> options)
           : base(options)
        { }
    }
}