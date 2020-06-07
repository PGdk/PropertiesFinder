using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseConnection.Models
{
    public class Log
    {
        public long Id { get; set; }
        public string RequestId { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
