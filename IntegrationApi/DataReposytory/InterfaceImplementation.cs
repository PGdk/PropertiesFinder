using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection;
using Application;
using System.Data.Entity;

namespace IntegrationApi.DataReposytory
{
    public class InterfaceImplementation : IRepo
    {

        public string GetInfo()
        {
            string ret = "connectionString" + ":" + @"Data Source =.\SQLEXPRESS; Initial Catalog=MikolajP158074; Integrated Security=SSPI" + "," +
                "" + "integrationName" + ":" + "odwlasciciela" + "," +
                "" + "studentName" + ":" + "Mikolaj" + "," +
                ""  + "studentIndex" + ":" + "158074";
            return ret;
        }

        public int LoadPage(int pageNumber)
        {
            int loadOK;
            FillDB fill = new FillDB();
            loadOK = fill.Start(pageNumber);

            return loadOK;
        }
        public EntryDB GetEntry(int id)
        {
            EntryDB entry = new EntryDB();
            var context = new EntryContexst();
            entry = context.Entries.Find(id);
            return entry;
        }

        public IEnumerable<EntryDB> GetEntrys(int pageLimit, int pageID)
        {
           List<EntryDB> entry = new List<EntryDB>();
            var context = new EntryContexst();
            

            /*
                foreach (EntryDB ent in context.Entries)
                {
                    entry.Add(ent);
                }
            */
            if (pageLimit != 0 && pageID != 0)
                ///entry = entry.GetRange (((pageLimit * pageID) - pageLimit), pageLimit);
                entry = context.Entries.OrderBy(e => e.ID).Skip(pageLimit * (pageID - 1)).Take(pageLimit).ToList();
            else
                entry = context.Entries.ToList();

            return entry;
        }
        /*
        public IEnumerable<EntryDB> GetEntries()
        {



            throw new NotImplementedException();
        }
        */

    }
}
