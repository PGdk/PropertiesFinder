using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseConnection.Models
{
    public class Log
    {
        public Guid Id { get; set; }
        public string HeaderValue { get; set; }
        public DateTime Time { get; set; }

        internal Log() { }

        public Log(string headerValue)
        {
            HeaderValue = headerValue;
        }
    }
}
