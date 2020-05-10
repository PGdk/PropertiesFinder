using System;
using System.Collections.Generic;
using System.Text;

namespace SampleApp
{
    class DumpAdresowoPL
    {
        public string City { get; set; }
        public string District { get; set; }
        public string StreetName { get; set; }
        public int DetailedAddress { get; set; }
        public int NumberOfRooms { get; set; }
        public int? Balconies { get; set; }
        public decimal TotalGrossPrice { get; set; }
        public decimal Area { get; set; }
        public decimal PricePerMeter { get; set; }
        public int? FloorNumber { get; set; }
        public decimal? GardenArea { get; set; }
        public int? YearOfConstruction { get; set; }
        public decimal? BasementArea { get; set; }
        public int? OutdoorParkingPlaces { get; set; }
        public int? IndoorParkingPlaces { get; set; }
        public string UrlOffer { get; set; }
        public string RawDescription { get; set; }

        public DumpAdresowoPL(string city, string district, string streetName, int detailedAddress, int numberOfRooms, int? balconies, decimal totalGrossPrice, decimal area, decimal pricePerMeter, int? floorNumber, decimal? gardenArea, int? yearOfConstruction, decimal? basementArea, int? outdoorParkingPlaces, int? indoorParkingPlaces, string rawDescription, string urlOffer)
        {
            City = city;
            District = district;
            StreetName = streetName;
            DetailedAddress = detailedAddress;
            NumberOfRooms = numberOfRooms;
            Balconies = balconies;
            TotalGrossPrice = totalGrossPrice;
            Area = area;
            PricePerMeter = pricePerMeter;
            FloorNumber = floorNumber;
            GardenArea = gardenArea;
            YearOfConstruction = yearOfConstruction;
            BasementArea = basementArea;
            OutdoorParkingPlaces = outdoorParkingPlaces;
            IndoorParkingPlaces = indoorParkingPlaces;
            UrlOffer = urlOffer;
            RawDescription = rawDescription;
        }

        public void print()
        {
            Console.WriteLine("Miasto:" + City);
            Console.WriteLine("Dzielnica: " + District);
            Console.WriteLine("Ulica: " + StreetName);
            Console.WriteLine("Numer budynku: " + DetailedAddress);
            Console.WriteLine("Ilosc pokoi: " + NumberOfRooms);
            Console.WriteLine("Balkony: " + Balconies);
            Console.WriteLine("Cena: " + TotalGrossPrice);
            Console.WriteLine("Powierzchnia: " + Area);
            Console.WriteLine("Cena za 1m2: " + PricePerMeter);
            Console.WriteLine("Pietro: " + FloorNumber);
            Console.WriteLine("Powierzchnia dzialki: " + GardenArea);
            Console.WriteLine("Rok konstrukcji: " + YearOfConstruction);
            Console.WriteLine("Link do oferty: " + UrlOffer);
            Console.WriteLine("Opis oferty: " + RawDescription);
            Console.WriteLine();
        }
    }
}