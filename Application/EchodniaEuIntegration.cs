using HtmlAgilityPack;
using Interfaces;
using Models;
using System;
using EchodniaEu;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application
{
    class EchodniaEuIntegration : IWebSiteIntegration
    {
        public WebPage WebPage { get; }

        public IDumpsRepository DumpsRepository { get; }

        public IEqualityComparer<Entry> EntriesComparer { get; }

        public EchodniaEuIntegration(IDumpsRepository dumpRepository, IEqualityComparer<Entry> entriesComparer)
        {
            DumpsRepository = dumpRepository;
            EntriesComparer = entriesComparer;
            WebPage = new WebPage {
                Url = "https://echodnia.eu/ogloszenia",
                Name = "Echodnia.eu Integration",
                WebPageFeatures = new WebPageFeatures
                {
                    HomeSale = true,
                    HomeRental = false,
                    HouseSale = true,
                    HouseRental = false
                }
            };
        }

        public Dump GenerateDump()
        {
            return new EchodniaEuParser
            {
                WebPage = WebPage,
            }.Parse();
        }
    }
}
