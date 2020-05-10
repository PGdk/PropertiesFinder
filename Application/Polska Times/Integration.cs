using HtmlAgilityPack;
using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Xml;

namespace Application.PolskaTimes
{
    public class Integration : IWebSiteIntegration
    {

        public WebPage WebPage { get; }
        public IDumpsRepository DumpsRepository { get; }

        public IEqualityComparer<Entry> EntriesComparer { get; }

        public Integration(IDumpsRepository dumpsRepository,
            IEqualityComparer<Entry> equalityComparer)
        {
            DumpsRepository = dumpsRepository;
            EntriesComparer = equalityComparer;
            WebPage = new WebPage
            {
                Url = "https://polskatimes.pl/",
                Name = "Polska Times",
                WebPageFeatures = new WebPageFeatures
                {
                    HomeSale = true,
                    HomeRental = true,
                    HouseSale = true,
                    HouseRental = true
                }
            };
        }

        public List<Entry> GetHtml()
        {



            char[] alphabet = { 'a', 'ą', 'b', 'c', 'ć', 'd', 'e', 'ę', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'ł', 'm', 'n', 'ń', 'o', 'ó', 'p', 'r', 's', 'ś', 't', 'u', 'w', 'y', 'z', 'ź', 'ż',
                                    'A', 'Ą', 'B', 'C', 'Ć', 'D', 'E', 'Ę', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'Ł', 'M', 'N', 'Ń', 'O', 'Ó', 'P', 'R', 'S', 'Ś', 'T', 'U', 'W', 'Y', 'Z', 'Ź', 'Ż',
                                    '\n', ' ', '/', '.'};

            List<Entry> Entries = new List<Entry>();

            var numberOfPages = 1; // liczba stron do przeparsowania
            for (int i = 1; i <= numberOfPages; i++)
            {
                // url każdej ze stron
                var URL = "";
                if (i == 1)
                {
                    URL = "https://polskatimes.pl/ogloszenia/78425,8433,fm,pk.html";
                }
                else
                {
                    URL = "https://polskatimes.pl/ogloszenia/" + i + ",78425,8433,n,fm,pk.html";
                }
                HtmlWeb web = new HtmlWeb();

                var doc = web.Load(URL);

                // li kazdej oferty ze strony
                var section = doc.DocumentNode.SelectNodes("/html/body/div[contains(@class, 'row') and contains(@class, 'kontener')]" +
                                                                "/div[@id='glowna-kolumna']/section/ul/li");

                // przechodzi przez wszystkie oferty na stronie
                foreach (var list in section)
                {
                    var link = list.SelectSingleNode(".//a/@href").GetAttributeValue("href", null);
                    var offer = web.Load(link);

                    // sciezka do prawej kolumny parametrów
                    var offerParams = offer.DocumentNode.SelectSingleNode("/html/body/div[@id='offer-card']" +
                                                           "/div[contains(@class, 'offer__inner') and contains(@class, 'row')]" +
                                                           "/div[@id='rightColumn']");

                    // parametry daty
                    var dateCreation = "";
                    var dateUpdate = "";

                    // parametry z nad obrazka w ofercie
                    OfferKind offerKind = OfferKind.SALE;
                    var fullPrice = "";
                    decimal fullPriceDecimal = 0;
                    var pricePerM2 = "";
                    decimal pricePerM2Decimal = 0;
                    var pricePerMonth = "";
                    decimal pricePerMonthDecimal = 0;

                    // parametry z prawej kolumny oferty
                    var phone = "";
                    var localization = "";
                    PolishCity city = 0;
                    var district = "";
                    decimal areaM2Decimal = 0;
                    var areaM2 = "";
                    var roomNumbers = "";
                    int roomNumbersInt = 0;
                    var floor = "";
                    int floorInt = 0;
                    var constructionYear = "";
                    int constructionYearInt = 0;
                    var stateOfEstate = "";

                    // z dodatkowego diva, jeśli istnieje
                    bool validOffer = true;

                    // parametry z lewej kolumny oferty
                    var nameSurname = "";
                    var rawDescription = "";

                    // parametr z mapy
                    var street = "";


                    // data utworzenia oferty
                    if (list.SelectSingleNode(".//a/div//footer/p[contains(@class, 'daty')]/*[1]") != null)
                    {
                        dateCreation = list.SelectSingleNode(".//a/div//footer/p[contains(@class, 'daty')]/*[1]").InnerText;
                    }
                    // data aktualizacji oferty
                    if (list.SelectSingleNode(".//a/div//footer/p[contains(@class, 'daty')]/*[2]") != null)
                    {
                        dateUpdate = list.SelectSingleNode(".//a/div//footer/p[contains(@class, 'daty')]/*[2]").InnerText;
                    }
                    // wynajem czy sprzedaż
                    if (offer.DocumentNode.SelectSingleNode("/html/body/div[@id='offer-card']//span[@class='priceInfo__value']/span[@class='priceInfo__currency']") != null)
                    {
                        if (offer.DocumentNode.SelectSingleNode("/html/body/div[@id='offer-card']//span[@class='priceInfo__value']/span[contains(. , 'miesiąc')]") != null)
                        {
                            offerKind = OfferKind.RENTAL;
                        }
                        else
                        {
                            offerKind = OfferKind.SALE;
                        }
                    }

                    // parsowanie ceny w zależności od typu oferty
                    if (offerKind == OfferKind.RENTAL)
                    {
                        // cena za wynajem
                        if (offer.DocumentNode.SelectSingleNode("/html/body/div[@id='offer-card']//span[@class='priceInfo__value']") != null)
                        {
                            pricePerMonth = offer.DocumentNode.SelectSingleNode("/html/body/div[@id='offer-card']//span[@class='priceInfo__value']").InnerText.Trim(alphabet);
                            if (pricePerMonth != "")
                            {
                                pricePerMonthDecimal = decimal.Parse(pricePerMonth);
                            }
                        }
                    }
                    else if (offerKind == OfferKind.SALE)
                    {
                        // cena całkowita za kupno
                        if (offer.DocumentNode.SelectSingleNode("/html/body/div[@id='offer-card']//span[@class='priceInfo__value']") != null)
                        {
                            fullPrice = offer.DocumentNode.SelectSingleNode("/html/body/div[@id='offer-card']//span[@class='priceInfo__value']").InnerText.Trim(alphabet);
                            if (fullPrice != "")
                            {
                                fullPriceDecimal = decimal.Parse(fullPrice);
                            }
                        }
                        // cena za metr
                        if (offer.DocumentNode.SelectSingleNode("/html/body/div[@id='offer-card']//span[@class='priceInfo__additional']") != null)
                        {
                            pricePerM2 = offer.DocumentNode.SelectSingleNode("/html/body/div[@id='offer-card']//span[@class='priceInfo__additional']").InnerText.Trim(alphabet).Replace(" zł/m2", "");
                            if (pricePerM2 != "")
                            {
                                pricePerM2Decimal = decimal.Parse(pricePerM2);
                            }
                        }
                    }
                    // numer tel
                    if (offerParams.SelectSingleNode(".//ul[@class='parameters__rolled']/li/div[@class='parameters__value']/div/a[@class='phoneButton__button']").GetAttributeValue("data-full-phone-number", null) != null)
                    {
                        phone = offerParams.SelectSingleNode(".//ul[@class='parameters__rolled']/li/div[@class='parameters__value']/div/a[@class='phoneButton__button']").GetAttributeValue("data-full-phone-number", null).Replace("\\u002B", "+");
                    }
                    // imie i nazwisko
                    if (offer.DocumentNode.SelectSingleNode(".//div[@id='leftColumn']/div[@id='contactWrapper']/div[@id='contact_container']/div[@class='offerOwner']/div[@class='offerOwner__details']/h3[@class='offerOwner__person ']") != null)
                    {
                        nameSurname = offer.DocumentNode.SelectSingleNode(".//div[@id='leftColumn']/div[@id='contactWrapper']/div[@id='contact_container']/div[@class='offerOwner']/div[@class='offerOwner__details']/h3[@class='offerOwner__person ']").InnerText;
                    }
                    // lokalizacja, miasto
                    if (offerParams.SelectSingleNode(".//ul[@class='parameters__rolled']/li/b[@class='parameters__value']/a[@class='parameters__locationLink'][1]") != null)
                    {
                        localization = offerParams.SelectSingleNode(".//ul[@class='parameters__rolled']/li/b[@class='parameters__value']/a[@class='parameters__locationLink'][1]").InnerText;
                        if (localization != "")
                        {
                            city = (PolishCity)Enum.Parse(typeof(PolishCity), localization.ToUpper());
                        }
                    }
                    // dzielnica
                    int counter = offerParams.SelectNodes(".//ul[@class='parameters__rolled']/li/b[@class='parameters__value']/a[@class='parameters__locationLink']").Count;
                    if (counter == 3)
                    {
                        district = offerParams.SelectSingleNode(".//ul[@class='parameters__rolled']/li/b[@class='parameters__value']/a[@class='parameters__locationLink'][2]").InnerText;
                    }
                    //ulica
                    if (offer.DocumentNode.SelectSingleNode(".//div[@id='leftColumn']/script[contains(. , 'lokalizacja_ulica')]") != null)
                    {
                        // próbowałem enkodować na utf-8, znaki unicode
                        street = offer.DocumentNode.SelectSingleNode(".//div[@id='leftColumn']/script[contains(. , 'lokalizacja_ulica')]").InnerText;

                        int from = street.IndexOf("\"lokalizacja_ulica\":") + "\"lokalizacja_ulica\":".Length;
                        street = street.Substring(from);
                        street = street.Split(',')[0];

                    }
                    // liczba metrów kwadratowych
                    if (offerParams.SelectSingleNode(".//ul[@class='parameters__rolled']/li[contains(.//span, 'Powierzchnia w m2')]/b") != null)
                    {
                        areaM2 = offerParams.SelectSingleNode(".//ul[@class='parameters__rolled']/li[contains(.//span, 'Powierzchnia w m2')]/b").InnerText.Replace(" m2", "");
                        if (areaM2 != "")
                        {
                            areaM2Decimal = decimal.Parse(areaM2);
                        }
                    }
                    // liczba pokoi
                    if (offerParams.SelectSingleNode(".//ul[@class='parameters__rolled']/li[contains(.//span, 'Liczba pokoi')]/b") != null)
                    {
                        roomNumbers = offerParams.SelectSingleNode(".//ul[@class='parameters__rolled']/li[contains(.//span, 'Liczba pokoi')]/b").InnerText;
                        if (roomNumbers != "")
                        {
                            roomNumbersInt = Int32.Parse(roomNumbers);
                        }
                    }
                    // piętro
                    if (offerParams.SelectSingleNode(".//ul[@class='parameters__rolled']/li[contains(.//span, 'Piętro')]/b") != null)
                    {
                        floor = offerParams.SelectSingleNode(".//ul[@class='parameters__rolled']/li[contains(.//span, 'Piętro')]/b").InnerText;
                        if (floor != "")
                        {
                            if (floor == "parter")
                            {
                                floor = "0";
                            }
                            floorInt = Int32.Parse(floor);
                        }
                    }
                    // stan
                    if (offerParams.SelectSingleNode(".//ul[@class='parameters__rolled']/li[contains(.//span, 'Stan')]/b") != null)
                    {
                        stateOfEstate = offerParams.SelectSingleNode(".//ul[@class='parameters__rolled']/li[contains(.//span, 'Stan')]/b").InnerText;
                    }
                    // rok budowy
                    if (offerParams.SelectSingleNode(".//ul[@class='parameters__rolled']/li[contains(.//span, 'Rok budowy')]/b") != null)
                    {
                        constructionYear = offerParams.SelectSingleNode(".//ul[@class='parameters__rolled']/li[contains(.//span, 'Rok budowy')]/b").InnerText;
                        if (constructionYear != "")
                        {
                            constructionYearInt = Int32.Parse(constructionYear);
                        }
                    }
                    // czy ogłoszenie aktualne
                    if (offer.DocumentNode.SelectSingleNode("/html/body/div[@id='offer-card']/div[contains(@class, 'row') and contains(@class, 'collapse')]/div") == null)
                    {
                        validOffer = true;
                    }
                    else
                    {
                        validOffer = false;
                    }
                    // opis oferty
                    if (offer.DocumentNode.SelectSingleNode(".//div[@id='leftColumn']/div[@class='description']/div/section/div[contains(@class, 'description__rolled') and contains(@class, 'ql-container')]") != null)
                    {
                        rawDescription = offer.DocumentNode.SelectSingleNode(".//div[@id='leftColumn']/div[@class='description']//div[contains(@class, 'description__rolled') and contains(@class, 'ql-container')]").InnerText;
                    }


                    // dodanie do kolekcji nowej oferty
                    Entry singleOffer = new Entry
                    {
                        OfferDetails = new OfferDetails
                        {
                            Url = link,
                            CreationDateTime = DateTime.Parse(dateCreation),
                            LastUpdateDateTime = DateTime.Parse(dateUpdate),
                            OfferKind = offerKind,
                            SellerContact = new SellerContact
                            {
                                Telephone = phone,
                                Name = nameSurname,
                            },
                            IsStillValid = validOffer,
                        },

                        PropertyPrice = new PropertyPrice
                        {
                            TotalGrossPrice = fullPriceDecimal,
                            PricePerMeter = pricePerM2Decimal,
                            ResidentalRent = pricePerMonthDecimal,
                        },

                        PropertyDetails = new PropertyDetails
                        {
                            Area = areaM2Decimal, // metry kwadratowe
                            NumberOfRooms = roomNumbersInt, // liczba pokoi
                            FloorNumber = floorInt,
                            YearOfConstruction = constructionYearInt, // rok konstrukcji budynku
                        },

                        PropertyAddress = new PropertyAddress
                        {
                            City = city,
                            District = district,
                            StreetName = street,
                        },

                        PropertyFeatures = new PropertyFeatures
                        {
                            GardenArea = null,
                            Balconies = null,
                            BasementArea = null,
                            OutdoorParkingPlaces = null,
                            IndoorParkingPlaces = null,
                        },

                        RawDescription = rawDescription,
                    };
                    Entries.Add(singleOffer);
                }
            }
            return Entries;
        }

        public Dump GenerateDump()
        {

            var allParsedPages = GetHtml();


            return new Dump
            {
                DateTime = DateTime.Now,
                WebPage = WebPage,
                Entries = allParsedPages,
            };
        }
    }
}
