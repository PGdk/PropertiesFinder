using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Extensions;
using HtmlAgilityPack;
using Models;
using Utilities;

namespace Implementation
{
    public class HtmlDocumentToEntryParser : IParser<HtmlDocument, Entry>
    {
        private static readonly Regex UrlRegex = new Regex(@"(?<url>https://[a-zA-Z0-9~_.,/-]+)");
        private static readonly Regex PriceRegex = new Regex(@"Cena: (?<price>[0-9 ]+|inf\. u dewelopera)");
        private static readonly List<string> OfferTypes = new List<string> {"pokoje", "mieszkania", "domy"};

        public Entry Parse(HtmlDocument source)
        {
            var documentNode = source.DocumentNode;
            if (IsValidOffer(documentNode))
            {
                return new Entry
                {
                    PropertyPrice = CreatePropertyPrice(documentNode),
                    PropertyAddress = PropertyAddress(documentNode),
                    OfferDetails = CreateOfferDetail(documentNode),
                    PropertyDetails = CreatePropertyDetails(documentNode),
                    PropertyFeatures = CreatePropertyFeatures(documentNode),
                    RawDescription = CreateDescription(documentNode)
                };
            }

            return null;
        }

        private PropertyFeatures CreatePropertyFeatures(HtmlNode htmlNode)
        {
            var propertyFeatures = new PropertyFeatures
            {
                HasElevator = GetCustomOfferDetailValue(htmlNode, "Winda:").StringToBool("tak", "nie"),
                HasBasementArea = GetCustomOfferDetailValue(htmlNode, "Piwnica:").StringToBool("tak", "nie"),
                HasBalcony = GetCustomOfferDetailValue(htmlNode, "Balkon:").StringToBool("Tak", "Nie"),
                BalconyArea = GetCustomOfferDetailValue(htmlNode, "Powierzchnia tarasu:").FindIntegerNullable(),
                GardenArea = GetCustomOfferDetailValue(htmlNode, "Powierzchnia ogródka").FindIntegerNullable(),
                ParkingPlaces = GetCustomOfferDetailValue(htmlNode, "Miejsca parkingowe:").FindIntegerNullable(),
                IsPrimaryMarket = GetCustomOfferDetailValue(htmlNode, "Rynek pierwotny:")
                    .StringToNullableBool("Tak", "Nie")
            };
            return propertyFeatures;
        }

        public bool? IsPrimaryMarket { get; set; }

        private PropertyAddress PropertyAddress(HtmlNode htmlNode)
        {
            var title = htmlNode
                .SelectSingleNode("//div[@class='header']/h1")
                .InnerText;

            var addressElements = title.Split(',')
                .Select(x => x
                    .Replace("(gm.)", string.Empty)
                    .ToUpper(CultureInfo.CurrentCulture)
                    .RemoveDiacritics()
                    .Trim()
                    .Replace("-", "_")
                    .Replace(" ", "_"));

            foreach (var addressElement in addressElements)
            {
                if (Enum.IsDefined(typeof(PolishCity), addressElement))
                {
                    return new PropertyAddress
                    {
                        City = Enum.Parse<PolishCity>(addressElement),
                        StreetName = title
                            .Split(',')
                            .Last()
                            .RemoveWhitespaces(),
                        District = GetCustomOfferDetailValue(htmlNode, "Województwo :\n")
                            .RemoveWhitespaces()
                    };
                }
            }

            return null;
        }

        private bool IsValidOffer(HtmlNode htmlNode)
        {
            if (htmlNode == null) return false;

            var hasPrice = htmlNode.SelectSingleNode("//div[@class='header']/h3") != null;

            var isForeignOffer = htmlNode
                                     .SelectSingleNode(@"//span[@class='propertyName' or @class='propertyCompanyName']")
                                     ?.InnerText?.Equals("Współpraca zagraniczna") ??
                                 false;

            var hasPhoneNumber = htmlNode.SelectSingleNode("//span[@class='visible-contact']") != null;

            return hasPrice && !isForeignOffer && hasPhoneNumber;
        }

