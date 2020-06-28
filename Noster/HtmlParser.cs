using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Models;
using System.Text.RegularExpressions;
using System.Net;

namespace Application
{
    public static class HtmlParser
    {
        public static OfferKind GetOfferKind(HtmlDocument htmlDocument)
        {
            string offerKind = htmlDocument.DocumentNode.Descendants("div")
                .Where(n => n.GetAttributeValue("class", "").Equals("pageHeader")).FirstOrDefault().Descendants("span")
                .Where(n => n.GetAttributeValue("class", "").Equals("small")).FirstOrDefault().InnerHtml;

            if (offerKind.Contains("wynajem"))
                return OfferKind.RENTAL;
            return OfferKind.SALE;
        }

        public static SellerContact GetSellecContact(HtmlDocument htmlDocument)
        {
            return new SellerContact
            {
                Email = GetSellerEmail(htmlDocument),
                Telephone = GetSellerTelephone(htmlDocument),
                Name = GetSellerName(htmlDocument)
            };
        }

        private static string GetSellerEmail(HtmlDocument htmlDocument)
        {
            return htmlDocument.DocumentNode.Descendants("div")
                .Where(n => n.GetAttributeValue("class", "").Equals("globalBoxContent")).FirstOrDefault().Descendants("a")
                .Where(n => n.GetAttributeValue("class", "").Equals("agentMail smallEmail")).FirstOrDefault().InnerHtml;
        }
        private static string GetSellerTelephone(HtmlDocument htmlDocument)
        {
            return htmlDocument.DocumentNode.Descendants("div")
                .Where(n => n.GetAttributeValue("class", "").Equals("globalBoxContent")).FirstOrDefault().Descendants("a")
                .Where(n => n.GetAttributeValue("class", "").Equals("agentMobile")).FirstOrDefault().InnerHtml;
        }
        private static string GetSellerName(HtmlDocument htmlDocument)
        {
            return htmlDocument.DocumentNode.Descendants("div")
                .Where(n => n.GetAttributeValue("class", "").Equals("globalBoxContent")).FirstOrDefault().Descendants("div")
                .Where(n => n.GetAttributeValue("class", "").Equals("agentName")).FirstOrDefault().InnerHtml;
        }

        #region PropertyPrice
        public static decimal GetTotalGrossPrice(HtmlDocument htmlDocument)
        {
            string price = htmlDocument.DocumentNode.Descendants("span")
                .Where(n => n.GetAttributeValue("class", "").Equals("offerPrice")).FirstOrDefault().Descendants("span")
                .Where(n => n.GetAttributeValue("class", "").Equals("big")).FirstOrDefault().InnerHtml;

            if (ParseToDecimal(price) == null)
                return 0;
            return ParseToDecimal(price).Value;
        }
        public static decimal GetPricePerMeter(HtmlDocument htmlDocument)
        {
            string price = htmlDocument.DocumentNode.Descendants("span")
                .Where(n => n.GetAttributeValue("class", "").Equals("offerSquarePrice")).FirstOrDefault().InnerHtml;

            if (ParseToDecimal(price) == null)
                return 0;
            return ParseToDecimal(price).Value;
        }
        public static decimal? GetResidentalRent(HtmlDocument htmlDocument)
        {
            if (GetOfferKind(htmlDocument) == OfferKind.SALE)
                return null;

            string mainPrice = htmlDocument.DocumentNode.Descendants("span")
                .Where(n => n.GetAttributeValue("class", "").Equals("offerPrice")).FirstOrDefault().Descendants("span")
                .Where(n => n.GetAttributeValue("class", "").Equals("big")).FirstOrDefault().InnerHtml;

            string rentPrice = htmlDocument.DocumentNode.Descendants("div")
                .Where(n => n.GetAttributeValue("class", "").Equals("propsRow vir_oferta_czynszletni")).FirstOrDefault()?.Descendants("span")
                .Where(n => n.GetAttributeValue("class", "").Equals("propValue")).FirstOrDefault().InnerHtml;

            if (rentPrice == null)
                return ParseToDecimal(mainPrice);

            return ParseToDecimal(mainPrice) + ParseToDecimal(rentPrice);
        }
        #endregion

