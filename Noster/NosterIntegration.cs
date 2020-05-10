using HtmlAgilityPack;
using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Application
{
    class NosterIntegration : IWebSiteIntegration
    {
        public WebPage WebPage { get; }

        public IDumpsRepository DumpsRepository { get; }

        public IEqualityComparer<Entry> EntriesComparer { get; }
        public NosterIntegration(IDumpsRepository dumpsRepository,
            IEqualityComparer<Entry> equalityComparer)
        {
            DumpsRepository = dumpsRepository;
            EntriesComparer = equalityComparer;
            WebPage = new WebPage
            {
                Url = "https://www.noster-nieruchomosci.pl/",
                Name = "Noster nieruchomosci",
                WebPageFeatures = new WebPageFeatures
                {
                    HomeSale = true,
                    HomeRental = true,
                    HouseSale = true,
                    HouseRental = true
                }
            };
        }

        public Dump GenerateDump()
        {
            return new Dump
            {
                DateTime = DateTime.Now,
                WebPage = WebPage,
                Entries = GetEntriesAsync().Result
            };
        }

        private async Task<List<Entry>> GetEntriesAsync(string url = "https://www.noster-nieruchomosci.pl/oferty/")
        {
            var htmlDocument = new HtmlDocument();
            var client = new WebClient();
            string html;
            try
            {
                html = client.DownloadString(url);
            }
            catch (System.Net.WebException)
            {
                await Task.Delay(3000);
                try
                {
                    html = client.DownloadString(url);
                }
                catch (System.Net.WebException)
                {
                    await Task.Delay(10000);
                    html = client.DownloadString(url);
                }
            }
            htmlDocument.LoadHtml(html);
            var links = GetLinksToOffersAsync().Result;

            List<Entry> entries = new List<Entry>();
            var taskList = new List<Task<Entry>>();
            foreach (var link in links)
            {
                await Task.Delay(30);
                taskList.Add(Task.Run(() => GetNewEntry(link)));
            }
            var results = await Task.WhenAll(taskList);
            entries = results.OfType<Entry>().ToList();

            return entries;
        }

        private Entry GetNewEntry(string url)
        {
            var htmlDocument = new HtmlDocument();
            var client = new WebClient();
            var html = client.DownloadString(url);
            htmlDocument.LoadHtml(html);

            var entry = new Entry
            {
                OfferDetails = new OfferDetails
                {
                    Url = url,
                    CreationDateTime = DateTime.Now,
                    OfferKind = HtmlParser.GetOfferKind(htmlDocument),
                    LastUpdateDateTime = null,
                    SellerContact = HtmlParser.GetSellecContact(htmlDocument),
                    IsStillValid = true
                },
                PropertyPrice = new PropertyPrice
                {
                    TotalGrossPrice = HtmlParser.GetTotalGrossPrice(htmlDocument),
                    PricePerMeter = HtmlParser.GetPricePerMeter(htmlDocument),
                    ResidentalRent = HtmlParser.GetResidentalRent(htmlDocument)
                },
                PropertyDetails = new PropertyDetails
                {
                    Area = HtmlParser.GetArea(htmlDocument),
                    NumberOfRooms = HtmlParser.GetNumberOfRooms(htmlDocument),
                    FloorNumber = HtmlParser.GetFloorNumber(htmlDocument),
                    YearOfConstruction = HtmlParser.GetYearOfConstruction(htmlDocument)
                },
                PropertyAddress = new PropertyAddress
                {
                    City = HtmlParser.GetCity(htmlDocument),
                    District = HtmlParser.GetDistrict(htmlDocument),
                    StreetName = HtmlParser.GetStreetName(htmlDocument),
                    DetailedAddress = HtmlParser.GetDetailedAddress(htmlDocument)
                },
                PropertyFeatures = new PropertyFeatures
                {
                    GardenArea = HtmlParser.GetGardenArea(htmlDocument),
                    Balconies = HtmlParser.GetBalconies(htmlDocument),
                    BasementArea = HtmlParser.GetBasementArea(htmlDocument),
                    OutdoorParkingPlaces = HtmlParser.GetOutdoorParkingPlaces(htmlDocument),
                    IndoorParkingPlaces = HtmlParser.GetIndoorParkingPlaces(htmlDocument)
                },
                RawDescription = HtmlParser.GetRawDescription(htmlDocument)
            };
            return entry;
        }

        private async Task<List<string>> GetLinksToOffersAsync()
        {
            string mainUrl = "https://www.noster-nieruchomosci.pl/oferty/?page=";

            List<string> links = new List<string>();
            var taskList = new List<Task<List<string>>>();

            int numberOfPages = HtmlParser.GetNumberOfPages();
            // Pobieramy oferty tylko dla jednej strony
            for (int i = 0; i < 1; i++)
            {
                var url = mainUrl + i.ToString();
                await Task.Delay(40);
                taskList.Add(Task.Run(() => HtmlParser.GetLinksToOffersFromUrl(url)));
            }

            var results = await Task.WhenAll(taskList);
            foreach (var result in results)
            {
                links.AddRange(result);
            }

            return links;
        }
    }
}
