using DatabaseConnection;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleApp
{
    public class ParseOnePage
    {
        public List<Entry> ListToPrint;
        public ParseOnePage(int pageNumber)
        {
            using DatabaseContext database = new DatabaseContext();

            List<DumpAdresowoPL> myDumpAdresowoPLpageNumber = new List<DumpAdresowoPL>();
            MyParser myParser = new MyParser(myDumpAdresowoPLpageNumber, pageNumber);
            ListToPrint = new List<Entry>();

            foreach (var item in myDumpAdresowoPLpageNumber)
            {
                var newEntry = new Entry
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
                database.Entries.Add(newEntry);
                database.SaveChanges();

                ListToPrint.Add(newEntry);
            }
        }
        public List<Entry> GetListToPrint()
        {
            return ListToPrint;
        }
    }
}
