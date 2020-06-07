using DatabaseConnection;
using DatabaseConnection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationApi.Repositories
{
    public class LogsRepository : ILogsRepository
    {
        private readonly DatabaseContext databaseContext;

        public LogsRepository(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public void AddLog(Log log)
        {
            this.databaseContext.Add(log);
            this.databaseContext.SaveChanges();
        }
    }
}
