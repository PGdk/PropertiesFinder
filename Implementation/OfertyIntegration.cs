using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DatabaseConnection;
using Extensions;
using HtmlAgilityPack;
using Interfaces;
using Models;
using Utilities;

namespace Implementation
{
    public class OfertyIntegration : IWebSiteIntegration
    {
        private static readonly Regex UrlRegex = new Regex(@"(?<url>https://[a-zA-Z0-9~_.,/-]+)");
        private static readonly Regex PriceRegex = new Regex(@"Cena: (?<price>[0-9 ]+|inf\. u dewelopera)");
        private static readonly List<string> OfferTypes = new List<string> {"pokoje", "mieszkania", "domy"};

        public OfertyIntegration(IDumpsRepository dumpsRepository,
            IEqualityComparer<Entry> equalityComparer)
        {
            DumpsRepository = dumpsRepository;
            EntriesComparer = equalityComparer;
            WebPage = new WebPage
            {
                Url = "https://www.oferty.net",
                Name = "oferty.net",
                WebPageFeatures = new WebPageFeatures
                {
                    HomeSale = true,
                    HomeRental = false,
                    HouseSale = false,
                    HouseRental = false
                }
            };
            Parser = new HtmlDocumentToEntryParser();
        }

        public IParser<HtmlDocument, Entry> Parser { get; set; }

        public Dump GenerateDump()
        {
            var web = new HtmlWeb();
            var entries = new List<Entry>();

            foreach (var offerType in OfferTypes)
            {
                foreach (var city in Enum.GetNames(typeof(PolishCity)))
                {
                    var uri = new Uri($"{WebPage.Url}/{offerType}/szukaj")
                        .AddParameter("page", "1") // For the sake of testing, only one page from each city
                        .AddParameter("ps[type]", "1")
                        .AddParameter("ps[foreign_search]", "0")
                        .AddParameter("ps[location][type]", "1")
                        .AddParameter("ps[location][text]", city)
                        .AddParameter("ps[transaction]", "1");

                    var doc = web.Load(uri);

                    var offerCollection = doc.DocumentNode.SelectNodes("//tr[contains(@class, 'property')]");

                    if (offerCollection != null)
                    {
                        foreach (var offer in offerCollection)
                        {
                            var offerLink = offer.GetAttributeValue("onclick", String.Empty);

                            if (UrlRegex.IsMatch(offerLink))
                            {
                                var url = UrlRegex.Match(offerLink).Groups["url"].Value;
                                var htmlNode = new HtmlWeb().Load(url);

                                var entry = Parser.Parse(htmlNode);

                                if (entry != null)
                                {
                                    entries.Add(entry);
                                }
                            }
                        }
                    }


                }

                if (entries.Count > 500) break;
            }

            return new Dump {Entries = entries, WebPage = WebPage, DateTime = DateTime.Now};
        }


        public WebPage WebPage { get; }
        public IDumpsRepository DumpsRepository { get; }
        public IEqualityComparer<Entry> EntriesComparer { get; }
    }
}