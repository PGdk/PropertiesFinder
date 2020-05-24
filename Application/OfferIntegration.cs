using HtmlAgilityPack;
using Interfaces;
using Models;
using Models.Pomorska;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Runtime.Loader;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

namespace Application
{
    class OfferIntegration : IWebSiteIntegration
    {
        public Dictionary<string, string> gratkaLinksWithDate;
        public WebPage WebPage { get; }
        public IDumpsRepository DumpsRepository { get; }
        public IEqualityComparer<Entry> EntriesComparer { get; }
        public List<PomorskaCitySite> CitiesList { get; set; } = new List<PomorskaCitySite>();
        public HtmlWeb Web = new HtmlWeb();

        public OfferIntegration(IDumpsRepository dumpsRepository,
            IEqualityComparer<Entry> equalityComparer)
        {
            CultureInfo ci = new CultureInfo("pl-PL");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            Web.OverrideEncoding = Encoding.UTF8;

            DumpsRepository = dumpsRepository;
            EntriesComparer = equalityComparer;
            WebPage = new WebPage
            {
                Url = "https://pomorska.pl/ogloszenia/76777,8435,fm,pk.html",
                Name = "Pomorska WebSite Integration",
                WebPageFeatures = new WebPageFeatures
                {
                    HomeSale = false,
                    HomeRental = false,
                    HouseSale = false,
                    HouseRental = false
                }
            };

            gratkaLinksWithDate = new Dictionary<string, string>();

            PrepareCitiesSites();
        }

        private void PrepareCitiesSites()
        {

            var htmlDoc = Web.Load(WebPage.Url);
            var cities = htmlDoc.GetElementbyId("ogloszenia-miasta").Descendants("a").Select(node => node.GetAttributeValue("href", null));

            PagesQuantityExtractor(WebPage.Url);

            foreach (var adres in cities)
            {
                if (adres != null && adres.Contains("https"))
                {
                    PagesQuantityExtractor(adres);
                }
            }
            foreach (PomorskaCitySite site in CitiesList)
            {
                getOffersLinksWithDates(site);
            }
        }

        private List<Entry> GenerateDumps()
        {
            var entries = new List<Entry>();
            foreach (PomorskaCitySite site in CitiesList)
            {
                foreach (var gratkaSite in site.GratkaUrlAndDate)
                {
                    entries.Add(GetEntryInfoFromGratka(gratkaSite));
                }
            }
            return entries;
        }

        public Dump GenerateDump()
        {
            var entries = GenerateDumps();
            return new Dump()
            {
                WebPage = WebPage,
                Entries = entries,
                DateTime = DateTime.Now
            };
        }

        #region SubPages quantity logic
        private int GetQuitityOfSubpages(HtmlDocument website)
        {
            List<HtmlNode> pagesQuantity = website.DocumentNode.Descendants("div")
                .Where(node => node.HasClass("stronicowanie")).Last().Descendants("a").ToList();
            pagesQuantity.RemoveRange(pagesQuantity.Count - 2, 2);
            return Int32.Parse(pagesQuantity.Last().InnerHtml);
        }

        private void PagesQuantityExtractor(string address)
        {
            PomorskaCitySite temp = new PomorskaCitySite();
            temp.URL = address;
            try
            {
                HtmlDocument htmlDoc = Web.Load(address);
                temp.SubPagesQuantity = GetQuitityOfSubpages(htmlDoc);
            }
            catch
            {

            }
            CitiesList.Add(temp);
        }
        #endregion

        #region Gratka Links and Dates logic
        private void getLinkAndDate(PomorskaCitySite website, string link)
        {
            HtmlDocument htmlDoc = Web.Load(link);
            IEnumerable<HtmlNode> offers = htmlDoc.GetElementbyId("lista-ogloszen").Descendants("ul").FirstOrDefault().Descendants("li");
            foreach (var offer in offers)
            {
                string gratkaLink = offer.Descendants("a").FirstOrDefault().GetAttributeValue("href", null);
                string gratkaDate = offer.Descendants("footer").FirstOrDefault().Descendants("p").LastOrDefault().ChildNodes.ToList()[1].InnerText;
                string gratkaUpdateDate = offer.Descendants("footer").FirstOrDefault().Descendants("p").LastOrDefault().ChildNodes.ToList()[3].InnerText;

                website.GratkaUrlAndDate.Add(new GratkaSite()
                {
                    URL = gratkaLink,
                    lastModDate = gratkaUpdateDate,
                    addDate = gratkaDate
                });
            }
        }

