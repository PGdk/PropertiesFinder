using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Net;
using System.Text.RegularExpressions;

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
            Url = "https://www.stepien.nieruchomosci.pl/oferty/mieszkania/sprzedaz/",
            Name = "WebSite Integration",
            WebPageFeatures = new WebPageFeatures
            {
                HomeSale = true,
                HomeRental = false,
                HouseSale = false,
                HouseRental = false
            }
        };
    }

    public Dump GenerateDump()
    {
        //Tutaj w normalnej sytuacji musimy ściągnąć dane z konkretnej strony, przeparsować je i dopiero wtedy zapisać do modelu Dump
        var urls = GetFlatUrls();
        var baseUrl = "https://www.stepien.nieruchomosci.pl/";
        var entries = new List<Entry>();

        foreach (var url in urls)
        {
            var fullUrl = $"{baseUrl}{url}";
            var doc = GetHtmlDocument(fullUrl);
            if (doc == null) continue;
            try
            {
                entries.Add(CreateNewEntry(doc, fullUrl));
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        var dump = new Dump
        {
            DateTime = DateTime.Now,
            WebPage = WebPage,
            Entries = entries,
        };

        return dump;
    }

    private List<string> GetFlatUrls()
    {
        var urls = new List<string>();

        var doc = GetHtmlDocument(WebPage.Url);

        if (doc != null)
        {
            AppendUrlsToList(doc, urls);
        }

        int counter = 1;
        while (true)
        {
            doc = GetHtmlDocument($"{WebPage.Url}?page={counter}");

            if (doc == null) break;
            AppendUrlsToList(doc, urls);
            counter++;
        }

        return urls;
    }

    private HtmlDocument GetHtmlDocument(String url)
    {
        var htmlWeb = new HtmlWeb();
        var lastStatusCode = HttpStatusCode.OK;

        htmlWeb.PostResponse = (request, response) =>
        {
            if (response != null)
            {
                lastStatusCode = response.StatusCode;
            }
        };

        var doc = htmlWeb.Load(url);
        if (lastStatusCode == HttpStatusCode.OK)
        {
            return doc;
        }

        return null;
    }

    private void AppendUrlsToList(HtmlDocument doc, List<string> urls)
    {
        var hrefs = doc.DocumentNode.SelectNodes("//a[@class='overlay-link']");
        foreach (var href in hrefs)
        {
            urls.Add(href.GetAttributeValue("href", string.Empty));
        }
    }

    private Entry CreateNewEntry(HtmlDocument doc, String url)
    {

        var entry = new Entry
        {
            OfferDetails = new OfferDetails
            {
                Url = url,
                CreationDateTime = DateTime.Now,
                OfferKind = OfferKind.SALE,
                SellerContact = new SellerContact
                {
                    Email = "", // <-- nie dostepne, zamiast tego tel
                    Telephone = (string)GetProperty(doc, "//a[@class='agent-mobile']", "string", "")
                },
                IsStillValid = true
            },
            RawDescription = (string)GetProperty(doc, "//div[@class='offer-description']", "string", ""),
            PropertyPrice = new PropertyPrice
            {
                PricePerMeter = (decimal)GetProperty(doc, "//p[@class='params-short']/span[not(@class)]", "decimal", 0.0m),
                TotalGrossPrice = (decimal)GetProperty(doc, "//p[@class='cena']", "decimal", 0.0m),
                ResidentalRent = (decimal?)GetProperty(doc, "//div[@class='propsRow vir_oferta_czynszletni']/span[@class='propValue']", "decimal", null)
            },
            PropertyAddress = new PropertyAddress
            {
                City = (PolishCity)GetProperty(doc, "//h1[@class='location']", "polishcity", null),
                StreetName = "",//(string)getProperty(doc, "//div[@class='propsRow ulica']/span[@class='propValue']", "string", ""), <-- Nie da sie niestety znalesc pola z tymi atrybutami
                DetailedAddress = "",//(string)getProperty(doc, "//div[@class='propsRow afress']/span[@class='propValue']", "string", ""), <-- Czasami sa podane razem w jednym stringu z miastem
                District = ""//(string)getProperty(doc, "//div[@class='propsRow dzielnica']/span[@class='propValue']", "string", "")  <-- Niestety nie ma tam zadnej zasady, wiec przypisywanie tych danych
                // do konkretnych zmiennych nie mialo by sensu i bylo by zwyklym zgadywaniem.
            },
            PropertyDetails = new PropertyDetails
            {
                Area = (decimal)GetProperty(doc, "//div[@class='propsRow vir_oferta_powierzchnia']/span[@class='propValue']", "decimal", 0.0m),
                FloorNumber = (int?)GetProperty(doc, "//div[@class='propsRow vir_oferta_pietro']/span[@class='propValue']", "int", null),
                NumberOfRooms = (int)GetProperty(doc, "//div[@class='propsRow vir_oferta_iloscpokoi']/span[@class='propValue']", "int", 0),
                YearOfConstruction = (int?)GetProperty(doc, "//div[@class='propsRow vir_oferta_rokbudowy']/span[@class='propValue']", "int", null)
            },
            PropertyFeatures = new PropertyFeatures
            {
                Balconies = (int?)GetProperty(doc, "//div[@class='propsRow vir_oferta_iloscbalkonow']/span[@class='propValue']", "int", null),
                BasementArea = (decimal?)GetProperty(doc, "//div[@class='propsRow vir_oferta_piwnica_m2']/span[@class='propValue']", "decimal", null),
                GardenArea = (decimal?)GetProperty(doc, "//div[@class='propsRow vir_klientoferta_powierzchniadzialki']/span[@class='propValue']", "decimal", null),
                IndoorParkingPlaces = CheckParkingPlace(doc, "//div[@class='propsRow vir_oferta_garaz']/span[@class='propValue']"), // na zadej ofercie nie mogem znalesc takich informacji
                OutdoorParkingPlaces = null // to samo
            }

        };

        return entry;
    }

    private object GetProperty(HtmlDocument doc, string property, string datatype, object defaultValue)
    {
        var element = doc.DocumentNode.SelectSingleNode(property);
        if (element != null)
        {
            var text = element.InnerText;

            switch (datatype)
            {
                case "decimal":
                    {
                        var stripped = Regex.Replace(text, "[^0-9,]", "");
                        stripped  = stripped.Replace(',', '.');
                        decimal dectmp;
                        if(decimal.TryParse(stripped, out dectmp))
                        {
                            dectmp = decimal.Round(dectmp, 2);
                            return dectmp;
                        }

                        break;
                    }

                case "int":
                    {
                        var stripped = Regex.Replace(text, "[^0-9,]", "");
                        int inttmp;
                        if (int.TryParse(stripped, out inttmp))
                        {
                            return inttmp;
                        }

                        break;
                    }

                case "string":
                    {
                        return text;
                    }

                case "polishcity": {
                        text = text.ToUpper();
                        if(text.Contains(','))
                        {
                            var tmp = text.Split(',');
                            text = tmp[0];
                        }
                        text = text.Trim();
                        text = text.Replace(' ', '_');
                        text = ChangeToNonPolishChars(text);
                        PolishCity city;
                        if (Enum.TryParse(text, out city))
                        {
                            return city;
                        }
                        else
                        {
                            throw new Exception("Skipping this property, City not recognized");
                        }
                    }
            }
        }

        return defaultValue;
    }

    private string ChangeToNonPolishChars(String toChange)
    {
        var changed = toChange.Replace('Ą', 'A');
        changed = changed.Replace('Ć', 'C');
        changed = changed.Replace('Ę', 'E');
        changed = changed.Replace('Ł', 'L');
        changed = changed.Replace('Ó', 'O');
        changed = changed.Replace('Ń', 'N');
        changed = changed.Replace('Ś', 'S');
        changed = changed.Replace('Ż', 'Z');
        changed = changed.Replace('Ź', 'Z');

        return changed;
    }

    private int CheckParkingPlace(HtmlDocument doc, String property)
    {
        var element = doc.DocumentNode.SelectSingleNode(property);
        if (element != null)
        {
            var text = element.InnerText;

            if (text.ToLower() == "garaż")
            {
                return 1;
            }

            var stripped = Regex.Replace(text, "[^0-9,]", "");
            int inttmp;
            if (int.TryParse(stripped, out inttmp))
            {
                return inttmp;
            }
        }

        return 0;
    }
}
