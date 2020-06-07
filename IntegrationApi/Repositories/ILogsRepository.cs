using DatabaseConnection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationApi.Repositories
{
    public interface ILogsRepository
    {
        void AddLog(Log log);
    }
}
