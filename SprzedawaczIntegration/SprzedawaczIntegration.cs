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
        private const NumberStyles style = NumberStyles.AllowDecimalPoint;

        public WebPage WebPage { get; }
        public IDumpsRepository DumpsRepository { get; }
        public IEqualityComparer<Entry> EntriesComparer { get; }

        private readonly HttpClient client;

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

        public Dump GenerateDump()
        {
            List<Entry> entries = new List<Entry>();

            var rentalFlats = GetCategoryUrls($"{mainUrl}{rentFlatsPath}");
            foreach(string url in rentalFlats)
            {
                entries.Add(GetEntryFromUrl($"{mainUrl}{url}", OfferKind.RENTAL));
            }
            var saleFlats = GetCategoryUrls($"{mainUrl}{sellFlatsPath}");
            foreach (string url in saleFlats)
            {
                entries.Add(GetEntryFromUrl($"{mainUrl}{url}", OfferKind.SALE));
            }
            var rentalHouses = GetCategoryUrls($"{mainUrl}{rentHousesPath}");
            foreach (string url in rentalHouses)
            {
                entries.Add(GetEntryFromUrl($"{mainUrl}{url}", OfferKind.RENTAL, true));
            }
            var saleHouses = GetCategoryUrls($"{mainUrl}{sellHousesPath}");
            foreach (string url in saleHouses)
            {
                entries.Add(GetEntryFromUrl($"{mainUrl}{url}", OfferKind.SALE, true));
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
            var firstPage = GetParsedHtmlFromUrl(categoryUrl);
            var pagesNumber = int.Parse(firstPage.QuerySelector(".paging").LastElementChild.Text());
            if (pagesNumber > maxPages)
                pagesNumber = maxPages;
            for (int i = 0; i <= pagesNumber; i++)
            {
                var listPage = GetParsedHtmlFromUrl($"{categoryUrl}?strona={i}");
                var offers = listPage.QuerySelectorAll(".ann-row .title a");
                foreach(IElement offer in offers)
                {
                    urls.Add(offer.Attributes.First(a => a.Name == "href").Value);
                }
            }
            return urls;
        }

        private Entry GetEntryFromUrl(string url, OfferKind kind, bool house = false)
        {
            var offerToParse = GetParsedHtmlFromUrl(url);
            var paragraphs = offerToParse.QuerySelectorAll(".c p");
            var floor = MatchOrNull(TryGetParagraphValueString(paragraphs, "Piętro:"), "([0-9]+|Parter)")?.Value;
            floor = floor == "Parter" ? "0" : floor;
            PolishCity city;
            var description = offerToParse.QuerySelector(".c .col-md-9 > p").Text();
            return new Entry
            {
                OfferDetails = new OfferDetails
                {
                    Url = url,
                    CreationDateTime = DateTime.Parse(offerToParse.QuerySelector(".ann-panel-right p:last-of-type b:last-of-type").Text()),
                    OfferKind = kind,
                    SellerContact = new SellerContact
                    {
                        //Not a proper email validation but it's simple
                        Email = MatchOrNull(description, @"\w+@\w+\.\w+")?.Value,
                        Telephone = GetParsedHtmlFromUrl(
                                $"{mainUrl}/ajax_daj_telefon.php?hash={offerToParse.QuerySelector(".tel-btn")?.GetAttribute("data-hash")}&token={offerToParse.QuerySelector(".tel-btn")?.GetAttribute("data-token")}")
                            ?.QuerySelector("a")
                            ?.Text() ?? MatchOrNull(description, "([0-9]{3}(-| )?){2}[0-9]{3}")?.Value,
                        Name = offerToParse.QuerySelector(".ann-panel-right > p > b").Text()
                    },
                    IsStillValid = true
                },
                PropertyAddress = new PropertyAddress
                {
                    City = Enum.TryParse(MatchOrNull(url, "m_(?<city>[a-z_]+)[-/]").Groups["city"]?.Value.ToUpper(), out city) ? city : default,
                    District = TryGetParagraphValueString(paragraphs, "Dzielnica:"),
                    StreetName = MatchOrNull(description, "(ul.|Ul.|UL.) [A-Za-z]+")?.Value,
                    DetailedAddress = MatchOrNull(description, "(ul. |Ul. |UL. )[A-Za-z]+ (?<detail>[0-9]{1,4}/?[0-9]{1,4})").Groups["detail"]?.Value
                },
                PropertyDetails = new PropertyDetails
                {
                    Area = house ? TryGetParagraphValueDecimal(paragraphs, "Pow. domu:")
                        : TryGetParagraphValueDecimal(paragraphs, "Powierzchnia:"),
                    FloorNumber = floor != null ? int.Parse(floor) : default,
                    NumberOfRooms = TryGetParagraphValueInt(paragraphs, "Liczba pokoi:"),
                    YearOfConstruction = TryGetParagraphValueInt(paragraphs, "Rok budowy:")
                },
                PropertyFeatures = new PropertyFeatures
                {
                    Balconies = Regex.IsMatch(description, "(B|b)alkony?") ? 1 : default,
                    BasementArea = null,
                    GardenArea = TryGetParagraphValueDecimal(paragraphs, "Pow. działki:"),
                    //May cause false possitives
                    IndoorParkingPlaces = Regex.IsMatch(description, "(G|g)araż") ? 1 : default,
                    OutdoorParkingPlaces = Regex.IsMatch(description, "(M|m)iejsce parkingowe") ? 1 : default
                },
                PropertyPrice = new PropertyPrice
                {
                    PricePerMeter = TryGetParagraphValueDecimal(paragraphs, "Cena za m2:"),
                    //No field, hard to guess from description
                    ResidentalRent = null,
                    TotalGrossPrice = GetPrice(offerToParse.QuerySelector(".c > .r > div.text-right > h2").Text())
                },
                RawDescription = description
            };
        }

        private decimal GetPrice(string str)
        {
            var possiblyEmpty = Regex.Matches(str, @"[0-9 \.]+").Select(m => m.Value).Aggregate((i, j) => i + j).Replace(" ", "");
            if (possiblyEmpty == "")
                return 0;
            return decimal.Parse(possiblyEmpty, style);
        }

        private string TryGetParagraphValueString(IHtmlCollection<IElement> paragraphs, string small)
        {
            var possiblyEmpty = paragraphs.Where(e => e.GetElementsByTagName("small").FirstOrDefault() != null).FirstOrDefault(e => e.GetElementsByTagName("small").FirstOrDefault().Text() == small)?.Text();
            if (possiblyEmpty == "")
                return null;
            return possiblyEmpty;
        }

        private int TryGetParagraphValueInt(IHtmlCollection<IElement> paragraphs, string small)
        {
            var str = MatchOrNull(TryGetParagraphValueString(paragraphs, small), "[0-9]+")?.Value;
            if (str == null || str == "")
                return default;
            return int.Parse(str);
        }

        private decimal TryGetParagraphValueDecimal(IHtmlCollection<IElement> paragraphs, string small)
        {
            var str = MatchOrNull(TryGetParagraphValueString(paragraphs, small), @"[0-9]+\.?[0-9]+")?.Value.Replace(".",",");
            if (str == null || str == "")
                return default;
            return decimal.Parse(str, style);
        }

        private IDocument GetParsedHtmlFromUrl(string url)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            return context.OpenAsync(new Url(url)).Result;
        }

        private Match MatchOrNull(string input, string rgx)
        {
            if (input != null)
                return Regex.Match(input, rgx);
            return null;
        }
    }
}
