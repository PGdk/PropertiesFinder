using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using Models;
using System.Reflection.Metadata;

namespace SampleApp
{
    public class MyParser
    {
        public MyParser(List<DumpAdresowoPL> myDumpAdresowoPL)
        {
            var url = "https://adresowo.pl";
            //Lista z offertami Tylko pierwszej strony
            List<string> linksToOffersList = new List<string>();
            //Lista z offertami wraz z podstronami
            List<string> linksToOffersListWithSubPages = new List<string>();

            GetLinksToAllProvinces(linksToOffersList, url);

            foreach (var item in linksToOffersList)
            {
                GetLinksToAllProvincesWithSubPages(linksToOffersListWithSubPages, item);
            }
            //Lista linkow do konkretnych ofert
            List<string> linksToOffers = new List<string>();

            foreach (var item in linksToOffersList)
            {
                GetLinksToOffers(linksToOffers, item);
            }

            foreach (var item in linksToOffers)
            {
                try
                {
                    GetDataFromOffert(item, myDumpAdresowoPL);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine();
                }
            }
        }

        //dla IntegrationApi
        public MyParser(List<DumpAdresowoPL> myDumpAdresowoPLpageNumber, int pageNumber)
        {
            var url = "https://adresowo.pl";
            //Lista z offertami Tylko pierwszej strony
            List<string> linksToOffersList = new List<string>();
            //Lista z offertami wraz z podstronami
            List<string> linksToOffersListWithSubPages = new List<string>();

            GetLinksToAllProvinces(linksToOffersList, url);

            foreach (var item in linksToOffersList)
            {
                GetLinksToAllProvincesWithSubPages(linksToOffersListWithSubPages, item);
            }
            //Lista linkow do konkretnych ofert
            List<string> linksToOffers = new List<string>();

            var ArraylinksToOffersListWithSubPages = linksToOffersListWithSubPages.ToArray();
            GetLinksToOffers(linksToOffers, ArraylinksToOffersListWithSubPages[pageNumber-1]);

            foreach (var item in linksToOffers)
            {
                try
                {
                    GetDataFromOffert(item, myDumpAdresowoPLpageNumber);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }


        private static HtmlDocument Connect(string url)
        {
            //adresowo.pl jest kodowana w iso-8859-2
            //AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false);
            string pageContent = "";
            using (HttpClient httpClient = new HttpClient())
            {
                using (var html = httpClient.GetAsync(url).Result)
                {
                    var byteArray = html.Content.ReadAsByteArrayAsync().Result;
                    pageContent = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
                }
            }
            var myHtmlDocument = new HtmlDocument();
            myHtmlDocument.LoadHtml(pageContent);

            return myHtmlDocument;
        }

        private static void GetLinksToAllProvinces(List<string> linksToOffersList, string url)
        {
            var myHtmlDocument = Connect(url);

            var ParselinksToOffers = myHtmlDocument.DocumentNode.SelectNodes("//article[@class='home-links group']/ul/li");

            foreach (var linkToOffers in ParselinksToOffers)
            {
                //pierwszy link z wszystkimi ofertami z wojewodztwa
                var shortUrl = linkToOffers.Descendants("a").FirstOrDefault().GetAttributeValue("href", "");
                if (shortUrl.Contains("dzialki") == false) //pomijamy ogloszenia samych dzialek
                {
                    var newUrl = url + shortUrl;
                    linksToOffersList.Add(newUrl);
                }
            }
        }

        private static void GetLinksToAllProvincesWithSubPages(List<string> linksToOffersListWithSubPages, string url)
        {
            var myHtmlDocument = Connect(url);

            var numberOfSubPages = myHtmlDocument.DocumentNode.SelectSingleNode("//span[@class='search-pagination__number-of-pages']").InnerText;

            linksToOffersListWithSubPages.Add(url); //Dodaj pierwsza strone

            //Odczytanie ilosci podstron i dodanie do listy.
            for (int i = 2; i <= int.Parse(numberOfSubPages); i++)
            {
                var newUrl = url + "_l" + i;
                linksToOffersListWithSubPages.Add(newUrl);
            }
        }

        private static void GetLinksToOffers(List<string> linksToOffers, string url)
        {
            var myHtmlDocument = Connect(url);

            var ParselinksToOffers = myHtmlDocument.DocumentNode.SelectNodes("//div[@class='result-info']");

            foreach (var linkToOffers in ParselinksToOffers)
            {
                //pierwszy link z wszystkimi ofertami z wojewodztwa
                var shortUrl = linkToOffers.Descendants("a").FirstOrDefault().GetAttributeValue("href", "");
                if (shortUrl.Contains("dzialki") == false) //pomijamy ogloszenia samych dzialek
                {
                    var newUrl = "https://adresowo.pl" + shortUrl;
                    linksToOffers.Add(newUrl);
                }
            }
        }

        private static void GetDataFromOffert(string url, List<DumpAdresowoPL> myDumpAdresowoPL)
        {
            var myHtmlDocument = Connect(url);

            var offerDetails = myHtmlDocument.DocumentNode.SelectNodes("//span[@class='offer-summary__value']");

            //Maksymalnie 5 szczegolowych informacji
            ArrayList offerDetailsArray = new ArrayList();

            foreach (var item in offerDetails)
            {
                offerDetailsArray.Add(item.InnerText);
            }

            //Maksymalnie 5 szczegolowych informacji. Dwie ostatnie sa dodatkowe. W takim przypadku dodajemy null
            while (offerDetailsArray.Count < 5)
            {
                offerDetailsArray.Add(null);
            }

            decimal TotalGrossPrice = Convert.ToDecimal(offerDetailsArray[0]);
            decimal Area = Convert.ToDecimal(offerDetailsArray[1]);
            decimal PricePerMeter = Convert.ToDecimal(offerDetailsArray[2]);
            var tempFloorNumberOrGardenArea = offerDetailsArray[3];
            decimal? GardenArea = null;
            int? FloorNumber = null;

            //czasem pojawiaja sie zbedne informacje np "10 pietro / winda"
            if (offerDetailsArray[3] != null && offerDetailsArray[3].ToString().Contains(" "))
            {
                tempFloorNumberOrGardenArea = offerDetailsArray[3].ToString().Substring(0, offerDetailsArray[3].ToString().IndexOf(" ") + 1);
            }
            //W tym samym polu co pietro, pojawia sie powierzchnia dzialki, w przypadku domow
            var checkIfGardenArea = myHtmlDocument.DocumentNode.SelectSingleNode("//div[@class='offer-summary__item offer-summary__item2']/div[@class='offer-summary__second-row']").InnerText.Contains("Pow.");
            if (offerDetailsArray[3] != null && checkIfGardenArea)
            {
                GardenArea = Convert.ToDecimal(tempFloorNumberOrGardenArea);
            }
            if (GardenArea == null)
            {
                FloorNumber = Convert.ToInt32(tempFloorNumberOrGardenArea);
            }
            int? YearOfConstruction = Convert.ToInt32(offerDetailsArray[4]);

            string StreetName = myHtmlDocument.DocumentNode.SelectSingleNode("//span[@class='offer-header__street']").InnerText;
            string District = myHtmlDocument.DocumentNode.SelectSingleNode("//span[@class='offer-header__city']").InnerText;
            string City = myHtmlDocument.DocumentNode.SelectSingleNode("//span[@class='offer-header__city']").InnerText.ToUpper();

            string LastWordOfStreetName = "";
            if (StreetName.Any(char.IsDigit)) LastWordOfStreetName = StreetName.Substring(StreetName.LastIndexOf(" ", StreetName.Length));
            //Sprawdzamy czy w podanym adresie na stronie jest numer domu.
            int DetailedAddress = 0;
            var isNumeric = int.TryParse(LastWordOfStreetName, out int n);
            if (isNumeric) DetailedAddress = int.Parse(LastWordOfStreetName);

            string headerInfo = myHtmlDocument.DocumentNode.SelectSingleNode("//span[@class='offer-header__info']").InnerText;
            string tempNumberOfRooms = "";
            int NumberOfRooms = 0;
            if (headerInfo.Contains("-pokojowe"))
            {
                tempNumberOfRooms = headerInfo.Substring(0, headerInfo.IndexOf("-pokojowe"));
                NumberOfRooms = int.Parse(tempNumberOfRooms);
            }

            int? Balconies = null;

#pragma warning disable CS0184 // 'is' expression's given expression is never of the provided type
            if (myHtmlDocument.DocumentNode.SelectSingleNode("//ul[@class='offer-description__summary']") is NotNullAttribute)
#pragma warning restore CS0184 // 'is' expression's given expression is never of the provided type

            {
                string summaryInfo = myHtmlDocument.DocumentNode.SelectSingleNode("//ul[@class='offer-description__summary']").InnerText;
                if (summaryInfo.Contains("balkon") || summaryInfo.Contains("Balkon")) Balconies = 1;
            }

            string RawDescriptionHTML = myHtmlDocument.DocumentNode.SelectSingleNode("//div[@class='offer-description']").InnerText;
            //RawDescription = Regex.Replace(RawDescription, @"( |\t|\r?\n)\1+", "$1");
            string RawDescription = RawDescriptionHTML.Replace("\n", "").Replace("\r", " ");
            while (RawDescription.Contains("  ")) RawDescription = RawDescription.Replace("  ", " ");

            decimal? BasementArea = null;
            int? OutdoorParkingPlaces = null;
            int? IndoorParkingPlaces = null;
            string urlOffer = url;
            AddNewOfferToDump(myDumpAdresowoPL, City, District, StreetName, DetailedAddress, NumberOfRooms, Balconies, TotalGrossPrice, Area, PricePerMeter, FloorNumber, GardenArea, YearOfConstruction, BasementArea, OutdoorParkingPlaces, IndoorParkingPlaces, RawDescription, urlOffer);
        }

        private static void AddNewOfferToDump(List<DumpAdresowoPL> myDumpAdresowoPL, string city, string district, string streetName, int detailedAddress, int numberOfRooms, int? balconies, decimal totalGrossPrice, decimal area, decimal pricePerMeter, int? floorNumber, decimal? gardenArea, int? yearOfConstruction, decimal? basementArea, int? outdoorParkingPlaces, int? indoorParkingPlaces, string rawDescription, string urlOffer)
        {
            DumpAdresowoPL newOffer = new DumpAdresowoPL(city, district, streetName, detailedAddress, numberOfRooms, balconies, totalGrossPrice, area, pricePerMeter, floorNumber, gardenArea, yearOfConstruction, basementArea, outdoorParkingPlaces, indoorParkingPlaces, rawDescription, urlOffer);
            myDumpAdresowoPL.Add(newOffer);
        }
    }
}