using System;
using Utilities;

namespace nportal.pl
{
    class Program
    {
        static void Main(string[] args)
        {
            var ni = new NportalIntegration(new DumpFileRepository(), new NportalComparer());
            ni.GenerateDump();
        }
    }
}
