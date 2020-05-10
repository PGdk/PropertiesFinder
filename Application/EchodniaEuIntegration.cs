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
            /*new EchodniaEuParser().Parse();
            var documentNode = GoToPropertyOffers();

            var offerUrls = new List<string>();

            while (documentNode != null) {
                GetOfferNodes(documentNode).ForEach(offerNode => offerUrls.Add(GetLinkToOffer(offerNode)));
                documentNode = GetNextPage(documentNode);
            }

            var processorCount = Environment.ProcessorCount;

            var offerUrlLists = offerUrls.Select((s, i) => new { s, i })
                                  .GroupBy(x => x.i % processorCount)
                                  .Select(g => g.Select(x => x.s).ToList())
                                  .ToList();

            var entriess = offerUrlLists.Select(list => Task.Factory.StartNew(obj =>
            {
                List<string> list = (List<string>)obj;
                return list.Select(url =>
                {
                    var web = new HtmlWeb();
                    // var documentNode = web.Load(url).DocumentNode;

                    return "YOlo";
                }).ToArray();
            }, list)).Select(t => t.Result).ToList();



            Console.WriteLine(entriess);*/
            return new EchodniaEuParser
            {
                WebPage = WebPage,
            }.Parse();
        }

        private List<HtmlNode> GetOfferNodes(HtmlNode documentNode)
        {
            return documentNode
                .Descendants("section")
                .Where(s => s.Attributes["id"].Value == "lista-ogloszen")
                .First()
                .Descendants("ul")
                .First()
                .Descendants("li")
                .ToList();
        }

        private string GetLinkToOffer(HtmlNode offerNode)
        {
            return offerNode.Descendants("a").First().Attributes["href"].Value;
        }

        private HtmlNode GetNextPage(HtmlNode documentNode)
        {
            var nextPageLinks = documentNode.Descendants("a")
                .Where(a => a.Attributes["data-gtm"]?.Value == "nowa_karta/nawigator/nastepna")
                .ToList();

            if (nextPageLinks.Count == 0)
            {
                return null;
            }

            var url = WebPage.Url + nextPageLinks.First().Attributes["href"].Value.Replace("/ogloszenia", "");

            var web = new HtmlWeb();
            return web.Load(url).DocumentNode;
        }

        private HtmlNode GoToPropertyOffers()
        {
            var url = WebPage.Url;
            var web = new HtmlWeb();
            var documentNode = web.Load(WebPage.Url).DocumentNode;

            var propertyOffersUrl = documentNode
                .Descendants("section")
                .Where(s => s.Attributes["id"]?.Value == "ogloszenia-kategorie")
                .First()
                .Descendants("a")
                .Where(a => a.InnerText.Contains("Nieruchomości"))
                .First()
                .Attributes["href"]
                .Value;

            return web.Load(propertyOffersUrl).DocumentNode;
        }
    }
}
