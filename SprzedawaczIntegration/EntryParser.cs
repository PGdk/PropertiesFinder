using AngleSharp.Dom;
using Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SprzedawaczIntegration
{
    public class EntryParser
    {
        private const NumberStyles style = NumberStyles.AllowDecimalPoint;
        const string mainUrl = "https://sprzedawacz.pl";
        private readonly AngleHelper _helper;

        public EntryParser(AngleHelper helper)
        {
            _helper = helper;
        }

        public Entry GetEntryFromUrl(Tuple<string, OfferKind, bool> tuple)
        {
            return GetEntryFromUrl(tuple.Item1, tuple.Item2, tuple.Item3);
        }
        public Entry GetEntryFromUrl(string url, OfferKind kind, bool house = false)
        {
            var offerToParse = _helper.GetParsedHtmlFromUrl(url);
            if (offerToParse != null)
            {
                var paragraphs = offerToParse.QuerySelectorAll(".c p");
                var floor = MatchOrNull(TryGetParagraphValueString(paragraphs, "Piętro:"), "([0-9]+|Parter)")?.Value;
                floor = floor == "Parter" ? "0" : floor;
                PolishCity city;
                var description = offerToParse.QuerySelector(".c .col-md-9 > p").Text();
                return new Entry
                {
                    OfferDetails = new OfferDetails
                    {
                        Url = url,
                        CreationDateTime = DateTime.Parse(offerToParse.QuerySelector(".ann-panel-right p:last-of-type b:last-of-type").Text()),
                        OfferKind = kind,
                        SellerContact = new SellerContact
                        {
                            //Not a proper email validation but it's simple
                            Email = MatchOrNull(description, @"\w+@\w+\.\w+")?.Value,
                            Telephone = _helper.GetParsedHtmlFromUrl(
                                    $"{mainUrl}/ajax_daj_telefon.php?hash={offerToParse.QuerySelector(".tel-btn")?.GetAttribute("data-hash")}&token={offerToParse.QuerySelector(".tel-btn")?.GetAttribute("data-token")}")
                                ?.QuerySelector("a")
                                ?.Text() ?? MatchOrNull(description, "([0-9]{3}(-| )?){2}[0-9]{3}")?.Value,
                            Name = offerToParse.QuerySelector(".ann-panel-right > p > b").Text()
                        },
                        IsStillValid = true
                    },
                    PropertyAddress = new PropertyAddress
                    {
                        City = Enum.TryParse(MatchOrNull(url, "m_(?<city>[a-z_]+)[-/]").Groups["city"]?.Value.ToUpper(), out city) ? city : default,
                        District = TryGetParagraphValueString(paragraphs, "Dzielnica:"),
                        StreetName = MatchOrNull(description, @"(ul.|Ul.|UL.) \p{L}+")?.Value,
                        DetailedAddress = MatchOrNull(description, @"(ul.|Ul.|UL.) \p{L}+ (?<detail>[0-9]{1,4}/?[0-9]{1,4})").Groups["detail"]?.Value
                    },
                    PropertyDetails = new PropertyDetails
                    {
                        Area = house ? TryGetParagraphValueDecimal(paragraphs, "Pow. domu:")
                            : TryGetParagraphValueDecimal(paragraphs, "Powierzchnia:"),
                        FloorNumber = floor != null ? int.Parse(floor) : default,
                        NumberOfRooms = TryGetParagraphValueInt(paragraphs, "Liczba pokoi:"),
                        YearOfConstruction = TryGetParagraphValueInt(paragraphs, "Rok budowy:")
                    },
                    PropertyFeatures = new PropertyFeatures
                    {
                        Balconies = Regex.IsMatch(description, "(B|b)alkony?") ? 1 : default,
                        BasementArea = GetBasement(description),
                        GardenArea = TryGetParagraphValueDecimal(paragraphs, "Pow. działki:"),
                        //May cause false possitives
                        IndoorParkingPlaces = Regex.IsMatch(description, "(G|g)araż") ? 1 : default,
                        OutdoorParkingPlaces = Regex.IsMatch(description, "(M|m)iejsce parkingowe") ? 1 : default
                    },
                    PropertyPrice = new PropertyPrice
                    {
                        PricePerMeter = TryGetParagraphValueDecimal(paragraphs, "Cena za m2:"),
                        //No field, hard to guess from description
                        ResidentalRent = null,
                        TotalGrossPrice = GetPrice(offerToParse.QuerySelector(".c > .r > div.text-right > h2").Text())
                    },
                    RawDescription = description
                };
            }
            return null;
        }
        private int TryGetParagraphValueInt(IHtmlCollection<IElement> paragraphs, string small)
        {
            var str = MatchOrNull(TryGetParagraphValueString(paragraphs, small), "[0-9]+")?.Value;
            if (String.IsNullOrWhiteSpace(str))
                return default;
            return int.Parse(str);
        }

        private decimal TryGetParagraphValueDecimal(IHtmlCollection<IElement> paragraphs, string small)
        {
            var str = MatchOrNull(TryGetParagraphValueString(paragraphs, small), @"[0-9]+\.?[0-9]+")?.Value.Replace(".", ",");
            if (String.IsNullOrWhiteSpace(str))
                return default;
            return decimal.Parse(str, style, CultureInfo.GetCultureInfo("pl-PL"));
        }

        private Match MatchOrNull(string input, string rgx)
        {
            if (input != null)
                return Regex.Match(input, rgx);
            return null;
        }

        private decimal? GetBasement(string desc)
        {
            var possiblyEmpty = Regex.Match(desc, @"(P|p)iwnica (?<barea>[0-9,\.]+) ?(mkw|m2|m\\^2|metrów)").Groups["barea"]?.Value;
            if (String.IsNullOrWhiteSpace(possiblyEmpty))
                return null;
            return decimal.Parse(possiblyEmpty, style, CultureInfo.GetCultureInfo("pl-PL"));
        }

        private decimal GetPrice(string str)
        {
            var possiblyEmpty = Regex.Matches(str, @"[0-9 \.]+").Select(m => m.Value).Aggregate((i, j) => i + j).Replace(" ", "");
            if (String.IsNullOrWhiteSpace(possiblyEmpty))
                return 0;
            return decimal.Parse(possiblyEmpty, style, CultureInfo.GetCultureInfo("pl-PL"));
        }

        private string TryGetParagraphValueString(IHtmlCollection<IElement> paragraphs, string small)
        {
            var possiblyEmpty = paragraphs.Where(e => e.GetElementsByTagName("small").FirstOrDefault() != null).FirstOrDefault(e => e.GetElementsByTagName("small").FirstOrDefault().Text() == small)?.Text();
            if (String.IsNullOrWhiteSpace(possiblyEmpty))
                return null;
            return possiblyEmpty;
        }
    }
}
