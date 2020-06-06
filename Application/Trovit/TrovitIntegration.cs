using Interfaces;
using Models;
using Models.Trovit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Trovit
{
    public class TrovitIntegration : IWebSiteIntegration
    {
        public WebPage WebPage { get; } = new WebPage
        {
            Url = "https://mieszkania.trovit.pl",
            Name = "mieszkania.trovit.pl",
            WebPageFeatures = new WebPageFeatures
            {
                HomeSale = true,
                HomeRental = true,
                HouseSale = true,
                HouseRental = true
            }
        };

        public IDumpsRepository DumpsRepository { get; }

        public IEqualityComparer<Entry> EntriesComparer { get; }

        private static Filter[] filters = {
            new Filter {
                Place = TrovitPlaceKind.APARTMENT,
                Kind = OfferKind.SALE,
            },
            new Filter {
                Place = TrovitPlaceKind.APARTMENT,
                Kind = OfferKind.RENTAL,
            },
            new Filter {
                Place = TrovitPlaceKind.HOUSE,
                Kind = OfferKind.SALE,
            },
            new Filter {
                Place = TrovitPlaceKind.HOUSE,
                Kind = OfferKind.RENTAL,
            },
        };

        public TrovitIntegration(IDumpsRepository dumpsRepository, IEqualityComparer<Entry> equalityComparer)
        {
            DumpsRepository = dumpsRepository;
            EntriesComparer = equalityComparer;
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
                            Url = "http://mock.url",
                            OfferKind = OfferKind.SALE,
                            SellerContact = new SellerContact
                            {
                                Email = "mock@mock.com",
                                Name = "Mockej Mockowy",
                                Telephone =  "666 666 666"
                            }
                        },
                        PropertyAddress = new PropertyAddress { 
                            City = PolishCity.GDANSK,
                            DetailedAddress = String.Format("ul.Mockowa {1}/{0}", i, page ),
                            District = "Mockow",
                            StreetName = "Mockowa"
                        },
                        PropertyDetails = new PropertyDetails { 
                            FloorNumber = page % 6,
                            NumberOfRooms = page % 4,
                            YearOfConstruction = 2020 - page,
                            Area = 666 / page,
                        },
                        PropertyFeatures = new PropertyFeatures { 
                            Balconies = page % 3,
                            BasementArea = 0,
                            GardenArea = 0,
                            IndoorParkingPlaces = 0,
                            OutdoorParkingPlaces = 0
                        },
                        PropertyPrice = new PropertyPrice
                        {
                            PricePerMeter = 666,
                            ResidentalRent = 0,
                            TotalGrossPrice = 666 * i + page,
                        },
                        RawDescription = String.Format("Description for offer {0} on page {1}", i, page),
                        
                    }
                );
            }

            return entries;
        }

        public Dump GenerateDump()
        {
            var entries = new TorvitClient().Fetch(filters).Select(entry => {
                var extension = TrovitExtensionFactory.New(entry);

                if (extension == null)
                    return entry;

                var enhancer = extension.Handle();
                if (enhancer == null)
                    return entry;

                return enhancer.Enhance(entry);
            });

            return new Dump
            {
                DateTime = DateTime.Now,
                WebPage = WebPage,
                Entries = entries,
            };
        }
    }
}
