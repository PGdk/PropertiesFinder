using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseConnection
{
    public interface IDatabaseContext
    {
        DbSet<Entry> Entries { get; set; }

        void SaveChanges();
    }
}
