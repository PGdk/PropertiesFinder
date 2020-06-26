using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Data.Common;

namespace DatabaseConnection
{
    public class DatabaseContext : DbContext, IDatabaseContext
    {
        public static string CONNECTION_STRING = new SqlConnectionStringBuilder()
        {
            IntegratedSecurity = true,
            DataSource = ".\\SQLEXPRESS",
            InitialCatalog = "NikodemG143227"

        }.ConnectionString;

        public DbSet<Entry> Entries { get; set; }

        public DbSet<RequestLog> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlServer(CONNECTION_STRING);
    }
}
