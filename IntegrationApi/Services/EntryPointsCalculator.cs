using System;
using IntegrationApi.Interfaces;
using Models;

namespace IntegrationApi.Services
{
    public class EntryPointsCalculator : IEntryPointsCalculator
    {
        private static readonly int MaxResidentalRent = 700;
        private static readonly int PricePerMeterPointsDivider = 500;
        private static readonly int ResidentalRentPointsDivider = 100;
        private static readonly decimal AreaPointsMultiplier = 0.05m;
        private static readonly decimal GardenAreaPointsMultiplier = 0.05m;
        private static readonly int GardenAreaMaxPointsNumber = 20;
        private static readonly int BasementPointsNumber = 3;
        private static readonly int BalconyPointsNumber = 3;
        private static readonly int OutdoorParkingPlacePointsNumber = 2;
        private static readonly int IndoorParkingPlacePointsNumber = 4;

        public decimal Calculate(Entry entry, decimal averagePricePerMeter)
        {
            decimal points = 0.0m;

            if (null != entry.PropertyPrice)
            {
                if (0 != entry.PropertyPrice.PricePerMeter)
                {
                    // Odpowiednio lepiej punktowane są mieszkania których cena za metr jest niższa od średniej.
                    // Każde 500zł różnicy = 1pkt
                    points += (averagePricePerMeter - entry.PropertyPrice.PricePerMeter) / PricePerMeterPointsDivider;
                }

                if (null != entry.PropertyPrice.ResidentalRent)
                {
                    // Dodatkowe punkty dla mieszkań z tańszym czynszem
                    // Każde 100zł różnicy = 1pkt
                    points += (MaxResidentalRent - (decimal)entry.PropertyPrice.ResidentalRent) / ResidentalRentPointsDivider;
                }
            }

            if (null != entry.PropertyDetails)
            {
                // Punkty za wielkość mieszkania
                points += entry.PropertyDetails.Area * AreaPointsMultiplier;
            }

            if (null != entry.PropertyFeatures)
            {
                if (null != entry.PropertyFeatures.GardenArea)
                {
                    // Dodatkowe punkty za wielkość ogrodu
                    // Ograniczone do 20pkt ponieważ nie chcę żeby domy z wielkimi ogrodami dysklasyfikowały inne nieruchomości
                    points += Math.Min((decimal)entry.PropertyFeatures.GardenArea * GardenAreaPointsMultiplier, GardenAreaMaxPointsNumber);
                }

                if (null != entry.PropertyFeatures.BasementArea)
                {
                    // Dodatkowe punkty za piwnicę
                    points += BasementPointsNumber;
                }

                // Dodatkowe punkty za każdy balkon
                points += (entry.PropertyFeatures.Balconies ?? 0) * BalconyPointsNumber;

                // Dodatkowe punkty za każde zewnętrzne miejsce postojowe
                points += (entry.PropertyFeatures.OutdoorParkingPlaces ?? 0) * OutdoorParkingPlacePointsNumber;

                // Dodatkowe punkty za każde wewnętrzne miejsce postojowe
                points += (entry.PropertyFeatures.IndoorParkingPlaces ?? 0) * IndoorParkingPlacePointsNumber;
            }

            return points;
        }
    }
}
