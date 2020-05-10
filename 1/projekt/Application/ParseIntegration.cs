using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace SampleApp
{
    public class ParseIntegration : IWebSiteIntegration
    {
        public WebPage WebPage { get; }
        public IDumpsRepository DumpsRepository { get; }

        public IEqualityComparer<Entry> EntriesComparer { get; }

        public ParseIntegration(IDumpsRepository dumpsRepository,
            IEqualityComparer<Entry> equalityComparer)
        {
            DumpsRepository = dumpsRepository;
            EntriesComparer = equalityComparer;
            WebPage = new WebPage
            {
                Url = "https://adresowo.pl",
                Name = "Adresowo.pl",
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
            var random = new Random();
            var randomValue = random.Next() % 10;

            List<DumpAdresowoPL> myDumpAdresowoPL = new List<DumpAdresowoPL>();
            MyParser myParser = new MyParser(myDumpAdresowoPL);

            Dump myDump = new Dump()
            {
                DateTime = DateTime.Now,
                WebPage = WebPage,
                myEntries = new List<Entry>()
            };

            foreach (var item in myDumpAdresowoPL)
            {
                Entry temp = new Entry
                {
                    OfferDetails = new OfferDetails
                    {
                        Url = item.UrlOffer,
                        CreationDateTime = DateTime.Now,
                        OfferKind = OfferKind.SALE,
                        SellerContact = new SellerContact
                        {
                            //Dane dostepne na stronie tylko po zarejestrowaniu sie
                            Email = "brak danych",
                            Telephone = "brak danych",
                            Name = "brak danych"
                        },
                        IsStillValid = true
                    },

                    PropertyDetails = new PropertyDetails
                    {
                        Area = item.Area,
                        FloorNumber = item.FloorNumber,
                        NumberOfRooms = item.NumberOfRooms,
                        YearOfConstruction = item.YearOfConstruction
                    },
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = item.PricePerMeter,
                        ResidentalRent = null,
                        TotalGrossPrice = item.TotalGrossPrice
                    },

                    PropertyAddress = new PropertyAddress
                    {
                        //City = PolishCity.SOPOT, //To Do - problem ze wzgledu na polskie znaki.
                        DetailedAddress = item.DetailedAddress.ToString(),
                        District = item.District,
                        StreetName = item.StreetName

                    },
                    PropertyFeatures = new PropertyFeatures
                    {
                        Balconies = item.Balconies,
                        BasementArea = null,
                        GardenArea = item.GardenArea,
                        IndoorParkingPlaces = null,
                        OutdoorParkingPlaces = null
                    },
                    RawDescription = item.RawDescription
                };
                myDump.myEntries.Add(temp);
            }
            return myDump;
        }
    }
}
