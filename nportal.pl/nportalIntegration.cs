using HtmlAgilityPack;
using Interfaces;
using Models;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using ScrapySharp.Extensions;
using WebPage = Models.WebPage;

namespace nportal.pl
{
    public class NportalIntegration : IWebSiteIntegration
    {

        public WebPage WebPage { get; }
        public IDumpsRepository DumpsRepository { get; }
        public IEqualityComparer<Entry> EntriesComparer { get; }

        public NportalIntegration(IDumpsRepository dumpsRepository,
            IEqualityComparer<Entry> equalityComparer)
        {
            DumpsRepository = dumpsRepository;
            EntriesComparer = equalityComparer;
            WebPage = new WebPage
            {
                Name = "nportal.pl Integration",
                Url = "http://www.nportal.pl",
                WebPageFeatures = new WebPageFeatures
                {
                    HomeSale = true,
                    HomeRental = true,
                    HouseRental = true,
                    HouseSale = true
                }
            };
        }
        public Dump GenerateDump()
        {
            var dump = new Dump
            {
                DateTime = DateTime.Now,
                WebPage = WebPage
            };
            var dumpEntries = new List<Entry>();
            dumpEntries.AddRange(GetEntries(OfferKind.RENTAL, "http://nportal.pl/wynajem/mieszkan/"));
            dumpEntries.AddRange(GetEntries(OfferKind.SALE, "http://nportal.pl/mieszkania/"));
            dump.Entries = dumpEntries;
            return dump;
        }

        private IEnumerable<Entry> GetEntries(OfferKind offerKind, string baseUri)
        {
            var pageCounter = 1;
            var offerCounter = 0;
            var totalPages = 2;
            var entries = new List<Entry>();
            var browser = new ScrapingBrowser();

            while (pageCounter <= totalPages)
            {
                var homepage = browser.NavigateToPage(new Uri($"{baseUri}?page={pageCounter}"));

                // first init
                if (totalPages.Equals(int.MaxValue))
                {
                    var totalPagesString = homepage.Html.CssSelect("#listContainer>header>div.sortBar.grad_g1>div.f-r>div>span")
                        .Last().InnerText.Split(' ')[2];
                    totalPages = int.Parse(totalPagesString);
                }

                var htmlNodesWithOffers =
                    homepage.Html.SelectNodes(@"/html/body/div[2]/div[3]/div[3]/section/div/div[2]/div[1]/h2/a");

                foreach (var htmlNode in htmlNodesWithOffers)
                {
                    var offerUrl = htmlNode.Attributes["href"].Value;
                    Console.WriteLine($"Page\t{pageCounter}/{totalPages}:\tOffer: {++offerCounter}:\t" + offerUrl);

                    var offerAsHtml = browser.NavigateToPage(new Uri(offerUrl)).Html;

                    var htmlTableWithFeatures =
                        offerAsHtml.SelectNodes("/html/body/div[2]/div[3]/section[1]/div/section[3]/table/tr");
                    var offerFeatures = FindOfferFeatures(htmlTableWithFeatures);
                    var sellerContact = FindSellerContact(offerAsHtml);

                    offerFeatures.Add("URL", offerUrl);
                    offerFeatures.Add("Ostatnia modyfikacja",
                        offerAsHtml.SelectSingleNode("/html/body/div[2]/div[3]/section[1]/div/div[3]").InnerText);
                    offerFeatures.Add("Opis", offerAsHtml.CssSelect("#description").First().InnerText.Trim());
                    offerFeatures.Add("Adres", offerAsHtml.CssSelect("#location").First().InnerText.Trim());
                    Console.WriteLine($"\t\t\t{offerFeatures["Opis"].Substring(0, 80)}");
                    entries.Add(new Entry
                    {
                        OfferDetails = GetOfferDetails(offerFeatures, sellerContact, offerKind),
                        PropertyAddress = GetPropertyAddress(offerFeatures),
                        PropertyDetails = GetPropertyDetails(offerFeatures),
                        PropertyFeatures = GetPropertyFeatures(offerFeatures),
                        PropertyPrice = GetPropertyPrice(offerFeatures),
                        RawDescription = offerFeatures["Opis"]
                    });
                }

                pageCounter++;
            }

            return entries;
        }

        // Seller is either company or agent, so we have if else thru offerFeatures.
        // ! No e-mail on website, just phone
        public SellerContact FindSellerContact(HtmlNode htmlOffer)
        {
            string name;
            var nameAsHtml = htmlOffer.CssSelect("#sidebar>div>section>div.agentInfo>div.agentName").FirstOrDefault();
            if (nameAsHtml != null)
                name = nameAsHtml.InnerText.Trim();
            else
            {
                nameAsHtml = htmlOffer.CssSelect("#sidebar>div>section>div.companyInfo>a").First();
                name = nameAsHtml.Attributes["href"].Value;
                name = string.Join(' ', name.Split('/')[4].Split('-').SkipLast(1).ToArray().Select(w => w.UpperCaseFirstLetter()));
            }
            string telephone;
            var telephoneAsHtml = htmlOffer
                .CssSelect("#sidebar>div>section>div.agentInfo>div.ownerContactInformation>div>div>div>span.hidden")
                .FirstOrDefault();
            if (telephoneAsHtml != null)
                telephone = telephoneAsHtml.InnerText.Trim();
            else
            {
                telephone = htmlOffer
                    .CssSelect("#sidebar>div>section>div.companyInfo>div.phone.BusinessContactHidden>span.hidden")
                    .FirstOrDefault()
                    ?.InnerHtml.Trim();
            }
            return new SellerContact
            {
                Email = "",
                Name = name,
                Telephone = telephone
            };
        }

