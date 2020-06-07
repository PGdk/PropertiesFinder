using HtmlAgilityPack;
using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Morizon {
    public class MorizonIntegration : IWebSiteIntegration {
        public WebPage WebPage { get; }
        public IDumpsRepository DumpsRepository { get; }

        public IEqualityComparer<Entry> EntriesComparer { get; }

        public MorizonIntegration(IDumpsRepository dumpsRepository,
            IEqualityComparer<Entry> equalityComparer) {
            DumpsRepository = dumpsRepository;
            EntriesComparer = equalityComparer;
            WebPage = new WebPage {
                Url = "https://morizon.pl/",
                Name = "Morizon Integration",
                WebPageFeatures = new WebPageFeatures {
                    HomeSale = true,
                    HomeRental = true,
                    HouseSale = true,
                    HouseRental = true
                }
            };
        }

        public static Dictionary<string, int> monthsMapped = new Dictionary<string, int> {
                { "stycznia", 1},
                { "lutego", 2},
                { "marca", 3},
                { "kwietnia", 4},
                { "maja", 5},
                { "czerwca", 6},
                { "lipca", 7},
                { "sierpnia", 8},
                { "września", 9},
                { "października", 10},
                { "listopada", 11},
                { "grudnia", 12}
        };

        Dictionary<string, string> polishCharactersMapper = new Dictionary<string, string> {
                    { "Ą", "A" },
                    { "Ć", "C" },
                    { "Ę", "E" },
                    { "Ł", "L" },
                    { "Ń", "N" },
                    { "Ó", "O" },
                    { "Ś", "S" },
                    { "Ź", "Z" },
                    { "Ż", "Z" },
                    {" ", "_" }
        };

        public static DateTime GetDate(string date) {
            DateTime newDate;
            date = date.Trim();

            if ( date == "dzisiaj" ) {
                newDate = DateTime.Now;
            }
            else if ( date == "wczoraj" ) {
                newDate = DateTime.Now.AddDays(-1);
            }
            else {
                string[] parts = date.Split(null);
                int month = monthsMapped[parts[1]];
                newDate = new DateTime(int.Parse(parts[2]), month, int.Parse(parts[0]));
            }
            return newDate;
        }

        public static int GetFloor(string data) {
            string floor = data.Split("/")[0].Trim();
            return floor == "parter" ? 0 : int.Parse(floor);
        }

        public static decimal? ConvertDecimal(string number) {
            string sepdec = Convert.ToString(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

            try {
                if ( !number.Contains(",") )
                    return Decimal.Parse(number + sepdec + "00");

                return Decimal.Parse(number, NumberStyles.Any, new CultureInfo("pl-PL"));
            }
            catch ( FormatException ) {
                return null;
            }
        }

        public OfferDetails CreateOfferDetails(HtmlNode property, string propertyUrl) {
            string creationDate = property.SelectSingleNode("//*[text()[contains(., 'Opublikowano: ')]]").ParentNode.SelectSingleNode("td").InnerText;
            string updateDate = property.SelectSingleNode("//*[text()[contains(., 'Zaktualizowano: ')]]").ParentNode.SelectSingleNode("td").InnerText;

            string name = property.SelectSingleNode("//div[@class='companyName']")?.InnerText;
            if ( name == null ) {
                var data = property.SelectSingleNode("//div[@class='agentName']")?.InnerText;
                name = data != null ? data : "";
                name = name + "(osoba prywatna)";
            }

            SellerContact sellerContact;
            string telephone = property.SelectSingleNode("//span[@class='phone hidden']")?.InnerText;
            if ( telephone != null ) {
                sellerContact = new SellerContact {
                    Name = name,
                    Telephone = telephone
                };
            }
            else {
                sellerContact = new SellerContact {
                    Name = name
                };
            }

            OfferDetails offerDetails = new OfferDetails {
                Url = propertyUrl,
                CreationDateTime = GetDate(creationDate),
                LastUpdateDateTime = GetDate(updateDate),
                OfferKind = OfferKind.SALE,
                SellerContact = sellerContact,
                IsStillValid = true
            };
            return offerDetails;
        }

        public PropertyPrice CreatePropertyPrice(HtmlNode property) {
            string sepdec = Convert.ToString(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

            decimal price;
            string dataPrice = property.SelectSingleNode("//li[@class='paramIconPrice']/em")?.InnerText;
            if ( dataPrice != null ) {
                dataPrice = dataPrice.Split("&")[0];
                if ( !dataPrice.Contains(",") )
                    dataPrice = dataPrice +",00";
            }

            if ( !decimal.TryParse(dataPrice, NumberStyles.Any, new CultureInfo("pl-PL"), out price) ) {
                // These page does not contain information about price (need to ask a seller)
                price = -1;
            }

            decimal pricePerMeter;
            string dataPricePerMeter = property.SelectSingleNode("//li[@class='paramIconPriceM2']/em")?.InnerText;
            if ( dataPricePerMeter != null ) {
                dataPricePerMeter = dataPricePerMeter.Split("&")[0];
                if ( !dataPricePerMeter.Contains(",") )
                    dataPricePerMeter = dataPricePerMeter + ",00";;
            }

            if ( dataPricePerMeter == null || !decimal.TryParse(dataPricePerMeter, NumberStyles.Any, new CultureInfo("pl-PL"), out pricePerMeter) ) {
                // These page does not contain information about price per meter (need to ask a seller)
                pricePerMeter = -1;
            }

            // Morizon does not provide data about Residental Rent 
            PropertyPrice propertyPrice = new PropertyPrice {
                TotalGrossPrice = price,
                PricePerMeter = pricePerMeter,
                ResidentalRent = null
            };
            return propertyPrice;
        }

        public PropertyDetails CreatePropertyDetails(HtmlNode property) {

            string floorData = property.SelectSingleNode("//*[text()[contains(., 'Piętro: ')]]")?.ParentNode.SelectSingleNode("td")?.InnerText;
            int? floorNumber = null;
            if ( floorData != null )
                floorNumber = GetFloor(floorData);

            int numberOfRooms;
            string rooms = property.SelectSingleNode("//li[@class='paramIconNumberOfRooms']/em")?.InnerText;
            if ( rooms == null || !int.TryParse(rooms.Split("&")[0], out numberOfRooms) ) {
                numberOfRooms = -1;
            }

            string year = property.SelectSingleNode("//*[text()[contains(., 'Rok budowy: ')]]")?.ParentNode?.SelectSingleNode("td")?.InnerText;

            string sepdec = Convert.ToString(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            decimal area = -1;
            string areaData = property.SelectSingleNode("//li[@class='paramIconLivingArea']/em")?.InnerText.Split("&")[0];
            if ( areaData != null ) {
                if ( !areaData.Contains(",") )
                    area = decimal.Parse(areaData + sepdec + "00");
                else
                    area = decimal.Parse(areaData.Replace(",", sepdec));
            }

            PropertyDetails propertyDetails = new PropertyDetails {
                Area = area,
                NumberOfRooms = numberOfRooms,
                FloorNumber = floorNumber,
                YearOfConstruction = ( year == null ) ? (int?)null : int.Parse(year)
            };
            return propertyDetails;
        }

        public PropertyAddress CreatePropertyAddress(HtmlNode property) {
            string[] address = property.SelectSingleNode("//nav[@class='breadcrumbs']").InnerText.Split("\n");
            address = address.Where(val => val != "").ToArray();

            string city = polishCharactersMapper.Aggregate(address[3].Trim(null).ToUpper(), (current, value) =>
                    current.Replace(value.Key, value.Value));

            string district = address[address.Length - 2].Trim().ToLower();
            string streetData = address.Last().Trim().ToLower();

            if ( streetData.ToLower().StartsWith("ul.") )
                streetData = streetData.Substring(9, streetData.Length - 9).Trim().ToLower();

            string[] streetDataArray = streetData.Split("&nbsp;");

            string streetNumber;
            string[] street;
            if ( streetDataArray.Length > 1 && Regex.IsMatch(streetDataArray.Last(), @"^[0-9/]*$") ) {
                streetNumber = streetDataArray.Last();
                street = streetDataArray.Take(streetDataArray.Count() - 1).ToArray();
            }
            else {
                streetNumber = "";
                street = streetDataArray.ToArray();
            }

            PropertyAddress propertyAddress;
            try {
                propertyAddress = new PropertyAddress {
                    City = (PolishCity)Enum.Parse(typeof(PolishCity), city),
                    District = district,
                    StreetName = string.Join(" ", street),
                    DetailedAddress = streetNumber
                };
            }
            catch ( ArgumentException ) {
                propertyAddress = new PropertyAddress {
                    District = district,
                    StreetName = string.Join(" ", street),
                    DetailedAddress = streetNumber
                };
            }

            return propertyAddress;
        }

        public static decimal? GetGardenArea(HtmlNode property, string description, string facilities) {

            decimal? gardenArea = Decimal.Zero;
            description = description.Replace("&oacute;", "ó");
            string gardenData = property.SelectSingleNode("//*[text()[contains(., 'Powierzchnia ogródka: ')]]")?.ParentNode?.SelectSingleNode("td")?.InnerText;
            if ( gardenData != null ) {
                if ( gardenData.Split(" ")[0] == "\n0" )
                    return Decimal.Zero;
                return ConvertDecimal(gardenData.Split(" ")[0]);
            }
            else {
                if ( facilities.Contains("ogród") || description.Contains("ogród") ) {
                    gardenArea = null;
                    string searchTerm = "ogród";
                    string value;
                    var indexes = Regex.Matches(description, "ogród").Cast<Match>().Select(m => m.Index).ToList();

                    foreach ( int i in indexes ) {
                        if ( i >= 0 ) {
                            string temp = description.Substring(i + searchTerm.Length).Trim();
                            string[] parts = temp.Split(' ');

                            // if parts[0] equals "ek" it means "ogródek" was found
                            value = parts[0] == "ek" ? parts[1] : parts[0];
                            value = Regex.Replace(value, @"[^0-9,]+", "");
                            if ( value.Length > 0 ) {
                                // for example "ogród (50 m2)"
                                return ConvertDecimal(value);
                            }

                            if ( parts[0] == "o" && parts[1].StartsWith("pow") || parts[1] == "o" && parts[2].StartsWith("pow") ) {
                                value = parts[0] == "ek" ? parts[3] : parts[2];
                                value = Regex.Replace(value, @"[^0-9,]+", "");
                                if ( value.Length > 0 ) {
                                    // for example "ogród o pow. 50 m2"
                                    return ConvertDecimal(value);
                                }
                            }
                        }
                    }
                }
            }
            return gardenArea;
        }

        public static decimal? GetBasementArea(HtmlNode property, string description, string facilities) {
            decimal? basementArea = Decimal.Zero;
            description = description.Replace("&oacute;", "ó");

            if ( facilities.Contains("piwnica") ||
                description.Contains("piwnica") ||
                description.Contains("komórka lokatorska") ||
                description.Contains("komórki  lokatorskie") ) {
                basementArea = null;


                List<string> keys = new List<string> { "piwnica", "komórka lokatorka", "komórki lokatorskie" };
                string value;
                foreach ( string key in keys ) {
                    var indexes = Regex.Matches(description, key).Cast<Match>().Select(m => m.Index).ToList();

                    foreach ( int i in indexes ) {
                        if ( i >= 0 ) {
                            string temp = description.Substring(i + key.Length).Trim();
                            string[] parts = temp.Split(' ');

                            value = Regex.Replace(parts[0], @"[^0-9,]+", "");
                            if ( value.Length > 0 ) {
                                // for example "piwnica (50 m2)"
                                return ConvertDecimal(value);
                            }

                            if ( parts[0] == "o" && parts[1].StartsWith("pow") ) {
                                value = Regex.Replace(parts[2], @"[^0-9,]+", "");
                                if ( value.Length > 0 ) {
                                    // for example "piwnica o pow. 50 m2"
                                    return ConvertDecimal(value);
                                }
                            }
                        }
                    }
                }
            }
            return basementArea;
        }

        public PropertyFeatures CreatePropertyFeatures(HtmlNode property, string description) {
            var facilities = property.SelectSingleNode("//*[text()[contains(., 'Udogodnienia')]]")?.NextSibling?.NextSibling?.InnerText;
            // When page does not contain information about facilities it means that property does not have it => value 0
            // When page contains information, but we can't get exact quantity => value null

            if ( facilities == null )
                facilities = "";

            decimal? gardenArea = GetGardenArea(property, description, facilities);

            int? balconies = 0;
            if ( facilities.Contains("balkon") || description.Contains("balkon") ) {
                balconies = null;
            }

            int? outdoorParkingPlaces = 0;
            if ( facilities.Contains("miejsce parkingowe") || description.Contains("parking") )
                outdoorParkingPlaces = null;

            int? indoorParkingPlaces = 0;
            if ( facilities.Contains("parking podziemny") ||
                facilities.Contains("garaż") ||
                description.Contains("parking podziemny") ||
                description.Contains("garaż") )
                indoorParkingPlaces = null;

            decimal? basementArea = GetBasementArea(property, description, facilities);

            PropertyFeatures propertyFeatures = new PropertyFeatures {
                GardenArea = gardenArea,
                Balconies = balconies,
                BasementArea = basementArea,
                OutdoorParkingPlaces = outdoorParkingPlaces,
                IndoorParkingPlaces = indoorParkingPlaces,
            };

            return propertyFeatures;
        }

        public List<Entry> GetEntries(string url, string urlPath, List<Entry> entries) {
            var web = new HtmlWeb();
            var doc = web.Load(url + urlPath);

            string nextUrl = doc.DocumentNode.SelectSingleNode(".//*[contains(@title,'następna strona')]")?.Attributes["href"]?.Value;

            List<string> propertiesUrls = new List<string>();

            foreach ( HtmlAgilityPack.HtmlNode node in doc.DocumentNode.SelectNodes("//a[@class='property_link property-url']") ) {
                propertiesUrls.Add(node.Attributes["href"].Value);
            }

            foreach ( string propertyUrl in propertiesUrls ) {
                var property = web.Load(propertyUrl).DocumentNode;

                var description = property.SelectSingleNode("//div[@class='description']")?.InnerText;
                description = description != null ? description : "";

                Entry propertyOffer = new Entry {
                    OfferDetails = CreateOfferDetails(property, propertyUrl),
                    PropertyPrice = CreatePropertyPrice(property),
                    PropertyDetails = CreatePropertyDetails(property),
                    PropertyAddress = CreatePropertyAddress(property),
                    PropertyFeatures = CreatePropertyFeatures(property, description),
                    RawDescription = description
                };

                entries.Add(propertyOffer);
            }

            if ( nextUrl != null )
                return GetEntries(url, nextUrl, entries);

            return entries;
        }

        public List<Entry> GetPage(int number) {
            var web = new HtmlWeb();
            var doc = web.Load("https://www.morizon.pl/mieszkania/" + number);

            List<string> propertiesUrls = new List<string>();
            List<Entry> entries = new List<Entry>();

            foreach ( HtmlAgilityPack.HtmlNode node in doc.DocumentNode.SelectNodes("//a[@class='property_link property-url']") ) {
                propertiesUrls.Add(node.Attributes["href"].Value);
            }

            foreach ( string propertyUrl in propertiesUrls ) {
                var property = web.Load(propertyUrl).DocumentNode;

                var description = property.SelectSingleNode("//div[@class='description']")?.InnerText;
                description = description != null ? description : "";

                Entry propertyOffer = new Entry {
                    OfferDetails = CreateOfferDetails(property, propertyUrl),
                    PropertyPrice = CreatePropertyPrice(property),
                    PropertyDetails = CreatePropertyDetails(property),
                    PropertyAddress = CreatePropertyAddress(property),
                    PropertyFeatures = CreatePropertyFeatures(property, description),
                    RawDescription = description
                };

                entries.Add(propertyOffer);
            }

            return entries;
        }


        public Dump GenerateDump() {
            
            List<Entry> entries = new List<Entry>();
            var url = "https://www.morizon.pl";

            entries = GetEntries(url, "/mieszkania", entries);

            List<Entry> uniqueEntries = entries.Distinct(new MorizonComparer()).ToList();
            
            return new Dump {
                DateTime = DateTime.Now,
                WebPage = WebPage,
                Entries = uniqueEntries
            };
        }
    }
}
