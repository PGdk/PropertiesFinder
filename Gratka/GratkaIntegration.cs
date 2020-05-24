using HtmlAgilityPack;
using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Application.Gratka
{
    public class GratkaIntegration : IWebSiteIntegration
    {
        public WebPage WebPage { get; }

        private HtmlWeb HtmlWeb = new HtmlWeb();

        private CultureInfo CultureInfo = new CultureInfo("pl-PL");

        private List<Entry> Entries = new List<Entry>();

        public IDumpsRepository DumpsRepository { get; }

        public IEqualityComparer<Entry> EntriesComparer { get; }

        public GratkaIntegration(IDumpsRepository dumpsRepository,
            IEqualityComparer<Entry> equalityComparer)
        {
            DumpsRepository = dumpsRepository;
            EntriesComparer = equalityComparer;
            WebPage = new WebPage
            {
                Url = "https://gratka.pl/nieruchomosci",
                Name = "Gratka.pl",
                WebPageFeatures = new WebPageFeatures
                {
                    HomeSale = true,
                    HomeRental = true,
                    HouseSale = true,
                    HouseRental = true
                }
            };
        }

        public Dump GenerateDump()
        {
            const int MAX_NUMBER_OF_PAGES = 2;
            GenerateAllEntries(MAX_NUMBER_OF_PAGES);

            return new Dump
            {
                DateTime = DateTime.Now,
                WebPage = WebPage,
                Entries = Entries
            };
        }

        private void GenerateAllEntries(int maxNumberOfPages)
        {
            for (int i = 1; i < maxNumberOfPages + 1; ++i)
            {
                GenerateEntriesFromPage(i);
            }
        }

        private void GenerateEntriesFromPage(int pageNumber)
        {
            var fullUrl = WebPage.Url + (pageNumber == 1 ? "" : "?page=" + pageNumber.ToString());
            var page = LoadUrl(fullUrl);
            if (page == null)
            {
                return;
            }

            var teaserNodes = GetHtmlNodesByClass(page, "teaserEstate", "article");
            foreach (var teaserNode in teaserNodes)
            {
                if (teaserNode != null)
                {
                    GenerateEntry(teaserNode);
                }
            }
        }

        private void GenerateEntry(HtmlNode teaserNode)
        {
            var city = GetCityFromTeaserNode(teaserNode);
            if (city == null)
            {
                return;
            }

            var url = GetUrlFromTeaserNode(teaserNode);
            var page = LoadUrl(url);
            if (page == null)
            {
                return;
            }

            var district = GetDistrictFromTeaserNode(teaserNode);
            var area = GetAreaFromTeaserNode(teaserNode);
            var lastUpdateDateTime = GetLastUpdateDateTimeFromTeaserNode(teaserNode);
            var totalGrossPrice = GetTotalGrossPriceFromTeaserNode(teaserNode);

            Entries.Add(new Entry
            {
                OfferDetails = GetOfferDetails(page, url, lastUpdateDateTime),
                PropertyPrice = GetPropertyPrice(page, totalGrossPrice, area),
                PropertyDetails = GetPropertyDetails(page, area),
                PropertyAddress = GetPropertyAddress(page, city, district),
                PropertyFeatures = GetPropertyFeatures(page),
                RawDescription = GetRawDescription(page)
            });
        }

        private PolishCity? GetCityFromTeaserNode(HtmlNode teaserNode)
        {
            string city = default;
            try
            {
                city = GetHtmlNodesByClass(teaserNode, "teaserEstate__localization", "span")[0].FirstChild.InnerText;
                city = Regex.Replace(city, @"\s+", "").ToUpper();
                city = RemovePolishCharacters(city).Replace(' ', '_');
                city = city.Split(',')[0];
            }
            catch (Exception)
            {
                return null;
            }

            PolishCity polishCity;
            if (PolishCity.TryParse(city, out polishCity))
            {
                return polishCity;
            }
            else
            {
                return null;
            }
        }

        private string GetDistrictFromTeaserNode(HtmlNode teaserNode)
        {
            string district = default;
            try
            {
                district = GetHtmlNodesByClass(teaserNode, "teaserEstate__localization", "span")[0].FirstChild.InnerText;
                district = Regex.Replace(district, @"\s+", "");
                district = district.Split(',')[1];
                return district;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private string RemovePolishCharacters(string text)
        {
            return text
                .Replace('Ą', 'A')
                .Replace('Ć', 'C')
                .Replace('Ę', 'E')
                .Replace('Ł', 'L')
                .Replace('Ń', 'N')
                .Replace('Ó', 'O')
                .Replace('Ś', 'S')
                .Replace('Ź', 'Z')
                .Replace('Ż', 'Z');
        }

        private string GetUrlFromTeaserNode(HtmlNode teaserNode)
        {
            string url = default;
            try
            {
                url = teaserNode.GetAttributeValue("data-href", "");
                return url;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private decimal? GetAreaFromTeaserNode(HtmlNode teaserNode)
        {
            string areaString = default;
            decimal? area = default;
            try
            {
                areaString = GetHtmlNodesByXPath(GetHtmlNodesByClass(teaserNode, "teaserEstate__offerParams", "ul")[0], "li")[0].InnerText;
                area = ParseDecimalFromString(areaString.Trim().Split('m')[0]);
                return area;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private DateTime? GetLastUpdateDateTimeFromTeaserNode(HtmlNode teaserNode)
        {
            string lastUpdateDateTimeString = default;
            DateTime? lastUpdateDateTime = default;
            try
            {
                lastUpdateDateTimeString = GetHtmlNodesByXPath(GetHtmlNodesByClass(teaserNode, "teaserEstate__details", "ul")[0], "li")[0].InnerText;
                lastUpdateDateTime = ParseDateTimeFromString(lastUpdateDateTimeString.Trim().Split(':')[1]);
                return lastUpdateDateTime;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private decimal? GetTotalGrossPriceFromTeaserNode(HtmlNode teaserNode)
        {
            string totalGrossPriceString = default;
            decimal? totalGrossPrice = default;
            try
            {
                totalGrossPriceString = GetHtmlNodesByClass(teaserNode, "teaserEstate__price", "p")[0].InnerText;
                totalGrossPrice = ParseDecimalFromString(totalGrossPriceString.Trim().Split('z')[0]);
                return totalGrossPrice;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string GetParameter(HtmlDocument page, string text)
        {
            try
            {
                var parametersContainer = GetHtmlNodesByClass(page, "parameters__container", "div")[0];
                var node = GetHtmlNodesByText(parametersContainer, text, "li")[0];
                var parameter = GetHtmlNodesByClass(node, "parameters__value", "b")[0].InnerText;
                if (parameter != null)
                {
                    return parameter;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        private OfferDetails GetOfferDetails(HtmlDocument page, string url, DateTime? lastUpdateDateTime)
        {
            var offerDetails = new OfferDetails();
            offerDetails.Url = url;
            if (lastUpdateDateTime != null)
            {
                offerDetails.CreationDateTime = (DateTime)lastUpdateDateTime; // NO INFO
                offerDetails.LastUpdateDateTime = lastUpdateDateTime;
            }
            try
            {
                offerDetails.OfferKind = GetRawDescription(page).ToUpper().Contains("WYNAJ") ? OfferKind.RENTAL : OfferKind.SALE;
            }
            catch (Exception)
            { }
            offerDetails.IsStillValid = true; // NO INFO

            var sellerContact = new SellerContact();
            try
            {
                sellerContact.Telephone = GetHtmlNodesByClass(page, "phoneSmallButton__button", "a")[0].GetAttributeValue("data-full-phone-number", "");
            }
            catch (Exception)
            { }
            try
            {
                sellerContact.Name = GetHtmlNodesByClass(page, "offerOwner__person", "h3")[0].InnerText;
            }
            catch (Exception)
            { }
            offerDetails.SellerContact = sellerContact;

            return offerDetails;
        }

        private PropertyPrice GetPropertyPrice(HtmlDocument page, decimal? totalGrossPrice, decimal? area)
        {
            var propertyPrice = new PropertyPrice();
            if (totalGrossPrice != null)
            {
                propertyPrice.TotalGrossPrice = (decimal)totalGrossPrice;
            }
            if (totalGrossPrice != null && area != null)
            {
                try
                {
                    propertyPrice.PricePerMeter = (decimal)totalGrossPrice / (decimal)area;
                }
                catch (Exception)
                { }
            }
            try
            {
                var residentalRent = ParseDecimalFromString(GetParameter(page, "Opłaty (czynsz administracyjny, media)").Split('z')[0]);
                if (residentalRent != null)
                {
                    propertyPrice.ResidentalRent = (decimal)residentalRent;
                }
            }
            catch (Exception)
            { }

            return propertyPrice;
        }

        private PropertyDetails GetPropertyDetails(HtmlDocument page, decimal? area)
        {
            var propertyDetails = new PropertyDetails();
            if (area != null)
            {
                propertyDetails.Area = (decimal)area;
            }
            var numberOfRooms = ParseIntFromString(GetParameter(page, "Liczba pokoi"));
            if (numberOfRooms != null)
            {
                propertyDetails.NumberOfRooms = (int)numberOfRooms;
            }
            var floorNumber = ParseIntFromString(GetParameter(page, "Piętro"));
            if (floorNumber != null)
            {
                propertyDetails.FloorNumber = (int)floorNumber;
            }
            var yearOfConstruction = ParseIntFromString(GetParameter(page, "Rok budowy"));
            if (yearOfConstruction != null)
            {
                propertyDetails.YearOfConstruction = (int)yearOfConstruction;
            }

            return propertyDetails;
        }

        private PropertyAddress GetPropertyAddress(HtmlDocument page, PolishCity? city, string district)
        {
            var propertyAddress = new PropertyAddress();
            if (city != null)
            {
                propertyAddress.City = (PolishCity)city;
            }
            if (district != "")
            {
                propertyAddress.District = district;
            }
            // propertyAddress.StreetName = ""; // NO INFO
            // propertyAddress.DetailedAddress = ""; // NO INFO

            return propertyAddress;
        }

        private PropertyFeatures GetPropertyFeatures(HtmlDocument page)
        {
            var propertyFeatures = new PropertyFeatures();
            // propertyFeatures.GardenArea = 0; // NO INFO
            var balconies = GetParameter(page, "Powierzchnia dodatkowa");
            if (balconies.Contains("balkon"))
            {
                propertyFeatures.Balconies = 1;
            }
            // propertyFeatures.BasementArea = 1; // NO INFO
            var parkingPlaces = GetParameter(page, "Miejsce parkingowe");
            if (parkingPlaces.Contains("przynależne na ulicy"))
            {
                propertyFeatures.OutdoorParkingPlaces = 1;
            }
            if (parkingPlaces.Contains("w garażu"))
            {
                propertyFeatures.IndoorParkingPlaces = 1;
            }
            if (parkingPlaces.Contains("brak miejsca parkingowego"))
            {
                propertyFeatures.OutdoorParkingPlaces = 0;
                propertyFeatures.IndoorParkingPlaces = 0;
            }

            return propertyFeatures;
        }

        private string GetRawDescription(HtmlDocument page)
        {
            try
            {
                string rawDescription = GetHtmlNodesByClass(page, "description__rolled", "div")[0].InnerText;
                return rawDescription;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private HtmlDocument LoadUrl(string fullUrl)
        {
            try
            {
                var page = HtmlWeb.Load(fullUrl);
                return page;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private int? ParseIntFromString(string text)
        {
            int number;
            if (Int32.TryParse(text.Trim(), NumberStyles.Any, CultureInfo, out number))
            {
                return number;
            }
            else
            {
                return null;
            }
        }

        private decimal? ParseDecimalFromString(string text)
        {
            decimal number;
            if (Decimal.TryParse(text.Trim(), NumberStyles.Any, CultureInfo, out number))
            {
                return number;
            }
            else
            {
                return null;
            }
        }

        private DateTime? ParseDateTimeFromString(string text)
        {
            DateTime dateTime;
            if (DateTime.TryParse(text.Trim(), CultureInfo, DateTimeStyles.NoCurrentDateDefault, out dateTime))
            {
                return dateTime;
            }
            else
            {
                return null;
            }
        }

        private List<HtmlNode> GetHtmlNodesByXPath(HtmlNode node, string xPath)
        {
            try
            {
                var nodes = node.SelectNodes(xPath).ToList();
                return nodes;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private List<HtmlNode> GetHtmlNodesByXPath(HtmlDocument page, string xPath)
        {
            return GetHtmlNodesByXPath(page.DocumentNode, xPath);
        }

        private List<HtmlNode> GetHtmlNodesByText(HtmlNode node, string text, string htmlElement = "*")
        {
            return GetHtmlNodesByXPath(node, ".//" + htmlElement + "[normalize-space() = '" + text + "']");
        }

        private List<HtmlNode> GetHtmlNodesByText(HtmlDocument page, string text, string htmlElement = "*")
        {
            return GetHtmlNodesByText(page.DocumentNode, text, htmlElement);
        }

        private List<HtmlNode> GetHtmlNodesByClass(HtmlNode node, string className, string htmlElement = "*")
        {
            return GetHtmlNodesByXPath(node, ".//" + htmlElement + "[contains(@class, '" + className + "')]");
        }

        private List<HtmlNode> GetHtmlNodesByClass(HtmlDocument page, string className, string htmlElement = "*")
        {
            return GetHtmlNodesByClass(page.DocumentNode, className, htmlElement);
        }
    }
}