        private string CreateSubsiteLink(string link, int i)
        {
            string[] linkParts = Regex.Split(link, @"\b[\/\,]");
            string newLink = linkParts[0] + "/" + linkParts[1] +
                "/" + i + "," + linkParts[2] + "," + linkParts[3] +
                ",n," + linkParts[4] + "," + linkParts[5];
            return newLink;
        }

        private void getOffersLinksWithDates(PomorskaCitySite website)
        {
            for (var i = 1; i <= website.SubPagesQuantity; i++)
            {
                if (i == 1)
                {
                    getLinkAndDate(website, website.URL);
                }
                else
                {
                    string subpageLink = CreateSubsiteLink(website.URL, i);
                    getLinkAndDate(website, subpageLink);
                }
            }
        }
        #endregion

        #region Gratka logic

        private string PrepareOfferDetails(HtmlDocument website, string parameter)
        {
            string searchNode = $"//li[contains(., '{parameter}')]";
            string value = "";
            try
            {

                value = website.DocumentNode.
                    SelectNodes(searchNode)
                    .FirstOrDefault().ChildNodes
                    .FirstOrDefault(node => node.Name == "b").InnerText;
            }
            catch
            {
                value = "";
            }
            return value;
        }

        private string GetPhoneNumber(HtmlDocument website)
        {
            string phone = "";
            try
            {
                phone = ConvertFromUnicode(website.GetElementbyId("pokaz-numer-gora").GetAttributeValue("data-full-phone-number", null));
            }
            catch
            {
                phone = "";
            }
            return phone;

        }

        private List<string> GetPropertyLocation(HtmlDocument website)
        {
            List<string> location;
            try
            {
                location = website.DocumentNode
                    .SelectNodes("//ul[@class='presentationMap__address']")
                    .FirstOrDefault()
                    .Descendants("li")
                    .Select(node => node.InnerText)
                    .ToList();

            }
            catch
            {
                location = new List<string>();
                location.Add("");
                location.Add("");
            }
            return location;
        }

        private OfferDetails GetOfferDetails(HtmlDocument htmlDocument)
        {
            OfferKind offerKind = PrepareOfferDetails(htmlDocument, "Forma własności") == "własność" ? OfferKind.SALE : OfferKind.RENTAL;

            return new OfferDetails()
            {
                OfferKind = offerKind,
                IsStillValid = true
            };
        }

        private string GetNameOfSeller(HtmlDocument htmlDoc)
        {
            try
            {
                return htmlDoc.DocumentNode
                                .SelectNodes("//h3[@class='offerOwner__person']").ToList()[0].InnerText;
            }
            catch
            {
                return "";
            }
        }

        private string GetInnerOfTagWithClass(HtmlDocument htmlDoc, string tag, string _class)
        {

            string searchNode = $"//{tag}[@class='{_class}']";
            string value;
            try
            {
                value = htmlDoc.DocumentNode
                                .SelectNodes(searchNode).ToList()[0].InnerText;
            }
            catch
            {
                value = "";
            }
            return value;

        }

        private SellerContact GetSellerContact(HtmlDocument htmlDoc)
        {
            return new SellerContact()
            {
                Email = "",
                Name = GetNameOfSeller(htmlDoc),
                Telephone = GetPhoneNumber(htmlDoc)
            };
        }

        private string GetFromRightSideProps(HtmlDocument htmlDoc, string innerText)
        {
            try
            {
                return htmlDoc.DocumentNode
                                .SelectNodes("//ul[@class='parameters__rolled']")
                                .Select(node => node.SelectNodes(".//li/span")).ToList()[0]
                                .Where(x => x.InnerText == innerText).ToList()[0]
                                .ParentNode.ChildNodes.FirstOrDefault(node => node.Name == "b").InnerText;
            }
            catch
            {
                return "";
            }

        }

        private PropertyPrice GetPropertyPrice(HtmlDocument htmlDoc)
        {
            string price = new String(GetInnerOfTagWithClass(htmlDoc, "span", "priceInfo__value").Where(Char.IsDigit).ToArray());
            string pricepermeter = new String(GetInnerOfTagWithClass(htmlDoc, "span", "priceInfo__additional").Where(Char.IsDigit).ToArray());
            string rent = new String(GetFromRightSideProps(htmlDoc, "Opłaty (czynsz administracyjny, media)").Where(Char.IsDigit).ToArray());
            decimal price2;
            decimal pricepermeter2;
            decimal rent2;
            Decimal.TryParse(price, out price2);
            Decimal.TryParse(pricepermeter, out pricepermeter2);
            Decimal.TryParse(rent, out rent2);
            return new PropertyPrice()
            {
                TotalGrossPrice = price2,
                PricePerMeter = pricepermeter2,
                ResidentalRent = rent2,
            };
        }

