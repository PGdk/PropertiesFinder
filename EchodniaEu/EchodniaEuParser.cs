using Models;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EchodniaEu
{
    public class EchodniaEuParser: IPageParser
    {
        public WebPage WebPage { get; set; }

        public Dump Parse()
        {
            var offersPage = new OffersListPage(WebPage.Url).GetPropertyOffersPage();
            var offerUrls = new List<string>();
            var Entries = new List<Entry>();
            var pageUrls = Enumerable.Range(1, 1).Select(i => $"https://echodnia.eu/ogloszenia/{i},22969,8433,n,fm,pk.html");


            Entries = pageUrls
                .AsParallel()
                .SelectMany(url => new OffersListPage(url)
                    .GetOfferUrls()
                    .AsParallel()
                    .Select(url =>
                    {
                        var page = new EntryParser(url.Item1, url.Item2);
                        var exists = page.Exists();

                        if (!exists)
                        {
                            Console.WriteLine(url);
                        }
                        return exists ? page.Dump() : null;
                    })
                    .Where(p => p != null)
                    .ToArray()
                ).ToList();

            return new Dump
            {
                WebPage = WebPage,
                DateTime = new DateTime(),
                Entries = Entries
            };
        }
    }
}
