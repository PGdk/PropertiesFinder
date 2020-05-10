using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Tracing;
using HtmlAgilityPack;
using System.Linq;
using System.Text.RegularExpressions;

namespace Application
{
    public class DomiportaIntegration : IWebSiteIntegration
    {
        public WebPage WebPage { get; }

        public IDumpsRepository DumpsRepository { get; }

        public IEqualityComparer<Entry> EntriesComparer { get; }

        List<string> LinksToAds = new List<string>();

        public DomiportaIntegration(IDumpsRepository dumpsRepository,
           IEqualityComparer<Entry> equalityComparer)
        {
            DumpsRepository = dumpsRepository;
            EntriesComparer = equalityComparer;
            WebPage = new WebPage
            {
                Url = "https://www.domiporta.pl",
                Name = "Domiporta.pl",
                WebPageFeatures = new WebPageFeatures
                {
                    HomeSale = true,
                    HomeRental = false,
                    HouseSale = false,
                    HouseRental = false
                }
            };
        }

        public Dump GenerateDump() // <--- TODO
        {
            //var random = new Random();
            //var randomValue = random.Next() % 10;

            //Tutaj w normalnej sytuacji musimy ściągnąć dane z konkretnej strony, przeparsować je i dopiero wtedy zapisać do modelu Dump

            var web = new HtmlAgilityPack.HtmlWeb();

            for (int i = 1; i <= 1; i++) // i = ilosc stron
            {
                var doc = web.Load("https://www.domiporta.pl/mieszkanie/sprzedam?Rodzaj=Bezposrednie&PageNumber=" + i);

                AddLinksToList(doc, LinksToAds);
            }

            Console.WriteLine(LinksToAds.Count); // TMP

            var EntriesTmp = new List<Entry>();

            for (int i = 0; i < LinksToAds.Count; i++)
            {
                var doc = web.Load("https://www.domiporta.pl"+LinksToAds[i]);
                EntriesTmp.Add(CreateEntry(doc, LinksToAds[i])); 
            }    
 
            return new Dump
            {
                DateTime = DateTime.Now,
                WebPage = WebPage,
                Entries = EntriesTmp
            };

        }

        private static Entry CreateEntry(HtmlDocument htmldoc, string link)
        {
            return new Entry
            {
                OfferDetails = new OfferDetails
                {
                    //Url = $"{WebPage.Url}{link}",
                    Url = $"https://www.domiporta.pl{link}",
                    CreationDateTime = DateTime.Now,
                    OfferKind = OfferKind.SALE,
                    SellerContact = new SellerContact
                    {
                        //Email = "okazje@mieszkania.pl"
                        Telephone = htmldoc.DocumentNode.Descendants("a").Where(d => d.Attributes.Contains("data-tel")).FirstOrDefault().Attributes["data-tel"].Value,
                    },
                    IsStillValid = true
                },

                PropertyPrice = new PropertyPrice
                {
                    TotalGrossPrice = ParseToDecimal(htmldoc.DocumentNode.Descendants("span").Where(d => d.Attributes.Contains("itemprop") &&
                    d.Attributes["itemprop"].Value.Contains("price")).FirstOrDefault().InnerText),

                    PricePerMeter = ParseToDecimalPerM(htmldoc.DocumentNode.Descendants("span").Where(d => d.Attributes.Contains("class") &&
                    d.Attributes["class"].Value.Contains("summary__subtitle summary__subtitle--price")).FirstOrDefault().InnerText),
                },

                PropertyAddress = new PropertyAddress
                {
                    // PolishCity TODO
                    //City = htmldoc.DocumentNode.Descendants("span").Where(d => d.Attributes.Contains("itemprop") &&
                    //d.Attributes["itemprop"].Value.Contains("price")).FirstOrDefault().InnerText,

                    //ulica zawiera tez numer - male szanse na wyluskanie osobno
                    StreetName = htmldoc.DocumentNode.Descendants("span").Where(d => d.Attributes.Contains("itemprop") &&
                    d.Attributes["itemprop"].Value.Contains("streetAddress")).FirstOrDefault().InnerText,

                },

                PropertyDetails = new PropertyDetails
                { 
                    NumberOfRooms = GetIntProperty(htmldoc, "Liczba pokoi"),
                    FloorNumber = GetIntProperty(htmldoc, "Piętro"),
                    YearOfConstruction = GetIntProperty(htmldoc, "Rok budowy"),
                    Area = GetDecProperty(htmldoc, "Powierzchnia całkowita")
                },

                RawDescription = "",
            };
        }

        private static int GetIntProperty(HtmlDocument htmldoc, string value)
        {
            var nodes = htmldoc.DocumentNode.SelectNodes("//dt[@class='features__item_name']");

            int ret = 0;

            foreach (var node in nodes)
            {
                if (node.InnerHtml.Contains(value))
                {
                    try
                    {
                        ret = int.Parse(node.NextSibling.InnerHtml);
                    }
                    catch 
                    {
                        ret = 0;
                    }
                } 
            }
            return ret;
        }

        private static decimal GetDecProperty(HtmlDocument htmldoc, string value)
        {
            var nodes = htmldoc.DocumentNode.SelectNodes("//dt[@class='features__item_name']");

            decimal ret = 0;

            foreach (var node in nodes)
            {
                if (node.InnerHtml.Contains(value))
                {
                    try
                    {
                        ret = ParseToDecimalArea(node.NextSibling.InnerText);
                    }
                    catch
                    {
                        ret = 0;
                    }
                }
            }
            return ret;
        }

        private static decimal ParseToDecimal(string value)
        {
            value = Regex.Replace(value, "&#160;", "");
            value = Regex.Replace(value, "\\D", "");

            try
            {
                return decimal.Parse(value);
            }
            catch
            {
                return 0;
            }
        }

        private static decimal ParseToDecimalPerM(string value)
        {
            value = Regex.Replace(value, "&#160;", "");
            value = Regex.Replace(value, "zł/m2", "");
            value = Regex.Replace(value, " ", "");

            try
            {
                return decimal.Parse(value);
            }
            catch
            {
                return 0;
            }
        }

        private static decimal ParseToDecimalArea(string value)
        {
            value = Regex.Replace(value, " m2", "");

            try
            {
                return decimal.Parse(value);
            }
            catch
            {
                return 0;
            }
        }
        static void AddLinksToList(HtmlDocument htmldoc, List<string> LinksToAds)
        {
            foreach (HtmlNode link in htmldoc.DocumentNode.SelectNodes("//a[@class='sneakpeak__button_a']"))
            {
                HtmlAttribute att = link.Attributes["href"];
                //HtmlAttribute att2 = link.Attributes["class"];

                if (att.Value != null)
                {
                    LinksToAds.Add(att.Value);
                }
            }

        }

    }
}
