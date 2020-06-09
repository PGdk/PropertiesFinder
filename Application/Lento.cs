using HtmlAgilityPack;
using Interfaces;
using Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public class Lento : IWebSiteIntegration
    {
        public WebPage WebPage { get; }
        public IDumpsRepository DumpsRepository { get; }

        public IEqualityComparer<Entry> EntriesComparer { get; }

        public Lento(IDumpsRepository dumpsRepository,
            IEqualityComparer<Entry> equalityComparer)
        {
            DumpsRepository = dumpsRepository;
            EntriesComparer = equalityComparer;
            WebPage = new WebPage
            {
                Url = "https://www.lento.pl/",
                Name = "Lento",
                WebPageFeatures = new WebPageFeatures
                {
                    HomeSale = true,
                    HomeRental = true,
                    HouseSale = true,
                    HouseRental = true
                }
            };
        }
        public Lento()
        {
            WebPage = new WebPage
            {
                Url = "https://www.lento.pl/",
                Name = "Lento",
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
            var entries = new List<Entry>();

            HtmlWeb web = new HtmlWeb();
            var doc = web.Load("https://www.lento.pl/nieruchomosci/?page=1");

            int NumberOfPages = Int32.Parse(doc.DocumentNode.SelectSingleNode("/html/body/main/div[@class = 'container-fluid otherpage-container']/div[@class = 'row alllist']/div[@class = 'col-md-10 col-md-pull-2']/div[@class = 'pagination']/span[@class = 'hash number alike button']").InnerText);
            // ofert jest ponad milion, dziele to na 400 by nie musieć czekać cały dzień aż skończy wszystkie przerabiać
            for (int i = 1; i <= NumberOfPages / 400; i++)
            {
                doc = web.Load("https://www.lento.pl/nieruchomosci/?page=" + i.ToString());
                var offers = doc.DocumentNode.SelectNodes("/html/body/main/div[@class = 'container-fluid otherpage-container']/div[@class = 'row alllist']/div[@class = 'col-md-10 col-md-pull-2']/div[@class = 'tablelist']/div");
                foreach (var offer in offers)
                {
                    string URL = "";
                    try
                    {
                        URL = offer.SelectSingleNode("div[@class = 'desc-list-row   desc-list-row-cover-show']/a/@href").GetAttributeValue("href", null);
                    }
                    catch
                    {
                        //coś co nie jest ofertą
                        continue;
                    }
                    var page = web.Load(URL);
                    var node = page.DocumentNode.SelectSingleNode("/html/body/main/div[@class = 'container-fluid otherpage-container']/div[@class = 'row']/div[@class = 'col-md-12']/div[@class = 'showogl']/div[@class = 'row-sm-flex']");

                    DateTime CreationDateTime = DateConverter.Convert(node.SelectSingleNode("div[@class = 'col-sm-left']/div[@class = 'quickdetail']/div[@class = 'pull-left text-13']/span").InnerText);
                    DateTime? LastUpdateDateTime = null;
                    OfferKind OfferKind = OfferKind.SALE;
                    foreach (var div in page.DocumentNode.SelectNodes("/html/body/main/div[@class = 'container-fluid otherpage-container']/div[@class = 'row breadcrumbs']/div[@class = 'col-md-12']/div[@class = 'breadcrumb-wrap-noflex breadcrumb-wrap']/ul[@class = 'breadcrumb']/li"))
                    {
                        if (div.SelectSingleNode("a/span").InnerText == "Do wynajęcia")
                        {
                            OfferKind = OfferKind.RENTAL;
                            break;
                        }
                    }
                    string temp = "";
                    var tmp = node.SelectSingleNode("div[@class = 'col-sm-right']/div[contains(@class, 'userbox userbox-')]/div[@class = 'userbox-wrap']/div[@class = 'hidden-xs row']/div[@class = 'col-sm-12']/button");
                    if (tmp != null)
                    {
                        temp = tmp.InnerText;
                        temp = temp.Replace("\n", "");
                        temp = temp.Replace("Pokaż", ""); // wiem że ten numer to głownie 'x', ale nie byłem w stanie ogarnąć Selenium WebDriver
                    }
                    string Telephone = temp;
                    string Name = node.SelectSingleNode("div[@class = 'col-sm-right']/div[contains(@class, 'userbox userbox-')]/div[@class = 'userbox-wrap']/div[@class = 'userbox-name']").InnerText;
                    string Email = "";
                    bool IsStillValid = true;

                    decimal TotalGrossPrice = 0;
                    decimal PricePerMeter = 0;
                    decimal? ResidentalRent = null;
                    decimal Area = 0;
                    int NumberOfRooms = 0;
                    int? FloorNumber = null;
                    int? YearOfConstruction = null;
                    PolishCity City = 0;
                    string District = "";
                    string StreetName = "";
                    string DetailedAddress = "";
                    decimal? GardenArea = null;
                    int? Balconies = null;
                    decimal? BasementArea = null;
                    int? OutdoorParkingPlaces = null;
                    int? IndoorParkingPlaces = null;
                    string RawDescription = "";
                    temp = node.SelectSingleNode("div[@class = 'col-sm-right']/div[contains(@class, 'userbox userbox-')]/div[@class = 'userbox-location']/div[@class = 'userbox-location-tip']/*[1]").InnerText;
                    temp = temp.ToUpper();
                    temp = temp.Replace("Ą", "A");
                    temp = temp.Replace("Ę", "E");
                    temp = temp.Replace("Ó", "O");
                    temp = temp.Replace("Ś", "S");
                    temp = temp.Replace("Ł", "L");
                    temp = temp.Replace("Ń", "N");
                    temp = temp.Replace("Ć", "C");
                    temp = temp.Replace("Ż", "Z");
                    temp = temp.Replace("Ź", "Z");
                    if (temp.Contains("/"))
                    {
                        string temp2;
                        temp2 = temp.Remove(0, temp.IndexOf("/") + 2);
                        temp = temp.Remove(temp.IndexOf("/") - 1);
                        temp = temp.Replace(" ", "_");
                        temp = temp.Replace("-", "_");
                        try
                        {
                            City = (PolishCity)Enum.Parse(typeof(PolishCity), temp2);
                        }
                        catch
                        {
                            City = (PolishCity)Enum.Parse(typeof(PolishCity), temp);
                        }
                    }
                    else
                    {
                        try
                        {
                            City = (PolishCity)Enum.Parse(typeof(PolishCity), temp);
                        }
                        catch
                        {
                            //wieś
                        }
                    }
                    foreach (var info in node.SelectNodes("div[@class = 'col-sm-left']/div[@class = 'details text-15']/ul/li"))
                    {
                        switch (info.SelectSingleNode("span[@class = 'label']").InnerText)
                        {
                            case "Cena:":
                                try
                                {
                                    temp = info.SelectSingleNode("span[@class = 'row-old']/span[@class = 'price']").InnerText;
                                }
                                catch
                                {
                                    continue;
                                    //ponieważ ten pan
                                    //https://warszawa.lento.pl/przyjme-pania-mloda-kobiete-do,3247868.html
                                    //nie będę go zaliczał
                                }
                                temp = temp.Remove(temp.IndexOf(" ", 1));
                                if (temp.Contains("z"))
                                    temp = temp.Remove(temp.IndexOf("z"));
                                if (OfferKind == OfferKind.RENTAL)
                                {
                                    ResidentalRent = (decimal)float.Parse(temp);
                                }
                                TotalGrossPrice = (decimal)float.Parse(temp);
                                if (info.SelectSingleNode("span[@class = 'row-old']/span[@class = 'text-g']") != null)
                                {
                                    temp = info.SelectSingleNode("span[@class = 'row-old']/span[@class = 'text-g']").InnerText;
                                    PricePerMeter = Int32.Parse(temp.Remove(temp.IndexOf("z")));
                                }
                                break;
                            case "Powierzchnia:":
                                temp = info.SelectSingleNode("span[@class = 'row-old']").InnerText;
                                Area = Int32.Parse(temp.Remove(temp.IndexOf("m")));
                                break;
                            case "Liczba pokoi:":
                                temp = info.SelectSingleNode("span[@class = 'row-old']").InnerText;
                                NumberOfRooms = Int32.Parse(new string(temp.Where(c => char.IsDigit(c)).ToArray()));
                                break;
                            case "Piętro :":
                                temp = info.SelectSingleNode("span[@class = 'row-old']").InnerText;
                                if (temp.Contains("arter"))
                                    FloorNumber = 0;
                                else
                                {
                                    try
                                    {
                                        FloorNumber = Int32.Parse(new string(temp.Where(c => char.IsDigit(c)).ToArray()));
                                    }
                                    catch
                                    {
                                        //ponieważ ktoś wstawił ofertę z Piętro :Poddasze i Liczba pięter:Wieżowiec
                                    }
                                }
                                break;
                            case "Rok budowy:":
                                temp = info.SelectSingleNode("span[@class = 'row-old']").InnerText;
                                YearOfConstruction = Int32.Parse(new string(temp.Where(c => char.IsDigit(c)).ToArray()));
                                break;
                            case "Pow. dodatkowa:":
                                temp = info.SelectSingleNode("span[@class = 'row-old']").InnerText;
                                GardenArea = temp.Contains("gród") ? 1 : 0;
                                Balconies = temp.Contains("alkon") ? 1 : 0;
                                BasementArea = temp.Contains("iwnica") ? 1 : 0;
                                if (temp.Contains("arking"))
                                {
                                    if (temp.Contains("Wewnętrz") || temp.Contains("wewnętrz"))
                                    {
                                        IndoorParkingPlaces = 1;
                                        if (temp.Contains("Zewnętrz") || temp.Contains("zewnętrz"))
                                        {
                                            OutdoorParkingPlaces = 1;
                                        }
                                    }
                                    else
                                        OutdoorParkingPlaces = 1;
                                }
                                break;
                        }
                    }

                    RawDescription = node.SelectSingleNode("div[@class = 'col-sm-left']/div[@class = 'desc text-15']").InnerText.Remove(0, 12);
                    if (RawDescription.Contains("ul. "))
                    {
                        temp = RawDescription.Remove(0, RawDescription.IndexOf("ul. ") + 4);
                        int min = temp.IndexOf(" ");
                        if (min < 0 || (min > temp.IndexOf(".") && temp.IndexOf(".") > 0))
                        {
                            min = temp.IndexOf(".");
                        }
                        StreetName = temp.Substring(0, min);    //rozwiązanie nie działa dla wielku przupadku ale nie miałem lepszego pomysłu
                                                                //nie mam również pomysłu jak wyciągnąć numer mieszkania gdyż nie każdy podaje przy nazwie ulicy a jak już podają to każdy inaczej
                                                                //a na nazwe dzielnicy to nie mam pojęcia jak
                    }

                    entries.Add(new Entry
                    {
                        OfferDetails = new OfferDetails
                        {
                            Url = URL,
                            CreationDateTime = CreationDateTime,
                            LastUpdateDateTime = LastUpdateDateTime,
                            OfferKind = OfferKind,
                            SellerContact = new SellerContact
                            {
                                Telephone = Telephone,
                                Name = Name,
                                Email = Email
                            },
                            IsStillValid = IsStillValid
                        },
                        PropertyPrice = new PropertyPrice
                        {
                            TotalGrossPrice = TotalGrossPrice,
                            PricePerMeter = PricePerMeter,
                            ResidentalRent = ResidentalRent
                        },
                        PropertyDetails = new PropertyDetails
                        {
                            Area = Area,
                            NumberOfRooms = NumberOfRooms,
                            FloorNumber = FloorNumber,
                            YearOfConstruction = YearOfConstruction
                        },
                        PropertyAddress = new PropertyAddress
                        {
                            City = City,
                            District = District,
                            StreetName = StreetName,
                            DetailedAddress = DetailedAddress
                        },
                        PropertyFeatures = new PropertyFeatures
                        {
                            GardenArea = GardenArea,
                            Balconies = Balconies,
                            BasementArea = BasementArea,
                            OutdoorParkingPlaces = OutdoorParkingPlaces,
                            IndoorParkingPlaces = IndoorParkingPlaces
                        },
                        RawDescription = RawDescription
                    });
                }
            }
            return new Dump
            {
                WebPage = WebPage,
                DateTime = DateTime.Now,
                Entries = entries
            };
        }
        public Dump GenerateDumpFromPage(int PageNumber)
        {
            var entries = new List<Entry>();

            HtmlWeb web = new HtmlWeb();
            var doc = web.Load("https://www.lento.pl/nieruchomosci/?page=" + PageNumber.ToString());

                var offers = doc.DocumentNode.SelectNodes("/html/body/main/div[@class = 'container-fluid otherpage-container']/div[@class = 'row alllist']/div[@class = 'col-md-10 col-md-pull-2']/div[@class = 'tablelist']/div");
                foreach (var offer in offers)
                {
                    string URL = "";
                    try
                    {
                        URL = offer.SelectSingleNode("div[@class = 'desc-list-row   desc-list-row-cover-show']/a/@href").GetAttributeValue("href", null);
                    }
                    catch
                    {
                        //coś co nie jest ofertą
                        continue;
                    }
                    var page = web.Load(URL);
                    var node = page.DocumentNode.SelectSingleNode("/html/body/main/div[@class = 'container-fluid otherpage-container']/div[@class = 'row']/div[@class = 'col-md-12']/div[@class = 'showogl']/div[@class = 'row-sm-flex']");

                    DateTime CreationDateTime = DateConverter.Convert(node.SelectSingleNode("div[@class = 'col-sm-left']/div[@class = 'quickdetail']/div[@class = 'pull-left text-13']/span").InnerText);
                    DateTime? LastUpdateDateTime = null;
                    OfferKind OfferKind = OfferKind.SALE;
                    foreach (var div in page.DocumentNode.SelectNodes("/html/body/main/div[@class = 'container-fluid otherpage-container']/div[@class = 'row breadcrumbs']/div[@class = 'col-md-12']/div[@class = 'breadcrumb-wrap-noflex breadcrumb-wrap']/ul[@class = 'breadcrumb']/li"))
                    {
                        if (div.SelectSingleNode("a/span").InnerText == "Do wynajęcia")
                        {
                            OfferKind = OfferKind.RENTAL;
                            break;
                        }
                    }
                    string temp = "";
                    var tmp = node.SelectSingleNode("div[@class = 'col-sm-right']/div[contains(@class, 'userbox userbox-')]/div[@class = 'userbox-wrap']/div[@class = 'hidden-xs row']/div[@class = 'col-sm-12']/button");
                    if (tmp != null)
                    {
                        temp = tmp.InnerText;
                        temp = temp.Replace("\n", "");
                        temp = temp.Replace("Pokaż", ""); // wiem że ten numer to głownie 'x', ale nie byłem w stanie ogarnąć Selenium WebDriver
                    }
                    string Telephone = temp;
                    string Name = node.SelectSingleNode("div[@class = 'col-sm-right']/div[contains(@class, 'userbox userbox-')]/div[@class = 'userbox-wrap']/div[@class = 'userbox-name']").InnerText;
                    string Email = "";
                    bool IsStillValid = true;

                    decimal TotalGrossPrice = 0;
                    decimal PricePerMeter = 0;
                    decimal? ResidentalRent = null;
                    decimal Area = 0;
                    int NumberOfRooms = 0;
                    int? FloorNumber = null;
                    int? YearOfConstruction = null;
                    PolishCity City = 0;
                    string District = "";
                    string StreetName = "";
                    string DetailedAddress = "";
                    decimal? GardenArea = null;
                    int? Balconies = null;
                    decimal? BasementArea = null;
                    int? OutdoorParkingPlaces = null;
                    int? IndoorParkingPlaces = null;
                    string RawDescription = "";
                    temp = node.SelectSingleNode("div[@class = 'col-sm-right']/div[contains(@class, 'userbox userbox-')]/div[@class = 'userbox-location']/div[@class = 'userbox-location-tip']/*[1]").InnerText;
                    temp = temp.ToUpper();
                    temp = temp.Replace("Ą", "A");
                    temp = temp.Replace("Ę", "E");
                    temp = temp.Replace("Ó", "O");
                    temp = temp.Replace("Ś", "S");
                    temp = temp.Replace("Ł", "L");
                    temp = temp.Replace("Ń", "N");
                    temp = temp.Replace("Ć", "C");
                    temp = temp.Replace("Ż", "Z");
                    temp = temp.Replace("Ź", "Z");
                    if (temp.Contains("/"))
                    {
                        string temp2;
                        temp2 = temp.Remove(0, temp.IndexOf("/") + 2);
                        temp = temp.Remove(temp.IndexOf("/") - 1);
                        temp = temp.Replace(" ", "_");
                        temp = temp.Replace("-", "_");
                        try
                        {
                            City = (PolishCity)Enum.Parse(typeof(PolishCity), temp2);
                        }
                        catch
                        {
                            City = (PolishCity)Enum.Parse(typeof(PolishCity), temp);
                        }
                    }
                    else
                    {
                        try
                        {
                            City = (PolishCity)Enum.Parse(typeof(PolishCity), temp);
                        }
                        catch
                        {
                            //wieś
                        }
                    }
                    foreach (var info in node.SelectNodes("div[@class = 'col-sm-left']/div[@class = 'details text-15']/ul/li"))
                    {
                        switch (info.SelectSingleNode("span[@class = 'label']").InnerText)
                        {
                            case "Cena:":
                                try
                                {
                                    temp = info.SelectSingleNode("span[@class = 'row-old']/span[@class = 'price']").InnerText;
                                }
                                catch
                                {
                                    continue;
                                    //ponieważ ten pan
                                    //https://warszawa.lento.pl/przyjme-pania-mloda-kobiete-do,3247868.html
                                    //nie będę go zaliczał
                                }
                                temp = temp.Remove(temp.IndexOf(" ", 1));
                                if (temp.Contains("z"))
                                    temp = temp.Remove(temp.IndexOf("z"));
                                if (OfferKind == OfferKind.RENTAL)
                                {
                                    ResidentalRent = (decimal)float.Parse(temp);
                                }
                                TotalGrossPrice = (decimal)float.Parse(temp);
                                if (info.SelectSingleNode("span[@class = 'row-old']/span[@class = 'text-g']") != null)
                                {
                                    temp = info.SelectSingleNode("span[@class = 'row-old']/span[@class = 'text-g']").InnerText;
                                    PricePerMeter = Int32.Parse(temp.Remove(temp.IndexOf("z")));
                                }
                                break;
                            case "Powierzchnia:":
                                temp = info.SelectSingleNode("span[@class = 'row-old']").InnerText;
                                Area = Int32.Parse(temp.Remove(temp.IndexOf("m")));
                                break;
                            case "Liczba pokoi:":
                                temp = info.SelectSingleNode("span[@class = 'row-old']").InnerText;
                                NumberOfRooms = Int32.Parse(new string(temp.Where(c => char.IsDigit(c)).ToArray()));
                                break;
                            case "Piętro :":
                                temp = info.SelectSingleNode("span[@class = 'row-old']").InnerText;
                                if (temp.Contains("arter"))
                                    FloorNumber = 0;
                                else
                                {
                                    try
                                    {
                                        FloorNumber = Int32.Parse(new string(temp.Where(c => char.IsDigit(c)).ToArray()));
                                    }
                                    catch
                                    {
                                        //ponieważ ktoś wstawił ofertę z Piętro :Poddasze i Liczba pięter:Wieżowiec
                                    }
                                }
                                break;
                            case "Rok budowy:":
                                temp = info.SelectSingleNode("span[@class = 'row-old']").InnerText;
                                YearOfConstruction = Int32.Parse(new string(temp.Where(c => char.IsDigit(c)).ToArray()));
                                break;
                            case "Pow. dodatkowa:":
                                temp = info.SelectSingleNode("span[@class = 'row-old']").InnerText;
                                GardenArea = temp.Contains("gród") ? 1 : 0;
                                Balconies = temp.Contains("alkon") ? 1 : 0;
                                BasementArea = temp.Contains("iwnica") ? 1 : 0;
                                if (temp.Contains("arking"))
                                {
                                    if (temp.Contains("Wewnętrz") || temp.Contains("wewnętrz"))
                                    {
                                        IndoorParkingPlaces = 1;
                                        if (temp.Contains("Zewnętrz") || temp.Contains("zewnętrz"))
                                        {
                                            OutdoorParkingPlaces = 1;
                                        }
                                    }
                                    else
                                        OutdoorParkingPlaces = 1;
                                }
                                break;
                        }
                    }

                    RawDescription = node.SelectSingleNode("div[@class = 'col-sm-left']/div[@class = 'desc text-15']").InnerText.Remove(0, 12);
                    if(RawDescription.Contains("ul. "))
                    {
                        temp = RawDescription.Remove(0, RawDescription.IndexOf("ul. ") + 4);
                        int min = temp.IndexOf(" ");
                        if(min < 0 || (min > temp.IndexOf(".") && temp.IndexOf(".") > 0))
                        {
                            min = temp.IndexOf(".");
                        }
                        StreetName = temp.Substring(0, min);    //rozwiązanie nie działa dla wielku przupadku ale nie miałem lepszego pomysłu
                                                                //nie mam również pomysłu jak wyciągnąć numer mieszkania gdyż nie każdy podaje przy nazwie ulicy a jak już podają to każdy inaczej
                                                                //a na nazwe dzielnicy to nie mam pojęcia jak
                    }

                    entries.Add(new Entry
                    {
                        OfferDetails = new OfferDetails
                        {
                            Url = URL,
                            CreationDateTime = CreationDateTime,
                            LastUpdateDateTime = LastUpdateDateTime,
                            OfferKind = OfferKind,
                            SellerContact = new SellerContact
                            {
                                Telephone = Telephone,
                                Name = Name,
                                Email = Email
                            },
                            IsStillValid = IsStillValid
                        },
                        PropertyPrice = new PropertyPrice
                        {
                            TotalGrossPrice = TotalGrossPrice,
                            PricePerMeter = PricePerMeter,
                            ResidentalRent = ResidentalRent
                        },
                        PropertyDetails = new PropertyDetails
                        {
                            Area = Area,
                            NumberOfRooms = NumberOfRooms,
                            FloorNumber = FloorNumber,
                            YearOfConstruction = YearOfConstruction
                        },
                        PropertyAddress = new PropertyAddress
                        {
                            City = City,
                            District = District,
                            StreetName = StreetName,
                            DetailedAddress = DetailedAddress
                        },
                        PropertyFeatures = new PropertyFeatures
                        {
                            GardenArea = GardenArea,
                            Balconies = Balconies,
                            BasementArea = BasementArea,
                            OutdoorParkingPlaces = OutdoorParkingPlaces,
                            IndoorParkingPlaces = IndoorParkingPlaces
                        },
                        RawDescription = RawDescription
                    });
                }
            
            return new Dump
            {
                WebPage = WebPage,
                DateTime = DateTime.Now,
                Entries = entries
            };
        }
    }
}
