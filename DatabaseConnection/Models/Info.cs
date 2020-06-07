using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseConnection.Models
{
    public class Info
    {
        public int ID { get; set; }
        public string ConnectionString { get; set; }
        public string IntegrationName { get; set; }
        public string StudentName { get; set; }
        public int StudentIndex { get; set; }
    }
}
