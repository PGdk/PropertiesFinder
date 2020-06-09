using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationApi.Models
{
    public class Info
    {
        public string ConnectionString { get; set; }
        public string IntegrationName { get; set; }
        public string StudentName { get; set; }
        public int StudentIndex { get; set; }
    }
}
