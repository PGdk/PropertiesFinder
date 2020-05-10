using Models.Trovit;
using System;
using System.Collections.Generic;
using Utilities.Trovit;

namespace Application.Trovit
{

    internal class TorvitClient
    {

        public List<TrovitEntry> Fetch(Filter[] filters) {
            var entries = new List<TrovitEntry>();
            foreach (var filter in filters)
            {
                entries.AddRange(Fetch(filter));
            }

            return entries;
        }

        public List<TrovitEntry> Fetch(Filter filter, int pageLimit = 0) {
            var entries = new List<TrovitEntry>();

            TrovitCursor cursor = new TrovitCursor(filter);
           
            while (true) {
                var content = HTTPClient.Get(cursor.URL());

                if (content == null) 
                    break;
                if (pageLimit > 0 && cursor.Page() == pageLimit + 1)
                    break;

                entries.AddRange(new TorvitParser().Parse(content));

                cursor.Next();
            }

            return entries;
        }
    }
}
