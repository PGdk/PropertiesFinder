using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DomyPL
{
    class JsonData
    {
        public Entry ToEntry()
        {

            Entry entry = new Entry();
            entry.PropertyAddress = new PropertyAddress();
            entry.PropertyDetails = new PropertyDetails();
            entry.PropertyPrice = new PropertyPrice();
            entry.OfferDetails = new OfferDetails();
            entry.PropertyFeatures = new PropertyFeatures();

            //Nie znalazłem na stronie informacji czy oferta jest aktualna
            entry.OfferDetails.IsStillValid = true;
            entry.OfferDetails.Url = property.url;
            if (property.transaction.Equals("sprzedaz"))
            {
                entry.OfferDetails.OfferKind = OfferKind.SALE;
            }
            else
            {
                 entry.OfferDetails.OfferKind = OfferKind.RENTAL;
            }

    Dictionary<string, string> polishCharacters = new Dictionary<string, string> {
                    { "Ą", "A" },{ "Ć", "C" },{ "Ę", "E" },{ "Ł", "L" },{ "Ń", "N" },{ "Ó", "O" },{ "Ś", "S" },{ "Ź", "Z" },{ "Ż", "Z" },{" ", "_" }};
            string cityName = polishCharacters.Aggregate(property.city.ToUpper(), (current, value) =>
                    current.Replace(value.Key, value.Value));
            if (Enum.IsDefined(typeof(PolishCity), cityName))
            {
                entry.PropertyAddress.City = (PolishCity)Enum.Parse(typeof(PolishCity), cityName);
            }


            entry.PropertyAddress.DetailedAddress = property.location;
            entry.PropertyAddress.District= property.district;

            entry.PropertyAddress.StreetName = property.street;

            entry.PropertyPrice.PricePerMeter = property.price_m2;
            entry.PropertyPrice.TotalGrossPrice = property.price;
            //jeśli cena to 0 oznacza to inf. u dewelopera

            entry.PropertyDetails.FloorNumber = property.floor;
            entry.PropertyDetails.NumberOfRooms = property.number_of_rooms;

            return entry;

        }
        public Property property { get; set; }

        public class Property
        {
            public string country { get; set; }
            public int residence_area { get; set; }
            public int floor { get; set; }
            public int number_of_rooms { get; set; }
            public int id { get; set; }
            public string url { get; set; }
            public int living_area { get; set; }
            public string street { get; set; }
            public string type { get; set; }
            public string transaction { get; set; }
            public string owner { get; set; }
            public string ownership { get; set; }
            public string finpack_type { get; set; }
            public int price { get; set; }
            public int price_m2 { get; set; }
            public string location { get; set; }
            public string market { get; set; }
            public string market_type { get; set; }
            public string photo_url { get; set; }
            public string county { get; set; }
            public string community { get; set; }
            public string district { get; set; }
            public string quarter { get; set; }
            public string province { get; set; }
            public string area { get; set; }
            public string city { get; set; }
        }


    }
}