        #region PropertyDetails

        public static decimal GetArea(HtmlDocument htmlDocument)
        {
            string area;
            if (IsItHouse(htmlDocument))
                area = htmlDocument.DocumentNode.Descendants("div")
                        .Where(n => n.GetAttributeValue("class", "").Equals("propsRow vir_oferta_powierzchniauzytkowa")).FirstOrDefault()?.Descendants("span")
                        .Where(n => n.GetAttributeValue("class", "").Equals("propValue")).FirstOrDefault().InnerText;
            else
                area = htmlDocument.DocumentNode.Descendants("div")
                    .Where(n => n.GetAttributeValue("class", "").Equals("propsRow vir_oferta_powierzchnia")).FirstOrDefault().Descendants("span")
                    .Where(n => n.GetAttributeValue("class", "").Equals("propValue")).FirstOrDefault().InnerHtml;

            if (area == null)
                area = htmlDocument.DocumentNode.Descendants("div")
                    .Where(n => n.GetAttributeValue("class", "").Equals("propsRow vir_oferta_powierzchnia")).FirstOrDefault().Descendants("span")
                    .Where(n => n.GetAttributeValue("class", "").Equals("propValue")).FirstOrDefault().InnerHtml;

            if (ParseToInt(area) == null)
                return 0;
            return ParseToDecimal(area).Value;
        }
        public static int GetNumberOfRooms(HtmlDocument htmlDocument)
        {
            string numberOfRooms = htmlDocument.DocumentNode.Descendants("div")
                    .Where(n => n.GetAttributeValue("class", "").Equals("propsRow vir_oferta_iloscpokoi")).FirstOrDefault()?.Descendants("span")
                    .Where(n => n.GetAttributeValue("class", "").Equals("propValue")).FirstOrDefault().InnerHtml;

            if (numberOfRooms == null)
                return FindNumberPrecedingThePhraseInDescription(GetRawDescription(htmlDocument), "pokoj");

            if (ParseToInt(numberOfRooms) == null)
                return 0;
            return ParseToInt(numberOfRooms).Value;
        }
        public static int? GetFloorNumber(HtmlDocument htmlDocument)
        {
            if (IsItHouse(htmlDocument))
                return null;
            string floorNumber = htmlDocument.DocumentNode.Descendants("div")
                    .Where(n => n.GetAttributeValue("class", "").Equals("propsRow vir_oferta_pietro")).FirstOrDefault()?.Descendants("span")
                    .Where(n => n.GetAttributeValue("class", "").Equals("propValue")).FirstOrDefault().InnerHtml;
            if (floorNumber == null)
                return null;
            if (floorNumber.Contains("arter"))
                return 0;
            if (floorNumber.Contains("oddasze"))
                floorNumber = htmlDocument.DocumentNode.Descendants("div")
                    .Where(n => n.GetAttributeValue("class", "").Equals("propsRow vir_oferta_iloscpieter")).FirstOrDefault()?.Descendants("span")
                    .Where(n => n.GetAttributeValue("class", "").Equals("propValue")).FirstOrDefault().InnerHtml;
            if (floorNumber == null)
                return null;

            return ParseToInt(floorNumber);
        }
        public static int? GetYearOfConstruction(HtmlDocument htmlDocument)
        {
            string yearOfConstruction = htmlDocument.DocumentNode.Descendants("div")
                    .Where(n => n.GetAttributeValue("class", "").Equals("propsRow vir_oferta_rokbudowy")).FirstOrDefault()?.Descendants("span")
                    .Where(n => n.GetAttributeValue("class", "").Equals("propValue")).FirstOrDefault().InnerHtml;
            if (yearOfConstruction == null)
                return null;

            return ParseToInt(yearOfConstruction);
        }
        #endregion

