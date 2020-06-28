using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using AngleSharp;
using AngleSharp.Html.Parser;
using System.Net.Http;
using AngleSharp.Dom;
using System.Threading.Tasks;
using System.Linq;
using System.Text.Encodings;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace SprzedawaczIntegration
{
    public class SprzedawaczIntegration : IWebSiteIntegration
    {
        const string mainUrl = "https://sprzedawacz.pl";

        const string sellFlatsPath = "/nieruchomosci/mieszkania/sprzedaz/";
        const string rentFlatsPath = "/nieruchomosci/mieszkania/wynajem/";
        const string sellHousesPath = "/nieruchomosci/domy/sprzedaz/";
        const string rentHousesPath = "/nieruchomosci/domy/wynajem/";
        // "Ilość prezentowanych ogłoszeń została ograniczona do 100 pierwszych podstron.
        // Jeśli chcesz przejrzeć pozostałe ogłoszenia skorzystaj z wyszukiwarki, filtrów
        // lub ogranicz lokalizację."
        const int maxPages = 100;

        public WebPage WebPage { get; }
        public IDumpsRepository DumpsRepository { get; }
        public IEqualityComparer<Entry> EntriesComparer { get; }

        private readonly HttpClient client;

        private List<Tuple<string, OfferKind, bool>> urls = new List<Tuple<string, OfferKind, bool>>();

        private readonly AngleHelper _helper;

        private readonly EntryParser parser;

        public SprzedawaczIntegration(AngleHelper helper = null)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(mainUrl);
            _helper = helper ?? new AngleHelper();
            parser = new EntryParser(_helper);
        }

        public SprzedawaczIntegration(IDumpsRepository dumpsRepository,
            IEqualityComparer<Entry> equalityComparer)
        {
            DumpsRepository = dumpsRepository;
            EntriesComparer = equalityComparer;
            WebPage = new WebPage
            {
                Url = mainUrl,
                Name = "Sprzedawacz.pl Integration",
                WebPageFeatures = new WebPageFeatures
                {
                    HomeSale = true,
                    HomeRental = true,
                    HouseSale = true,
                    HouseRental = true
                }
            };
            client = new HttpClient();
            client.BaseAddress = new Uri(WebPage.Url);
        }

        public List<Entry>? GeneratePage(int pageNumber, int pageSize)
        {
            urls.AddRange(GetCategoryUrls($"{mainUrl}{rentFlatsPath}").Select(url => Tuple.Create($"{mainUrl}{url}", OfferKind.RENTAL, false)));
            urls.AddRange(GetCategoryUrls($"{mainUrl}{sellFlatsPath}").Select(url => Tuple.Create($"{mainUrl}{url}", OfferKind.SALE, false)));
            urls.AddRange(GetCategoryUrls($"{mainUrl}{rentHousesPath}").Select(url => Tuple.Create($"{mainUrl}{url}", OfferKind.RENTAL, true)));
            urls.AddRange(GetCategoryUrls($"{mainUrl}{sellHousesPath}").Select(url => Tuple.Create($"{mainUrl}{url}", OfferKind.SALE, true)));
            if (urls.Count() < pageNumber * pageSize)
                return null;
            var entries = urls.GetRange(pageNumber * pageSize, pageSize).Select(url => parser.GetEntryFromUrl(url)).ToList();
            entries.RemoveAll(entry => entry == null);
            return entries;
        }

        public Dump GenerateDump()
        {
            List<Entry> entries = new List<Entry>();

            var rentalFlats = GetCategoryUrls($"{mainUrl}{rentFlatsPath}");
            foreach(string url in rentalFlats)
            {
                entries.Add(parser.GetEntryFromUrl($"{mainUrl}{url}", OfferKind.RENTAL));
            }
            var saleFlats = GetCategoryUrls($"{mainUrl}{sellFlatsPath}");
            foreach (string url in saleFlats)
            {
                entries.Add(parser.GetEntryFromUrl($"{mainUrl}{url}", OfferKind.SALE));
            }
            var rentalHouses = GetCategoryUrls($"{mainUrl}{rentHousesPath}");
            foreach (string url in rentalHouses)
            {
                entries.Add(parser.GetEntryFromUrl($"{mainUrl}{url}", OfferKind.RENTAL, true));
            }
            var saleHouses = GetCategoryUrls($"{mainUrl}{sellHousesPath}");
            foreach (string url in saleHouses)
            {
                entries.Add(parser.GetEntryFromUrl($"{mainUrl}{url}", OfferKind.SALE, true));
            }

            return new Dump
            {
                DateTime = DateTime.Now,
                WebPage = WebPage,
                Entries = entries
            };
        }

        private List<string> GetCategoryUrls(string categoryUrl)
        {
            var urls = new List<string>();
            var firstPage = _helper.GetParsedHtmlFromUrl(categoryUrl);
            var pagesNumber = int.Parse(firstPage.QuerySelector(".paging").LastElementChild.Text());
            if (pagesNumber > maxPages)
                pagesNumber = maxPages;
            for (int i = 0; i <= pagesNumber; i++)
            {
                var listPage = _helper.GetParsedHtmlFromUrl($"{categoryUrl}?strona={i}");
                var offers = listPage.QuerySelectorAll(".ann-row .title a");
                foreach(IElement offer in offers)
                {
                    urls.Add(offer.Attributes.First(a => a.Name == "href").Value);
                }
            }
            return urls;
        }
    }
}