        // there's only Last Modified date, no Creation
        // All offers are valid
        private OfferDetails GetOfferDetails(Dictionary<string, string> offerFeatures, SellerContact seller, OfferKind offerKind)
        {
            var url = offerFeatures["URL"];
            var creationDateTime = offerFeatures["Ostatnia modyfikacja"].ToDateTime();
            return new OfferDetails
            {
                Url = url,
                SellerContact = seller,
                CreationDateTime = creationDateTime,
                LastUpdateDateTime = creationDateTime,
                IsStillValid = true,
                OfferKind = offerKind
            };
        }

        // Addresses are inconsistent - sometimes it's 'Warsaw, Praga', sometimes 'Warsaw Praga', or 'Warsaw City, Warsaw, Praga'.
        // ! That's why I defaulted City value to PolishCity.ALEKSANDROW_KUJAWSKI, because City can't be null or undefined
        public PropertyAddress GetPropertyAddress(Dictionary<string, string> offerFeatures)
        {
            var address = offerFeatures["Adres"].Split(',');
            if (!Enum.TryParse(address[0].RemoveDiacritics(), true, out PolishCity city))
            {
                city = PolishCity.ALEKSANDROW_KUJAWSKI;
            }

            var district = address.Length > 1 ? address[1] : "";
            var streetName = address.Length > 2 ? address[2] : "";
            Console.WriteLine($"\t\t\t{city}\t{district}\t{streetName}");
            return new PropertyAddress
            {
                City = city,
                District = district,
                StreetName = streetName
            };
        }

        // Place for special parsing rules. I though it will be more common and built Flag mechanism for it, but at the end found just one use-case for that,
        // but I didn't want to remove working piece of code
        [Flags]
        enum ParsingRule
        {
            None,
            GroundFloor // ground floor is not '0', but 'parter'
        };
        int? FindFeatureKey(IReadOnlyDictionary<string, string> features, string featureKey, ParsingRule parsingRules = ParsingRule.None)
        {
            if (!features.ContainsKey(featureKey)) return null;
            if ((parsingRules & ParsingRule.GroundFloor) == ParsingRule.GroundFloor)
            {
                if (features[featureKey] == "parter")
                    return 0;
            }

            return int.Parse(features[featureKey]);
        }

        public PropertyDetails GetPropertyDetails(Dictionary<string, string> offerFeatures)
        {
            var area = decimal.Parse(offerFeatures["Powierzchnia mieszkalna"].Replace("m²", "").Replace(",", "."));
            var floorNumber = FindFeatureKey(offerFeatures, "Piętro", ParsingRule.GroundFloor);
            var numberOfRooms = FindFeatureKey(offerFeatures, "Liczba pokoi").GetValueOrDefault(0);
            var yearOfConstruction = FindFeatureKey(offerFeatures, "Rok budowy");


            Console.WriteLine($"\t\t\tArea: {area}\tFloor number: {floorNumber}\tRooms: {numberOfRooms}\tYear: {yearOfConstruction}");
            return new PropertyDetails
            {
                Area = area,
                FloorNumber = floorNumber,
                NumberOfRooms = numberOfRooms,
                YearOfConstruction = yearOfConstruction
            };
        }

        // nPortal stores balcony/basement data as a boolean - Yes/No, and model requires these values to specify count/area, not presence, so I set it to 1.
        private PropertyFeatures GetPropertyFeatures(Dictionary<string, string> offerFeatures)
        {
            int? balconies = null;
            if (offerFeatures.ContainsKey("Balkon"))
            {
                balconies = 1;
            }
            decimal? basementArea = null;
            if (offerFeatures.ContainsKey("Piwnica"))
            {
                basementArea = 1;
            }

            Console.WriteLine($"\t\t\tBalconies: {balconies}\tBasement:{basementArea}");
            return new PropertyFeatures
            {
                Balconies = balconies,
                BasementArea = basementArea,
                GardenArea = null,
                IndoorParkingPlaces = null,
                OutdoorParkingPlaces = null
            };
            //throw new NotImplementedException();
        }

        // Sometimes there's no price info and types can't be null, so I defaulted it to 0.
        // ! Also there's no rent info.
        public PropertyPrice GetPropertyPrice(Dictionary<string, string> offerFeatures)
        {
            var pricePerMeterAsHtml = "0";
            if (offerFeatures.ContainsKey("Cena za m&sup2;"))
                pricePerMeterAsHtml = string.Join(string.Empty, offerFeatures["Cena za m&sup2;"].Split(' ').SkipLast(1)).Replace(',', '.');
            var pricePerMeter = decimal.Parse(pricePerMeterAsHtml);

            decimal totalGrossPrice = 0;
            if (!offerFeatures["Cena"].Equals("zapytaj o cenę"))
                totalGrossPrice = decimal.Parse(offerFeatures["Cena"].Split('&')[0].Replace(" ", string.Empty)); // "Cena"
            Console.WriteLine($"\t\t\tPrice: {totalGrossPrice}\tPrice per meter: {pricePerMeter}");
            return new PropertyPrice
            {
                PricePerMeter = pricePerMeter,
                ResidentalRent = null,
                TotalGrossPrice = totalGrossPrice
            };
        }
        public Dictionary<string, string> FindOfferFeatures(HtmlNodeCollection htmlTableWithFeatures)
        {
            var features = new Dictionary<string, string>();
            foreach (var htmlRow in htmlTableWithFeatures)
            {
                foreach (var htmlHeader in htmlRow.SelectNodes("th"))
                {
                    var header = htmlHeader.InnerText.Trim().TrimEnd(':');
                    if (string.IsNullOrEmpty(header))
                        continue;
                    var htmlData = htmlHeader.NextSibling.NextSibling;
                    var data = htmlData.InnerText.Trim();
                    features.Add(header, data);
                }
            }

            return features;
        }


    }
}