        #region PropertyAddress
        public static PolishCity GetCity(HtmlDocument htmlDocument)
        {
            string city = htmlDocument.DocumentNode.Descendants("div")
                    .Where(n => n.GetAttributeValue("class", "").Equals("pageHeader")).FirstOrDefault().Descendants("span")
                    .Where(n => n.GetAttributeValue("class", "").Equals("location")).FirstOrDefault().InnerHtml;
            city = Regex.Replace(city, ",", "");
            city = city.ToUpper();

            var polishCities = Enum.GetValues(typeof(PolishCity)).Cast<PolishCity>();
            foreach (var polishCity in polishCities)
            {
                if (polishCity.ToString().Contains(city))
                    return polishCity;
            }

            //Problem z wioskami, np.: Nadarzyn, Lesznowola... 
            return PolishCity.WARSZAWA;
        }
        public static string GetDistrict(HtmlDocument htmlDocument)
        {
            string district = htmlDocument.DocumentNode.Descendants("div")
                    .Where(n => n.GetAttributeValue("class", "").Equals("pageHeader")).FirstOrDefault().Descendants("span")
                    .Where(n => n.GetAttributeValue("class", "").Equals("quarter")).FirstOrDefault()?.InnerHtml;

            if (district == null)
                return "brak";
            return district;
        }
        public static string GetStreetName(HtmlDocument htmlDocument)
        {
            string streetName = htmlDocument.DocumentNode.Descendants("div")
                    .Where(n => n.GetAttributeValue("class", "").Equals("pageHeader")).FirstOrDefault().Descendants("span")
                    .Where(n => n.GetAttributeValue("class", "").Equals("street")).FirstOrDefault()?.InnerHtml;

            if (streetName == null)
                return "brak";
            return streetName;
        }
        public static string GetDetailedAddress(HtmlDocument htmlDocument)
        {
            // Kontakt tylko przez formularz na stronie
            return "brak";
        }
        #endregion

        #region PropertyFeatures
        public static decimal? GetGardenArea(HtmlDocument htmlDocument)
        {
            return FindNumberFollowingThePhraseInDescription(GetRawDescription(htmlDocument), "ogród");
        }
        public static int? GetBalconies(HtmlDocument htmlDocument)
        {
            string balconies = htmlDocument.DocumentNode.Descendants("div")
                    .Where(n => n.GetAttributeValue("class", "").Equals("propsRow vir_oferta_iloscbalkonow")).FirstOrDefault()?.Descendants("span")
                    .Where(n => n.GetAttributeValue("class", "").Equals("propValue")).FirstOrDefault().InnerHtml;

            if (balconies == null)
                return null;
            return ParseToInt(balconies);
        }
        public static decimal? GetBasementArea(HtmlDocument htmlDocument)
        {
            return FindNumberFollowingThePhraseInDescription(GetRawDescription(htmlDocument), "piwni");
        }
        public static int? GetOutdoorParkingPlaces(HtmlDocument htmlDocument)
        {
            return null;
        }
        public static int? GetIndoorParkingPlaces(HtmlDocument htmlDocument)
        {
            return null;
        }
        #endregion

        public static string GetRawDescription(HtmlDocument htmlDocument)
        {
            return htmlDocument.DocumentNode.Descendants("div")
                .Where(n => n.GetAttributeValue("id", "").Equals("offerDescription")).FirstOrDefault().InnerHtml;
        }

        public static List<string> GetLinksToOffersFromUrl(string url)
        {
            var htmlDocument = new HtmlDocument();
            var client = new WebClient();
            string html = client.DownloadString(url);
            htmlDocument.LoadHtml(html);

            List<string> links = new List<string>();

            var currentOffer = htmlDocument.DocumentNode.Descendants("div")
                .Where(n => n.GetAttributeValue("class", "").Equals("offersListHolder")).FirstOrDefault().FirstChild.NextSibling;
            while (currentOffer != null)
            {
                links.Add("https://www.noster-nieruchomosci.pl/" + currentOffer.Descendants("a").FirstOrDefault().ChildAttributes("href").FirstOrDefault().Value.ToString());
                currentOffer = currentOffer.NextSibling.NextSibling;
            }
            return links;
        }