        private PropertyDetails GetPropertyDetails(HtmlDocument htmlDoc)
        {

            string area = new String(GetFromRightSideProps(htmlDoc, "Powierzchnia w m2").Where(Char.IsDigit).ToArray());
            string numberofrooms = new String(GetFromRightSideProps(htmlDoc, "Liczba pokoi").Where(Char.IsDigit).ToArray());
            string floornumber = new String(GetFromRightSideProps(htmlDoc, "Piętro").Where(Char.IsDigit).ToArray());
            string yearofconstruction = new String(GetFromRightSideProps(htmlDoc, "Rok budowy").Where(Char.IsDigit).ToArray());
            decimal area2;
            int numberofrooms2;
            int floornumber2;
            int year2;
            Decimal.TryParse(area, out area2);
            Int32.TryParse(numberofrooms, out numberofrooms2);
            Int32.TryParse(floornumber, out floornumber2);
            Int32.TryParse(yearofconstruction, out year2);

            return new PropertyDetails
            {
                Area = area2,
                NumberOfRooms = numberofrooms2,
                FloorNumber = floornumber2,
                YearOfConstruction = year2
            };
        }

        private PropertyAddress GetPropertyAddress(HtmlDocument htmlDoc)
        {
            List<string> address = GetPropertyLocation(htmlDoc);
            PolishCity city;
            Enum.TryParse(address[1], out city);
            return new PropertyAddress()
            {
                City = city,
                District = "",
                StreetName = address[0],
                DetailedAddress = ""
            };
        }

        private PropertyFeatures GetPropertyFeatures(HtmlDocument htmlDoc)
        {

            PropertyFeatures pf = new PropertyFeatures();

            if (GetFromRightSideProps(htmlDoc, "Miejsce parkingowe").Equals("przynależne na ulicy"))
            {
                if (GetFromRightSideProps(htmlDoc, "Liczba miejsc parkingowych") == "")
                {
                    pf.OutdoorParkingPlaces += 1;
                }
                else
                {
                    pf.OutdoorParkingPlaces += Int32.Parse(GetFromRightSideProps(htmlDoc, "Liczba miejsc parkingowych"));
                }
                
            }


            if (GetFromRightSideProps(htmlDoc, "Miejsce parkingowe").Equals("w garażu"))
            {
                if (GetFromRightSideProps(htmlDoc, "Liczba miejsc parkingowych") == "")
                {
                    pf.OutdoorParkingPlaces += 1;
                }
                else
                {
                    pf.OutdoorParkingPlaces += Int32.Parse(GetFromRightSideProps(htmlDoc, "Liczba miejsc parkingowych"));
                }
            }

            return pf;
        }

        private string GetPropertyRawDescription(HtmlDocument htmlDoc)
        {
            try
            {
                return ConvertFromUnicode(HtmlEntity.DeEntitize(htmlDoc.DocumentNode
                    .SelectNodes("//div[@class='description__rolled ql-container']")
                    .FirstOrDefault().InnerText));
            }
            catch
            {
                return "";
            }
        }

        private string ConvertFromUnicode(string uni)
        {
            return Encoding.UTF8.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(uni)));
        }

        private Entry GetEntryInfoFromGratka(GratkaSite site)
        {
            Entry entry = new Entry();
            Console.WriteLine(site.URL);
            HtmlDocument htmlDoc = Web.Load(site.URL);

            SellerContact sellerContact = GetSellerContact(htmlDoc);

            OfferDetails offerDetails = GetOfferDetails(htmlDoc);
            offerDetails.Url = site.URL;
            offerDetails.CreationDateTime = DateTime.Parse(site.addDate);
            offerDetails.LastUpdateDateTime = DateTime.Parse(site.lastModDate);
            offerDetails.SellerContact = sellerContact;

            PropertyPrice propertyPrice = GetPropertyPrice(htmlDoc);

            PropertyDetails propertyDetails = GetPropertyDetails(htmlDoc);

            PropertyAddress propertyAddress = GetPropertyAddress(htmlDoc);

            PropertyFeatures propertyFeatures = GetPropertyFeatures(htmlDoc);


            entry.OfferDetails = offerDetails;
            entry.PropertyPrice = propertyPrice;
            entry.PropertyDetails = propertyDetails;
            entry.PropertyAddress = propertyAddress;
            entry.PropertyFeatures = propertyFeatures;
            entry.RawDescription = GetPropertyRawDescription(htmlDoc);
            return entry;
        }
        #endregion
    }
}
