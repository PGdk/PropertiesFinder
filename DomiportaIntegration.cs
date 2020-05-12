using Interfaces;
using Models;
using System;
using System.Collections.Generic;
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
                    HomeRental = true,
                    HouseSale = false,
                    HouseRental = false
                }
            };
        }

        public Dump GenerateDump()
        {
            //var random = new Random();
            //var randomValue = random.Next() % 10;

            //Tutaj w normalnej sytuacji musimy ściągnąć dane z konkretnej strony, przeparsować je i dopiero wtedy zapisać do modelu Dump

            var web = new HtmlAgilityPack.HtmlWeb();

            //sale
            for (int i = 1; i <= 2; i++) // i = ilosc stron
            {
                var doc = web.Load("https://www.domiporta.pl/mieszkanie/sprzedam?Rodzaj=Bezposrednie&PageNumber=" + i);
                
                AddLinksToList(doc, LinksToAds);
            }

            //rental
            for (int i = 1; i <= 2; i++)
            {
                var doc = web.Load("https://www.domiporta.pl/mieszkanie/wynajme?rodzaj=bezposrednie&pagenumber=" + i);

                AddLinksToList(doc, LinksToAds);
            }

            // TMP - licznik stron
            //Console.WriteLine(LinksToAds.Count);

            var EntriesTmp = new List<Entry>();

            // dodawanie Entriskow

            for (int i = 0; i < LinksToAds.Count; i++)
            {
                var doc = web.Load("https://www.domiporta.pl" + LinksToAds[i]);
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
                    Url = $"https://www.domiporta.pl{link}",
                    CreationDateTime = DateTime.Now,

                    OfferKind = GetOfferKind(link),

                    SellerContact = new SellerContact
                    {
                        Telephone = GetTelephone(htmldoc),
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
               
                PropertyAddress = GetPropertyAddress(htmldoc),
         
                PropertyDetails = new PropertyDetails
                {
                    NumberOfRooms = GetIntProperty(htmldoc, "Liczba pokoi"),
                    FloorNumber = GetIntProperty(htmldoc, "Piętro"),
                    YearOfConstruction = GetIntProperty(htmldoc, "Rok budowy"),
                    Area = GetDecProperty(htmldoc, "Powierzchnia całkowita")
                },

                RawDescription = GetRawDescription(htmldoc),
            };
        }

        private static PropertyAddress GetPropertyAddress(HtmlDocument htmldoc)
        {
            string city;

            try
            {
                city = htmldoc.DocumentNode.Descendants("span").Where(d => d.Attributes.Contains("itemprop") &&
                       d.Attributes["itemprop"].Value.Contains("addressLocality")).FirstOrDefault().InnerText.Replace(" ", "_").ToUpper();
                
                city = city.Replace("Ą", "A");
                city = city.Replace("Ć", "C");
                city = city.Replace("Ę", "E");
                city = city.Replace("Ł", "L");
                city = city.Replace("Ó", "O");
                city = city.Replace("Ś", "S");
                city = city.Replace("Ź", "Z");
                city = city.Replace("Ż", "Z");
            }
            catch
            {
                city = "";
            }

            try
            {
                PolishCity _City = (PolishCity)Enum.Parse(typeof(PolishCity), city);

            return new PropertyAddress
            {
                City = _City,

                //District = htmldoc.DocumentNode.SelectSingleNode("//span[@itemprop='addressLocality']").NextSibling.NextSibling.InnerText,
                District = GetDistrict(htmldoc),
                StreetName = GetStreetName(htmldoc),
                DetailedAddress = GetNr(htmldoc),
            };

            }
            catch
            {
                return new PropertyAddress
                {
                    District = GetDistrict(htmldoc),
                    StreetName = GetStreetName(htmldoc),
                    DetailedAddress = GetNr(htmldoc),
                };
            }
        }

        private static string GetNr(HtmlDocument htmldoc)
        {
            string Nr = "";

            var StreetNode = htmldoc.DocumentNode.SelectSingleNode("//span[@itemprop='streetAddress']");

            if (StreetNode != null)
            {
                Nr = StreetNode.InnerText;
                string[] NrSplit = Nr.Split(' ');

                Nr = ParseNr(NrSplit.Last());
            }

            return Nr;
        }

        private static string GetDistrict(HtmlDocument htmldoc)
        {
            var DistrictNode = htmldoc.DocumentNode.SelectSingleNode("//span[@itemprop='addressLocality']");

            HtmlAttribute att = DistrictNode.NextSibling.NextSibling.Attributes["itemprop"];

            if (att == null && DistrictNode != null)
                return DistrictNode.NextSibling.NextSibling.InnerText;

            else
                return "";
        }

        private static string GetStreetName(HtmlDocument htmldoc)
        {
            string Street = "";
            string OnlyStreet = "";
            //var StreetNode = htmldoc.DocumentNode.Descendants("span").Where(d => d.Attributes.Contains("itemprop") &&
            //              d.Attributes["itemprop"].Value.Contains("streetAddress"));

            var StreetNode = htmldoc.DocumentNode.SelectSingleNode("//span[@itemprop='streetAddress']");

            if (StreetNode != null)
            {
                Street = StreetNode.InnerText;
               
                string[] StreetSplit = Street.Split(' ');

                if (StreetSplit.Length == 1)
                    OnlyStreet = StreetSplit[0];

                if (ParseNr(StreetSplit.Last()) != "") // czyli jest numer
                {
                    for (int i = 0; i < (StreetSplit.Length-1); i++)
                        OnlyStreet += $"{StreetSplit[i] }";
                }
            
            }
            
            return OnlyStreet;
        }

        private static string GetRawDescription(HtmlDocument htmldoc)
        {
            var nodes = htmldoc.DocumentNode.SelectNodes("//dt[@class='features__item_name']");

            string value = "";

            foreach (var node in nodes)
            {
                if (node.InnerHtml.Contains("Informacje dodatkowe"))
                {
                    try
                    {
                        value = node.NextSibling.InnerHtml;
                    }
                    catch
                    {
                        value = "";
                    }
                }
            }
            return value;
        }

        private static string GetTelephone(HtmlDocument htmldoc)
        {
            string tel;

            try
            {
                tel = htmldoc.DocumentNode.Descendants("a").Where(d => d.Attributes.Contains("data-tel")).FirstOrDefault().Attributes["data-tel"].Value;
            }
            catch
            {
                tel = "brak";
            }
            return tel;
        }

        private static OfferKind GetOfferKind(string link)
        {   
            if (link.Contains("wynajme"))
                return OfferKind.RENTAL;
            else
                return OfferKind.SALE;
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

        private static string ParseNr(string value)
        {
            Regex re = new Regex(@"^[0-9]");
            Match m = re.Match(value);

            return m.Value;
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

                if (att.Value != null)
                {
                    LinksToAds.Add(att.Value);
                }
            }
        }

    }
}