        public static int GetNumberOfPages()
        {
            string url = "https://www.noster-nieruchomosci.pl/oferty";
            var htmlDocument = new HtmlDocument();
            var client = new WebClient();
            var html = client.DownloadString(url);
            htmlDocument.LoadHtml(html);

            var numberOfPages = htmlDocument.DocumentNode.Descendants("div")
                .Where(n => n.GetAttributeValue("class", "").Equals("pagingHolder")).FirstOrDefault().Descendants("ul")
                .Where(n => n.GetAttributeValue("class", "").Equals("paging")).FirstOrDefault().LastChild.PreviousSibling.InnerText;

            return int.Parse(numberOfPages);
        }

        private static bool IsItHouse(HtmlDocument htmlDocument)
        {
            // true dla domu, false dla mieszkania
            string offerKind = htmlDocument.DocumentNode.Descendants("div")
                .Where(n => n.GetAttributeValue("class", "").Equals("pageHeader")).FirstOrDefault().Descendants("span")
                .Where(n => n.GetAttributeValue("class", "").Equals("small")).FirstOrDefault().InnerHtml;

            if (offerKind.Contains("Dom"))
                return true;
            return false;
        }
        private static decimal? ParseToDecimal(string phrase)
        {
            string value = phrase;
            value = Regex.Replace(value, "m2", "");
            value = Regex.Replace(value, "m&sup2", "");
            value = Regex.Replace(value, "m<sup>2", "");
            value = Regex.Replace(value, "[.]", ",");
            value = Regex.Replace(value, "[^0-9,]", "");
            try
            {
                return decimal.Parse(value);
            }
            catch (FormatException)
            {
                return null;
            }
        }
        private static int? ParseToInt(string phrase)
        {
            string value = phrase;
            value = Regex.Replace(value, "m2", "");
            value = Regex.Replace(value, "m&sup2", "");
            value = Regex.Replace(value, "m<sup>2", "");
            value = Regex.Replace(value, "[.]", ",");
            value = Regex.Replace(value, "[^0-9,]", "");
            try
            {
                double f = double.Parse(value);
                return (int)f;
            }
            catch (FormatException)
            {
                return null;
            }
        }

        private static int FindNumberPrecedingThePhraseInDescription(string rawDescription, string phrase)
        {
            rawDescription = rawDescription.ToLower();
            if (!rawDescription.Contains(phrase))
                return 0;

            // Ucinamy wszystko od szukanej frazy
            int indexOfFirstLetter = rawDescription.IndexOf(phrase);
            rawDescription = rawDescription.Remove(indexOfFirstLetter + phrase.Length);

            // Ucinamy wszystkie zdania do tego, które zawiera podaną frazę
            int indexOfDot = rawDescription.IndexOf(".");
            while (indexOfDot < indexOfFirstLetter && indexOfDot != -1)
            {
                rawDescription = rawDescription.Remove(0, indexOfDot + 1);
                indexOfDot = rawDescription.IndexOf(".");
                indexOfFirstLetter = rawDescription.IndexOf(phrase);
            }

            // Zamieniamy pozostały string na liczbę
            int? result = ParseToInt(rawDescription);
            if (result == null)
                return 0;
            return result.Value;
        }

        private static int? FindNumberFollowingThePhraseInDescription(string rawDescription, string phrase)
        {
            rawDescription = rawDescription.ToLower();
            if (!rawDescription.Contains(phrase))
                return null;

            // Ucinamy wszystko do szukanej frazy
            int indexOfFirstLetter = rawDescription.IndexOf(phrase);
            rawDescription = rawDescription.Remove(0, indexOfFirstLetter);

            // Ucinamy wszystko od pierwszej kropki LUB od 22 znaku
            // Zakładam, że nie ma sensu szukać liczby (np. balkonów) dalej
            // Przykład z opisu: "Ogród o wielkości 1000m2"
            indexOfFirstLetter = rawDescription.IndexOf(".");
            if (indexOfFirstLetter == -1 || indexOfFirstLetter > 22)
                indexOfFirstLetter = 22;
            rawDescription = rawDescription.Remove(indexOfFirstLetter);

            // Zamieniamy pozostały string na liczbę
            return ParseToInt(rawDescription);
        }
    }
}