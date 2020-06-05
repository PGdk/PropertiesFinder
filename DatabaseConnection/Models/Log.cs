using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseConnection.Models
{
    public class Log
    {
        public long ID { get; set; }
        public DateTime Time { get; set; }
        public string Header { get; set; }
    }
}
