using HtmlAgilityPack;
using Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EchodniaEu
{
    class PropertyAddressParser : OfferParser<PropertyAddress>
    {
        private string[] Localization { get; set; }

        private string FullStreetName { get; set; }

        private string LocalizationStreet { get; set; }

        private string LocalizationBuildingNumber { get; set; }

        private PolishCity City
        {
            get
            {
                return (PolishCity)Enum.Parse(typeof(PolishCity), Localization[0], true);
            }
        }

        private string District
        {
            get
            {
                return Localization.Length == 3 ? Localization[1] : "N/A";
            }
        }

        private string StreetName
        {
            get
            {
                var pattern = "ul[. ]+[^0-9.,]*";
                return LocalizationStreet
                    ?? MatchRegex(pattern, FullStreetName)
                    ?? "N/A";
            }
        }

        private string DetailedAddress
        {
            get
            {
                if (StreetName == "N/A")
                {
                    return "N/A";
                }

                if (LocalizationBuildingNumber != null)
                {
                    return LocalizationBuildingNumber;
                }

                var pattern = $"{StreetName} *[0-9/]+";
                return MatchRegex(pattern, RawDescription)
                        ?.Replace(StreetName, "")
                        .Trim()
                        ?? "N/A";

            }
        }

        public PropertyAddressParser(HtmlDocument htmlDocument): base(htmlDocument)
        {
        }

        public override PropertyAddress Dump()
        {
            Localization = FindLocalization();
            LocalizationStreet = FindLocalizationStreet();
            LocalizationBuildingNumber = FindLocalizationBuildingNumber();
            FullStreetName = FindFullStreetName();

            return new PropertyAddress
            {
                City = City,
                District = District,
                StreetName = StreetName,
                DetailedAddress = DetailedAddress
            };
        }

        private string[] FindLocalization()
        {
            return GetOfferProperty(OfferPropertyLabel.Localization)?
                .Replace("\n", "")
                .Trim()
                .Split(',')
                .Select(part => part.Trim())
                .ToArray() ?? new string[0];
        }

        private string FindLocalizationStreet()
        {
            var localizationStreet = MatchRegex("\"lokalizacja_ulica\":\"[^\"]*\"", HtmlDocument.Text)?
                .Replace("\"", "")
                .Replace("lokalizacja_ulica:", "");

            return localizationStreet != null
                ? ConvertToUnicodeString(localizationStreet)
                : null;
        }

        private string FindLocalizationBuildingNumber()
        {
            return MatchRegex("\"lokalizacja_nr-budynku\":\"[^\"]*\"", HtmlDocument.Text)?
                .Replace("\"", "")
                .Replace("lokalizacja_nr-budynku:", "");
        }

        private string FindFullStreetName()
        {
            var address = LocalizationStreet;

            var fullStreetNameRegex = "ul[. ]+[^0-9.,)\n]+[0-9/]+";

            if (address != null)
            {
                address = MatchRegex(fullStreetNameRegex, address);
            }

            if (address != null)
            {
                return address;
            }

            return MatchRegex(fullStreetNameRegex, RawDescription);
        }

        private string ConvertToUnicodeString(string value)
        {
            return new Regex(@"\\[u]([0-9a-f]{4})")
                .Replace(value, match => ((char)Int32.Parse(match.Value.Substring(2), NumberStyles.HexNumber)).ToString());
        }
    }
}
