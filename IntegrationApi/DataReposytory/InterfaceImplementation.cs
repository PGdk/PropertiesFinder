using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection;
using Application;
using System.Data.Entity;
using System.Data.SqlClient;

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
            
            if (pageLimit != 0 && pageID != 0)
                entry = context.Entries.OrderBy(e => e.ID).Skip(pageLimit * (pageID - 1)).Take(pageLimit).ToList();
            else
                entry = context.Entries.ToList();
            return entry;
        }

        public IEnumerable<EntryDB> GetBestOffers(string districtName)
        {
            //zwraca 5 najtańszych ofert za m2 w podanej getem dzielnicy (propozycja 1 z polecenia projektowego)
            //jesli nie można pobrac 5 ofert zwraca null
            List<EntryDB> entry = new List<EntryDB>();
            var context = new EntryContexst();
            entry = context.Entries.Where(e => e.PropertyAddress.District == districtName).OrderBy(e => e.PropertyPrice.PricePerMeter).ToList();
            if (entry.Count() < 5)
            {
                return null;
            }
            return entry.GetRange(0,5);
        }
    }
}
