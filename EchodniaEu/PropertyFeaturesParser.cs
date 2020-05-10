using HtmlAgilityPack;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EchodniaEu
{
    class PropertyFeaturesParser : OfferParser<PropertyFeatures>
    {
        private static string NoParkingPlacePattern = "brak miejsca parkingowego";
        private static string IndoorParingPlacePattern = "w garażu";
        private static string OutdoorParkingPlacePattern = "(przynależne na ulicy|parking strzeżony|pod wiatą)";
        public PropertyFeaturesParser(HtmlDocument htmlDocument) : base(htmlDocument)
        {
        }

        public override PropertyFeatures Dump()
        {
            return new PropertyFeatures
            {
                GardenArea = null,
                Balconies = GetBalconies(),
                BasementArea = null,
                IndoorParkingPlaces = GetIndoorParkingPlaces(),
                OutdoorParkingPlaces = GetOutdoorParkingPlaces()
            };
        }

        private int? GetIndoorParkingPlaces()
        {
            return GetParkingPlaces(IndoorParingPlacePattern);
        }
        private int? GetOutdoorParkingPlaces()
        {
            return GetParkingPlaces(OutdoorParkingPlacePattern);
        }

        private int? GetParkingPlaces(string pattern)
        {
            var parkingPlaceType = GetOfferProperty(OfferPropertyLabel.ParkingPlaceType);

            if (parkingPlaceType == null)
            {
                return null;
            }

            if (MatchRegex(NoParkingPlacePattern, parkingPlaceType) != null)
            {
                return 0;
            }

            var parkingPlaceCount = GetOfferProperty(OfferPropertyLabel.ParkingPlaceCount);

            if (MatchRegex(pattern, parkingPlaceType) != null)
            {
                return (int?)ParseToNullableDecimal(parkingPlaceCount) ?? 1;
            }

            return 0;
        }

        private int? GetBalconies()
        {
            var extraSpace = GetOfferProperty(OfferPropertyLabel.ExtraSpace);

            if (extraSpace?.Contains("balkon") ?? false)
            {
                return 1;
            }

            return null;
        }
    }
}
