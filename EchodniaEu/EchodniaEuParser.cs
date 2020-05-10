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
            var pageUrls = Enumerable
                .Range(1, offersPage.GetPagesCount())
                .Select(i => $"https://echodnia.eu/ogloszenia/{i},22969,8433,n,fm,pk.html");


            var entries = pageUrls
                .AsParallel()
                .SelectMany(url => new OffersListPage(url)
                    .GetOfferUrls()
                    .AsParallel()
                    .Select(pageInfo =>
                    {
                        var page = new EntryParser(pageInfo.Item1, pageInfo.Item2);
                        return page.Exists() ? page.Dump() : null;
                    })
                    .Where(p => p != null)
                    .ToArray()
                ).ToList();

            return new Dump
            {
                WebPage = WebPage,
                DateTime = DateTime.Now,
                Entries = entries
            };
        }
    }
}
