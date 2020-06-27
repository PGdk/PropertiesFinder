using DatabaseConnection.Models;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Bezposrednie;
using System.Text;
using Utilities;

namespace DatabaseConnection
{
    public class BezposrednieIntegrationRepo
    {
        private Bezposrednie.BezposrednieIntegration bezposredniaWebSiteIntegration;
        Dump dump;

        public BezposrednieIntegrationRepo() { }

        public IEnumerable<Entry> DownloadAllEntries()
        {
            bezposredniaWebSiteIntegration = new BezposrednieIntegration(new DumpFileRepository(), new BezposrednieComparer());
            dump = this.bezposredniaWebSiteIntegration.GenerateDump();
            return dump.Entries.ToList();
        }

        public IEnumerable<Entry> Split(int page)
        {
            bezposredniaWebSiteIntegration = new BezposrednieIntegration(new DumpFileRepository(), new BezposrednieComparer());
            dump = this.bezposredniaWebSiteIntegration.GenerateDump();

            var numberOfEntries = dump.Entries.Count();
            List<Entry> PagedEntries = new List<Entry>();
            if (page * 5 > numberOfEntries)
            {
                for (int counter = (page - 1) * 5; counter < numberOfEntries - ((page - 1) * 5); counter++)
                {
                    PagedEntries.Add(dump.Entries.ToList()[counter]);
                }
            }
            else
            {
                for (int counter = (page - 1) * 5; counter < page * 5; counter++)
                {
                    PagedEntries.Add(dump.Entries.ToList()[counter]);
                }
            }
            return PagedEntries;
        }

        public void AddLog(string Header)
        {
            using (var Context = new BezposrednieIntegrationContext())
            {
                Context.Logs.Add(new Log()
                {
                    Header = Header,
                    Time = DateTime.Now
                });
                Context.SaveChangesAsync();
            }
        }

        public void AddEntry(IEnumerable<Entry> Entry)
        {
            using (var Context = new BezposrednieIntegrationContext())
            {
                Context.AddRange(Entry);
                Context.SaveChanges();
            }
        }

        public Entry GetEntry(string id)
        {
            using (var Context = new BezposrednieIntegrationContext())
            {
                var entries = Context
                    .Entries.Include(x => x.OfferDetails).ThenInclude(y => y.SellerContact)
                    .Include(x => x.PropertyPrice)
                    .Include(x => x.PropertyDetails)
                    .Include(x => x.PropertyAddress)
                    .Include(x => x.PropertyFeatures).ToList();
                return entries.FirstOrDefault(x => x.ID == int.Parse(id));
            }
        }

        public IEnumerable<Entry> GetEntries()
        {
            using (var Context = new BezposrednieIntegrationContext())
                return Context
                    .Entries
                    .Include(x => x.OfferDetails).ThenInclude(y => y.SellerContact)
                    .Include(x => x.PropertyPrice)
                    .Include(x => x.PropertyDetails)
                    .Include(x => x.PropertyAddress)
                    .Include(x => x.PropertyFeatures).ToList();
        }
    }
}
