using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Data.Common;

namespace DatabaseConnection
{
    public interface IDatabaseContext
    {
        DbSet<Entry> Entries { get; set; }

        DbSet<RequestLog> Logs { get; set; }

        int SaveChanges();
    }
}