        private PropertyDetails CreatePropertyDetails(HtmlNode htmlNode)
        {
            var propertyDetails = new PropertyDetails
            {
                Area = GetCustomOfferDetailValue(htmlNode, "Powierzchnia użytkowa:").FindDecimal(),
                NumberOfRooms = GetCustomOfferDetailValue(htmlNode, "Liczba pokoi:").FindInteger(),
                BuldingType = GetCustomOfferDetailValue(htmlNode, "Typ budynku:").RemoveWhitespaces(),
                NumberOfFloors = GetCustomOfferDetailValue(htmlNode, "Liczba pięter:").FindInteger(1),
                YearOfConstruction = GetCustomOfferDetailValue(htmlNode, "Rok budowy:").FindIntegerNullable(),
                // FloorNumber = GetCustomOfferDetailValue(htmlNode, "Piętro:").FindIntegerNullable()
            };

            var floorNumberText = GetCustomOfferDetailValue(htmlNode, "Piętro:");

            if (!String.IsNullOrWhiteSpace(floorNumberText))
            {
                propertyDetails.FloorNumber = floorNumberText.Contains("parter") ? 0 : floorNumberText.FindInteger();
            }

            return propertyDetails;
        }

        private static string CreateDescription(HtmlNode htmlNode)
        {
            return htmlNode
                       .SelectSingleNode("//div[@id='description']")?.InnerText ?? String.Empty;
        }

        private PropertyPrice CreatePropertyPrice(HtmlNode htmlNode)
        {
            var propertyPrice = new PropertyPrice();

            var priceNode = htmlNode.SelectSingleNode("//div[@class='header']/h3").InnerText;
            var priceText = PriceRegex.Match(priceNode).Groups["price"].Value;

            if (priceText.Contains("inf. u dewelopera"))
            {
                propertyPrice.NegotiablePrice = true;
            }
            else
            {
                propertyPrice.TotalGrossPrice = priceText.RemoveWhitespaces().FindDecimal();
            }

            propertyPrice.PricePerMeter = GetCustomOfferDetailValue(htmlNode, "Cena za m").FindDecimal();
            // propertyPrice.ResidentalRent = GetCustomOfferDetailValue(htmlNode,); // todo

            return propertyPrice;
        }

        private OfferDetails CreateOfferDetail(HtmlNode htmlNode)
        {
            var sellerName = htmlNode
                .SelectSingleNode(@"//span[@class='propertyName' or @class='propertyCompanyName']")
                .InnerText;

            var offerType = htmlNode
                .SelectSingleNode(@"//div[@class='header']/span")
                .InnerText;

            OfferKind offerKind;

            if (offerType.Contains("wynajęcia")) offerKind = OfferKind.RENTAL;
            else offerKind = OfferKind.SALE;


            var sellerTelephone = htmlNode
                .SelectSingleNode("//span[@class='visible-contact']")
                .InnerText
                .RemoveWhitespaces();

            var lastUpdateDateTime = htmlNode
                .SelectNodes("//div[@class='baseParam']/div")
                .FirstOrDefault(node => node.Element("b")?.InnerText == "Data aktualizacji:")
                .InnerText
                .FindDate();

            var creationDateTime = htmlNode
                .SelectNodes("//div[@class='baseParam']/div")
                .FirstOrDefault(node => node.Element("b")?.InnerText == "Data dodania:")
                ?.InnerText.FindDate() ?? lastUpdateDateTime;


            var urlShort = htmlNode
                .SelectNodes("//div[@class='baseParam']/div")
                .FirstOrDefault(node => node.Element("b")?.InnerText == "Link do oferty:")
                ?.InnerText;

            return new OfferDetails
            {
                CreationDateTime = creationDateTime,
                LastUpdateDateTime = lastUpdateDateTime,
                IsStillValid = true,
                Url = UrlRegex.Match(urlShort).Groups["url"].Value,
                OfferKind = offerKind,
                SellerContact = new SellerContact
                {
                    Name = sellerName,
                    Telephone = sellerTelephone
                }
            };
        }

        private string GetCustomOfferDetailValue(HtmlNode htmlNode, string name)
        {
            var nameNodes = htmlNode.SelectNodes(@"//*[@id=""propertyLeft""]/div[3]/dl/dt");
            var nameNode = nameNodes.FirstOrDefault(node => node.InnerText.StartsWith(name));

            if (nameNode == null) return null;

            var valueNode = htmlNode
                .SelectNodes(@"//*[@id=""propertyLeft""]/div[3]/dl/dd")
                .ElementAt(nameNodes.IndexOf(nameNode));

            return valueNode.InnerText;
        }
    }
}