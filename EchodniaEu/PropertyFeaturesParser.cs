using HtmlAgilityPack;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EchodniaEu
{
    public class PropertyFeaturesParser : OfferParser<PropertyFeatures>
    {
        private static string NoParkingPlacePattern = "brak miejsca parkingowego";
        private static string IndoorParingPlacePattern = "w garażu";
        private static string OutdoorParkingPlacePattern = "(przynależne na ulicy|parking strzeżony|pod wiatą)";
        private static string NoGardenPattern = "(nie ma|brak|bez).{0,5}ogr[oóu](d|t)[^z]";
        // https://regex101.com/r/BZ0QM3
        private static string GardenAreaPattern = "ogr[oóu](d|t)(.{0,5}powie....?ni)?(.{0,5}m2)?.{0,5}[0-9,.xX]+";
        private static string NoBasementAreaPattern = "(nie ma|brak|bez).{0,5}piwnic ";
        // https://regex101.com/r/Y7Mz6g
        private static string BasementAreaPattern = "piwnic(.{0,5}powie....?ni)?(.{0,5}m2)?.{0,5}[0-9,.xX]+";
        public PropertyFeaturesParser(HtmlDocument htmlDocument) : base(htmlDocument)
        {
        }

        public override PropertyFeatures Dump()
        {
            return new PropertyFeatures
            {
                GardenArea = GetGardenArea(),
                Balconies = GetBalconies(),
                BasementArea = GetBasementArea(),
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

        private decimal? GetGardenArea()
        {
            return GetArea(GardenAreaPattern, NoGardenPattern);
        }

        private decimal? GetBasementArea()
        {
            return GetArea(BasementAreaPattern, NoBasementAreaPattern);
        }

        private decimal? GetArea(string areaPattern, string noAreaPattern)
        {
            if (MatchRegex(noAreaPattern, RawDescription) != null)
            {
                return 0;
            }

            var match = MatchRegex(areaPattern, RawDescription);

            if (match == null)
            {
                return null;
            }

            match = match.Replace("m2", "");

            var dimensionsString = MatchRegex("[0-9][0-9.,]*(x|X)[0-9][0-9.,]*", match);
            if (dimensionsString != null)
            {
                var area = dimensionsString.ToLower()
                    .Split('x')
                    .Select(p => ParseToDecimal(p))
                    .Aggregate((x, y) => x * y);
                return area;
            }

            var areaString = MatchRegex("[0-9.,]+", match);

            if (areaString == null)
            {
                return null;
            }

            var parsed = decimal.TryParse(areaString, out var result);

            if (parsed)
            {
                return result;
            }

            return null;
        }
    }
}
