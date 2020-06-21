using System;
using System.Collections.Generic;
using System.Text;
using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;
namespace Application.Otodom
{
        public class OtodomIntegration : IWebSiteIntegration
        {
            int maximumAds = 200; //maksymalna ilość ofert
            List<String> linksToAdvertisements;
            public WebPage WebPage { get; }
            public IDumpsRepository DumpsRepository { get; }
            public IEqualityComparer<Entry> EntriesComparer { get; }
            public OtodomIntegration(IDumpsRepository dumpsRepository,
            IEqualityComparer<Entry> equalityComparer)
            {
                DumpsRepository = dumpsRepository;
                EntriesComparer = equalityComparer;
                WebPage = new WebPage
                {
                    Url = "https://www.otodom.pl/sprzedaz/mieszkanie/?nrAdsPerPage=72",
                    Name = "Otodom WebSite Integration",
                    WebPageFeatures = new WebPageFeatures
                    {
                        HomeSale = true,
                        HomeRental = true,
                        HouseSale = true,
                        HouseRental = true
                    }
                };
            }

        public IEnumerable<Entry> GetOffersByPage(int page)
        {
            var entries = new List<Entry>();

            for (int i = 0; i < 25; i++)
            {
                entries.Add(
                    new Entry
                    {
                        OfferDetails = new OfferDetails
                        {
                            CreationDateTime = new DateTime(),
                            IsStillValid = true,
                            LastUpdateDateTime = new DateTime(),
                            Url = "http://oto.url",
                            OfferKind = OfferKind.SALE,
                            SellerContact = new SellerContact
                            {
                                Email = "oto@oto.com",
                                Name = "Oto Dom",
                                Telephone = "600 500 400"
                            }
                        },
                        PropertyAddress = new PropertyAddress
                        {
                            City = PolishCity.GDANSK,
                            DetailedAddress = String.Format("ul.Prosta {1}/{0}", i, page),
                            District = "Oto",
                            StreetName = "Prosta"
                        },
                        PropertyDetails = new PropertyDetails
                        {
                            FloorNumber = "",
                            NumberOfRooms = page % 4,
                            YearOfConstruction = 2020 - page,
                            Area = 666 / page,
                        },
                        PropertyFeatures = new PropertyFeatures
                        {
                            Balconies = page % 3,
                            BasementArea = 0,
                            GardenArea = 0,
                            IndoorParkingPlaces = 0,
                            OutdoorParkingPlaces = 0
                        },
                        PropertyPrice = new PropertyPrice
                        {
                            PricePerMeter = 765,
                            ResidentalRent = 0,
                            TotalGrossPrice = 765 * i + page,
                        },
                        RawDescription = String.Format("Description for offer {0} on page {1}", i, page),

                    }
                );
            }

            return entries;
        }


        public Dump GenerateDump()
            {
                linksToAdvertisements = new List<String>();
                var Entries = new List<Entry>();
                var htmlWeb = new HtmlWeb();
                int maxLoopCounter = maximumAds / 72;
                for (int i = 1; i < maxLoopCounter + 1; i++)
                {
                    if (linksToAdvertisements.Count() == maximumAds)
                    {
                        break;
                    }
                 
                    var document = htmlWeb.Load(WebPage.Url + "&page=" + i);

                    var nodes = document.DocumentNode.SelectNodes("//*[@class = 'offer-item-details']");

                    foreach (var node in nodes)
                    {
                        var htmlNode = node.Descendants("h3").Select(node => node.SelectSingleNode("a[@href]")).Where(node => node.Attributes["href"] != null).First();
                        {
                            linksToAdvertisements.Add(htmlNode.Attributes["href"].Value);
                        }
                    }
                }


                foreach (var link in linksToAdvertisements)
                {

                    var documentWithDetails = htmlWeb.Load(link);
                    var jsonObject = documentWithDetails.DocumentNode.Descendants().Where(n => n.Name == "script" && n.Id == "server-app-state").First();
                    OtodomJsonParser otodomJsonParser = new OtodomJsonParser();
                    Entries.Add(otodomJsonParser.GetEntry(jsonObject.InnerText.ToString()));
                }

                Dump dump = new Dump();
                dump.Entries = Entries;
                dump.WebPage = WebPage;
                dump.DateTime = DateTime.Now;
                return dump;
            }
        }

}