using HtmlAgilityPack;
using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.DziennikBaltycki
{

    public class DziennikBaltyckiIntegration : IWebSiteIntegration
    {
        private static WebClient klient { get; set; }
        private static HtmlDocument dokument { get; set; }
        private static HtmlAttribute atrybut { get; set; }
        static DziennikBaltyckiIntegration()
        {
            dokument = new HtmlDocument();
        }
        public WebPage WebPage { get; }
        public IDumpsRepository DumpsRepository { get; }
        public IEqualityComparer<Entry> EntriesComparer { get; }
        public DziennikBaltyckiIntegration(IDumpsRepository dumpsRepository,
            IEqualityComparer<Entry> equalityComparer)
        {
            DumpsRepository = dumpsRepository;
            EntriesComparer = equalityComparer;
            WebPage = new WebPage
            {
                Url = "https://dziennikbaltycki.pl/ogloszenia/12261,8433,fm,pk.html",
                Name = "Dziennik Bałtycki",
                WebPageFeatures = new WebPageFeatures
                {
                    HomeSale = true,
                    HomeRental = true,
                    HouseSale = true,
                    HouseRental = true
                }
            };
        }
        private HtmlNode PobierzDokument(string url)
        {
            using (klient = new WebClient())
            {
                var zawartosc = klient.DownloadString($@"{url}");
                dokument.LoadHtml(zawartosc);
            }
            return dokument.DocumentNode.SelectSingleNode("//body");
        }
        private int ZwrocLiczbeStron(HtmlNode htmlbody)
        {
            var iloscStron = htmlbody.SelectSingleNode("//a[@class=\"strzalka ostatniaStrz\"]");
            var wzorzec = new Regex(@"(.)(\w+)(.)(\d+)(.)(\d+)(.)(\d+),n,fm,pk.html");
            atrybut = iloscStron.Attributes["href"];
            Match ZwroconeDopasowanie = wzorzec.Match(atrybut.Value);

            return Convert.ToInt32(ZwroconeDopasowanie.Groups[4].Value);
        }
        private string? ZwrocPojedynczeOgloszenieMieszkaniowe(HtmlNode linkA)
        {
            atrybut = linkA.Attributes["href"];

            if (atrybut != null)
            {
                var element = linkA.SelectSingleNode("//header");
                var cena = element.Element("p");

                if (cena != null)
                {
                    string nowy = cena.InnerText.Split(",")[0].Replace(" ", string.Empty);

                    if (Convert.ToInt32(nowy) < 500)
                    {
                        return null;
                    }
                    if (atrybut.Value.ToLower().Contains(@"pomieszczenia-biurowe-na-wynajem") || atrybut.Value.ToLower().Contains(@"lokal")
                        || atrybut.Value.ToLower().Contains(@"hala") || atrybut.Value.ToLower().Contains(@"dzialka"))
                    {
                        return null;
                    }
                }
                return atrybut.Value;
            }
            return null;
        }
        private List<string> PobiezLinkiDoStron()
        {
            HtmlNode htmlBody = PobierzDokument("https://dziennikbaltycki.pl/ogloszenia/12261,8433,fm,pk.html");
            var section = htmlBody.SelectSingleNode("//section[@id='ogloszenia-miasta']");
            List<string> linkiDoStron = new List<string>();
            linkiDoStron.Add(@"12261,8433,n,fm,pk.html");

            foreach (var link in section.Descendants("li"))
            {
                atrybut = link.FirstChild.Attributes["href"];

                if (atrybut != null)
                {
                    string czescLinku = atrybut.Value.Split("/")[4];
                    string adres = $@"{czescLinku.Split(",")[0]},{czescLinku.Split(",")[1]},n,fm,pk.html";
                    linkiDoStron.Add(adres);
                }
            }
            return linkiDoStron;
        }

        //Odwiedzam 3 strony w każdym z 10 miast i wyciągam po 5 ofert, jeśli pasują do kryteriów. Spowodowane jest to ogromną ilością ofert 
        private List<string> PobierzLinkiDoMieszkan(List<string> linkiDoStron)
        {
            string url2 = @"https://dziennikbaltycki.pl/ogloszenia/";

            int ileMiast = 0;

            List<string> linkiDoMieszkan = new List<string>();

            foreach (var link in linkiDoStron)
            {
                int ileStronWDanymMiescie = 1;
                int ileStron = ZwrocLiczbeStron(PobierzDokument($"{url2}{link}"));

                for (int i = 1; i <= ileStron; i++)
                {
                    HtmlNode htmlbody = PobierzDokument($"{url2}{i},{link}");
                    var sekcja = htmlbody.SelectSingleNode("//section[@id='lista-ogloszen']/ul");
                    int ileLinkow = 1;

                    foreach (var linkA in sekcja.Descendants("a"))
                    {
                        string mieszkanie = ZwrocPojedynczeOgloszenieMieszkaniowe(linkA);
                        if (mieszkanie != null)
                            linkiDoMieszkan.Add(atrybut.Value);
                        if (ileLinkow == 5)
                        {
                            break;
                        }
                        ileLinkow++;
                    }
                    if (ileStronWDanymMiescie == 3)
                    {
                        break;
                    }
                    ileStronWDanymMiescie++;
                }
                if (ileMiast == 10)
                {
                    break;
                }
                ileMiast++;
            }
            return linkiDoMieszkan;
        }
        private PolishCity ZwrocNazweMiasta(string miastoPoKonversji)
        {
            return (PolishCity)Enum.Parse(typeof(PolishCity), miastoPoKonversji);
        }
        private PropertyAddress ZwrocAdres(Dictionary<string, string> zbiorDanych)
        {
            string normalizacja = zbiorDanych["miasto"].Normalize(NormalizationForm.FormD);
            string miastoPoKonversji = new string(normalizacja.Where(c => c < 128).ToArray());

            if (Enum.IsDefined(typeof(PolishCity), miastoPoKonversji))
            {
                return new PropertyAddress
                {
                    City = ZwrocNazweMiasta(miastoPoKonversji),
                    District = zbiorDanych["dzielnica"],
                    StreetName = zbiorDanych["nazwaUlicy"],
                    //Brak danych na stronie
                    DetailedAddress = "Brak danych"
                };
            }
            else
            {
                return new PropertyAddress
                {
                    District = zbiorDanych["dzielnica"],
                    StreetName = zbiorDanych["nazwaUlicy"],
                    //Brak danych na stronie
                    DetailedAddress = "Brak danych"
                };
            }

        }
        private int? SprawdzIleMetrow(string czegoSzukamy, ref string opis)
        {
            int pozycjaStartowa = opis.LastIndexOf(czegoSzukamy) + czegoSzukamy.Length + 1;
            string obszarSprawdzania = opis.Substring(pozycjaStartowa, 10);

            if (obszarSprawdzania.Any(x => char.IsDigit(x)))
            {
                bool stop = false;
                string temp = null;
                foreach (var znak in obszarSprawdzania)
                {
                    if (char.IsDigit(znak))
                    {
                        temp += znak;
                        stop = true;
                    }
                    if (stop == true && znak == 'm')
                    {
                        break;
                    }
                }
                return Convert.ToInt32(temp);
            }
            else
            {
                return null;
            }
        }
        private OfferDetails ZwrocSzczegolyOferty(HtmlNode htmlbody, string url)
        {
            var tel = htmlbody.SelectSingleNode("//a[@class='phoneButton__button']");

            var sprzedawca = htmlbody.SelectSingleNode("//div[@class='offerOwner__details']/h3[@class='offerOwner__person ']");

            var jakaCena = htmlbody.SelectSingleNode("//span[@class='priceInfo__value']");

            var cenaBrutto = jakaCena != null && jakaCena.InnerText.Any(x => char.IsDigit(x)) ? Convert.ToDecimal(jakaCena.InnerText.Split("zł")[0].Replace(" ", string.Empty)) : 0;

            OfferKind sprzedarz = cenaBrutto == 0 || cenaBrutto > 10000 ? OfferKind.SALE : OfferKind.RENTAL;

            return new OfferDetails
            {
                Url = url,
                CreationDateTime = DateTime.Now,
                LastUpdateDateTime = DateTime.Now,
                OfferKind = sprzedarz,
                SellerContact = new SellerContact
                {
                    Telephone = tel != null ? tel.Attributes["data-full-phone-number"].Value : "brak informacji",
                    Name = sprzedawca != null ? sprzedawca.InnerText : "brak informacji"
                },
                IsStillValid = true
            };
        }
        private PropertyPrice ZwrocDaneOWartosci(HtmlNode htmlbody, Dictionary<string, string> zbiorDanych)
        {
            var jakaCena = htmlbody.SelectSingleNode("//span[@class='priceInfo__value']");

            var jakaCenaZaMetr = htmlbody.SelectSingleNode("//span[@class='priceInfo__additional']");

            return new PropertyPrice
            {
                TotalGrossPrice = jakaCena != null && jakaCena.InnerText.Any(x => char.IsDigit(x)) ? Convert.ToDecimal(jakaCena.InnerText.Split("zł")[0].Replace(" ", string.Empty)) : 0,
                PricePerMeter = jakaCenaZaMetr != null ? Convert.ToDecimal(jakaCenaZaMetr.InnerText.Split("zł")[0].Replace(",", ".").Replace(" ", string.Empty).Replace("\n", string.Empty), new CultureInfo("en-US")) : 0,
                ResidentalRent = zbiorDanych.ContainsKey("oplaty") ? Convert.ToDecimal(zbiorDanych["oplaty"]) : 0
            };
        }
        private PropertyDetails ZwrocSzczegolyWlasnosci(Dictionary<string, string> zbiorDanych)
        {
            return new PropertyDetails
            {
                Area = zbiorDanych.ContainsKey("powierzchnia") ? zbiorDanych["powierzchnia"] != null ? Convert.ToDecimal(zbiorDanych["powierzchnia"], new CultureInfo("en-US")) : 0 : 0,
                NumberOfRooms = zbiorDanych.ContainsKey("liczbaPokoi") ? zbiorDanych["liczbaPokoi"].All(x => char.IsDigit(x)) ? Convert.ToInt32(zbiorDanych["liczbaPokoi"]) :
                zbiorDanych["liczbaPokoi"].Any(x => char.IsDigit(x)) ? Convert.ToInt32(zbiorDanych["liczbaPokoi"].FirstOrDefault(x => char.IsDigit(x))) : 0 : 0,
                FloorNumber = zbiorDanych.ContainsKey("pietro") && zbiorDanych["pietro"].All(x => char.IsDigit(x)) ? Convert.ToInt32(zbiorDanych["pietro"]) : 0,
                YearOfConstruction = zbiorDanych.ContainsKey("rokBudowy") ? Convert.ToInt32(zbiorDanych["rokBudowy"]) : 0,
            };
        }
        private int? ZwrocParkingZewnetrzny(Dictionary<string, string> zbiorDanych)
        {
            return zbiorDanych.ContainsKey("miejsceParkingowe") ? !zbiorDanych["miejsceParkingowe"].Contains("podziemn") && !zbiorDanych["miejsceParkingowe"].Contains("garaż") && !zbiorDanych["miejsceParkingowe"].Contains("brak") ?
                zbiorDanych.ContainsKey("liczbaMiejscParkingowych") ? Convert.ToInt32(zbiorDanych["liczbaMiejscParkingowych"]) : 1 : 0 : 0;
        }
        private int? ZwrocParkingWewnetrzny(Dictionary<string, string> zbiorDanych)
        {
            return zbiorDanych.ContainsKey("miejsceParkingowe") ? zbiorDanych["miejsceParkingowe"].Contains("podziemn") || zbiorDanych["miejsceParkingowe"].Contains("garaż") ?
                zbiorDanych.ContainsKey("liczbaMiejscParkingowych") ? Convert.ToInt32(zbiorDanych["liczbaMiejscParkingowych"]) : 1 : 0 : 0;
        }
        private PropertyFeatures ZwrocCechy(HtmlNode htmlbody, Dictionary<string, string> zbiorDanych)
        {
            var opis = ZwrocOpis(htmlbody);

            return new PropertyFeatures
            {
                GardenArea = opis.Contains("ogród") ? SprawdzIleMetrow("ogród", ref opis) : null,
                Balconies = opis.Contains("balkon") ? 1 : 0,
                BasementArea = opis.Contains("piwnica") ? SprawdzIleMetrow("piwnica", ref opis) : opis.Contains("Komórka lokatorska") ? SprawdzIleMetrow("piwnica", ref opis) : null,
                OutdoorParkingPlaces = ZwrocParkingZewnetrzny(zbiorDanych),
                IndoorParkingPlaces = ZwrocParkingWewnetrzny(zbiorDanych)
            };
        }
        private Dictionary<string, string> ZwrocZbiorDanych(HtmlNode htmlbody)
        {
            var listaParametrow = htmlbody.SelectSingleNode("//ul[@class='parameters__rolled']");

            var Adres = htmlbody.SelectSingleNode("//h1[@class='sticker__title']");

            Dictionary<string, string> zbiorDanych = new Dictionary<string, string>();

            foreach (var parametr in listaParametrow.Descendants("li"))
            {
                if (parametr.Element("span") != null && parametr.Element("b") != null)
                {
                    var zawartoscSpan = parametr.Element("span").InnerText;
                    var zawartoscB = parametr.Element("b").InnerText;

                    if (zawartoscSpan.ToLower().Contains("lokalizacja"))
                    {
                        var dane = parametr.Descendants("a").ToList();

                        zbiorDanych.Add("nazwaUlicy", Adres.InnerText.Contains("ul") ? Adres.InnerText.Split("ul.")[1] : "Brak informacji");

                        zbiorDanych.Add("miasto", dane[0].InnerHtml.Replace(" ", "_").ToUpper());

                        zbiorDanych.Add("dzielnica", dane.Count == 3 ? dane[1].InnerText : "brak informacji");
                    }
                    if (zawartoscSpan.ToLower().Contains("powierzchnia w m2"))
                    {
                        zbiorDanych.Add("powierzchnia", zawartoscB.Split("m")[0].Replace(",", ".").Replace(" ", string.Empty));
                    }
                    if (zawartoscSpan.ToLower().Contains("liczba pokoi"))
                    {
                        zbiorDanych.Add("liczbaPokoi", zawartoscB);
                    }
                    if (zawartoscSpan.ToLower().Contains("piętro"))
                    {
                        zbiorDanych.Add("pietro", zawartoscB);
                    }
                    if (zawartoscSpan.ToLower().Contains("opłaty"))
                    {
                        zbiorDanych.Add("oplaty", zawartoscB.Split("zł")[0]);
                    }
                    if (zawartoscSpan.ToLower().Contains("rok budowy"))
                    {
                        zbiorDanych.Add("rokBudowy", zawartoscB);
                    }
                    if (zawartoscSpan.ToLower().Contains("miejsce parkingowe"))
                    {
                        zbiorDanych.Add("miejsceParkingowe", zawartoscB);
                    }
                    if (zawartoscSpan.ToLower().Contains("liczba miejsc parkingowych"))
                    {
                        zbiorDanych.Add("liczbaMiejscParkingowych", zawartoscB);
                    }
                }
            }

            return zbiorDanych;
        }
        private string ZwrocOpis(HtmlNode htmlbody)
        {
            var Opis = htmlbody.SelectSingleNode("//div[@class='description__rolled ql-container']");
            return Opis != null ? Opis.InnerText : null;
        }
        public Dump GenerateDump()
        {
            List<string> linkiDoStron = PobiezLinkiDoStron();
            List<string> linkiDoMieszkan = PobierzLinkiDoMieszkan(linkiDoStron);
            List<Entry> mieszkania = new List<Entry>();

            foreach (var url in linkiDoMieszkan)
            {
                HtmlNode htmlbody = PobierzDokument(url);
                Dictionary<string, string> zbiorDanych = ZwrocZbiorDanych(htmlbody);

                mieszkania.Add(new Entry
                {
                    OfferDetails = ZwrocSzczegolyOferty(htmlbody, url),
                    PropertyPrice = ZwrocDaneOWartosci(htmlbody, zbiorDanych),
                    PropertyDetails = ZwrocSzczegolyWlasnosci(zbiorDanych),
                    PropertyAddress = ZwrocAdres(zbiorDanych),
                    PropertyFeatures = ZwrocCechy(htmlbody, zbiorDanych),
                    RawDescription = ZwrocOpis(htmlbody),
                });
            }
            return new Dump
            {
                WebPage = WebPage,
                DateTime = DateTime.Now,
                Entries = mieszkania
            };
        }
    }
}


