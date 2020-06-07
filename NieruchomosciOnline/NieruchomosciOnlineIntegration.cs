namespace NieruchomosciOnline
{
    using HtmlAgilityPack;
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class NieruchomosciOnlineIntegration : IWebSiteIntegration
    {
        private readonly HtmlWeb htmlWeb = new HtmlWeb();
        private readonly List<string> allowedOfferTypes = new List<string> 
        { "kawalerka do wynajęcia", "mieszkanie do wynajęcia", "dom na sprzedaż", "mieszkanie na sprzedaż", "kawalerka na sprzedaż", "dom do wynajęcia",
          "mały dom do wynajęcia", "mały dom na sprzedaż"};

        public WebPage WebPage { get; }

        public IDumpsRepository DumpsRepository { get; }

        public IEqualityComparer<Entry> EntriesComparer { get; }

        public int MaximumOffersPerDump { get; set; } = 500;

        public int DumpStartingPage { get; set; } = 1;

        public int? LastDumpFinishedPage { get; set; }

        public NieruchomosciOnlineIntegration(IDumpsRepository dumpsRepository,
            IEqualityComparer<Entry> equalityComparer)
        {
            DumpsRepository = dumpsRepository;
            EntriesComparer = equalityComparer;
            WebPage = new WebPage
            {
                Url = "https://nieruchomosci-online.pl/",
                Name = "Nieruchomosci-online",
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

            var nextPageExists = true;
            var page = this.DumpStartingPage;

            while (nextPageExists && entries.Count < this.MaximumOffersPerDump)
            {
                var result = this.GetEntriesFromPage(page++, out nextPageExists);
                entries.AddRange(result);
            }
            this.LastDumpFinishedPage = page;

            return new Dump()
            {
                Entries = entries,
                DateTime = DateTime.Now,
                WebPage = this.WebPage
            };
        }

        public List<Entry> GetEntriesFromPage(int page, out bool nextPageExists)
        {
            var result = new List<Entry>();
            var urlList = this.GetOffersFromPage(page, out nextPageExists);
            foreach (var url in urlList)
            {
                if (this.TryToParseOffer(url, out Entry entry))
                {
                    result.Add(entry);
                }
            }

            return result;
        }

        public List<Entry> GetEntriesFromPage(int page)
        {
            return this.GetEntriesFromPage(page, out bool tmp);
        }

        /// <summary>
        /// Gets offers urls from given page.
        /// </summary>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="nextPageExists">Information whether next page exists or not.</param>
        /// <returns>Url lists.</returns>
        private List<string> GetOffersFromPage(int pageNumber, out bool nextPageExists)
        {
            var url = $"https://www.nieruchomosci-online.pl/szukaj.html?p={pageNumber}";

            var hrefList = new List<string>();
            var document = this.htmlWeb.Load(url);
            nextPageExists = document.DocumentNode.Descendants("em").Any(n => n.GetAttributeValue("class", string.Empty) == "nextLabel");

            // Czasem jak strona nie istnieje przenosi na pierwszą
            if(pageNumber != 1 
                && document
                .DocumentNode
                .Descendants("ul")
                .FirstOrDefault(u => u.GetAttributeValue("class", string.Empty) == "pagination-mob-sub")
                ?.Descendants("span")
                .FirstOrDefault(u => u.GetAttributeValue("class", string.Empty) == "active")
                ?.InnerText=="1")
            {
                return hrefList;
            }

            var links = document.DocumentNode.Descendants("a");

            foreach(var l in links)
            {
                if(l.GetAttributeValue("id", string.Empty).Contains("tertiary-name_") && !l.ParentNode.ParentNode.ParentNode.ParentNode.GetAttributeValue("class", string.Empty).Contains("tile-investment"))
                {
                    var hrefValue = l.GetAttributeValue("href", string.Empty);
                    if (hrefValue != string.Empty)
                    {
                        hrefList.Add(hrefValue);
                    }
                }
            }

            return hrefList;
        }

        /// <summary>
        /// Tries to parse offer
        /// </summary>
        /// <param name="url">Offers url.</param>
        /// <param name="entry">Offer entry.</param>
        /// <returns>If operation suceeded.</returns>
        private bool TryToParseOffer(string url, out Entry entry)
        {
            entry = new Entry()
            {
                OfferDetails = new OfferDetails()
                {
                    Url = url,
                    IsStillValid = true,
                    SellerContact = new SellerContact()
                },
                PropertyAddress = new PropertyAddress(),
                PropertyDetails = new PropertyDetails(),
                PropertyFeatures = new PropertyFeatures(),
                PropertyPrice = new PropertyPrice()
            };

            var document = this.htmlWeb.Load(url);
            var offerType = this.GetOfferType(document);
            if (offerType == null || !this.allowedOfferTypes.Contains(offerType))
            {
                entry = null;
                return false;
            }

            if (offerType.EndsWith("na sprzedaż"))
            {
                entry.OfferDetails.OfferKind = OfferKind.SALE;
            }
            else if (offerType.EndsWith("do wynajecia"))
            {
                entry.OfferDetails.OfferKind = OfferKind.RENTAL;
            }

            this.SetContactInfo(document, entry);
            this.SetLastUpdateTime(document, entry);
            this.SetPriceAndAddressData(document, entry);
            this.SetPropertyInfo(document, entry);
            this.SetRawDescription(document, entry);
            this.TryToLoadAdditionalInfoFromOfferDetails(document, entry);
            this.TryToLoadAdditionalInfoFromDescription(entry);
            this.AssumeNoFeatureIfNotMentioned(entry, offerType);

            return true;
        }

        /// <summary>
        /// Gets offert type if can be found and null if can't.
        /// </summary>
        /// <param name="document">Offer html document.</param>
        /// <returns>Offer type.</returns>
        private string GetOfferType(HtmlDocument document)
        {
            var oferta = document.DocumentNode.Descendants("li").FirstOrDefault(li =>
            {
                var st = li.Descendants("strong").FirstOrDefault();
                return st != null && st.InnerText == "Typ oferty:";
            });

            if (oferta == null || oferta.Descendants("span").FirstOrDefault() == null)
            {
                return null;
            }
            else
            {
                return oferta.Descendants("span").First().InnerText;
            }
        }

        /// <summary>
        /// Sets last update time.
        /// </summary>
        /// <param name="document">Offer html document.</param>
        /// <param name="entry">Offer entry.</param>
        private void SetLastUpdateTime(HtmlDocument document, Entry entry)
        {
            var oferta = document.DocumentNode.Descendants("li").FirstOrDefault(li =>
            {
                var st = li.Descendants("strong").FirstOrDefault();
                return st != null && st.InnerText == "Źródło:";
            });

            if (oferta == null || oferta.Descendants("span").FirstOrDefault() == null)
            {
                return;
            }

            var text = oferta.Descendants("span").First().InnerText;
            var r = new Regex("zaktualizowane: ([0-9]{2}\\.[0-9]{2}\\.[0-9]{4})");
            var match = r.Match(text);
            if (match.Success)
            {
                entry.OfferDetails.LastUpdateDateTime = DateTime.ParseExact(match.Groups[1].Value, "dd'.'MM'.'yyyy", null);
                
                // There's no offer creation time on the page, so lets assume last update time is creation time.
                entry.OfferDetails.CreationDateTime = entry.OfferDetails.LastUpdateDateTime.Value;
            }
        }

        /// <summary>
        /// Sets price and address.
        /// </summary>
        /// <param name="document">Offer html document.</param>
        /// <param name="entry">Offer entry.</param>
        private void SetPriceAndAddressData(HtmlDocument document, Entry entry)
        {
            var infoBox = document.DocumentNode
                .Descendants("div")
                .FirstOrDefault(d => d.GetAttributeValue("class", string.Empty).Contains("box-offer-up desktop-only"));

            if (infoBox == null)
            {
                return;
            }

            var regex = new Regex(">([0-9]+(&nbsp;[0-9]+)*)(,[0-9]{2})?&nbsp;zł\\/m&sup2;<");
            entry.PropertyPrice.PricePerMeter = this.ParseOfferInfoRegex(regex, infoBox.InnerHtml);

            regex = new Regex(">([0-9]+(&nbsp;[0-9]+)*)(,[0-9]{2})?&nbsp;zł<");
            entry.PropertyPrice.TotalGrossPrice = this.ParseOfferInfoRegex(regex, infoBox.InnerHtml);
            
            regex = new Regex(">([0-9]+(&nbsp;[0-9]+)*)(,[0-9]{2})?&nbsp;m&sup2;<");
            entry.PropertyDetails.Area = this.ParseOfferInfoRegex(regex, infoBox.InnerHtml);

            var addressProperty = infoBox.Descendants().FirstOrDefault(d => d.GetAttributeValue("class", string.Empty).Contains("title-b"));
            if(addressProperty != null)
            {
                var addressElements = addressProperty.InnerText.Split(',').Select(e => e.Trim()).ToList();
                addressElements.Reverse();
                if (addressElements.Count > 1)
                {
                    if (Enum.TryParse(addressElements[1].ToUpperLatinLetters(), true, out PolishCity city))
                    {
                        entry.PropertyAddress.City = city;
                    }
                }

                if (addressElements.Count > 2)
                {
                    entry.PropertyAddress.District = addressElements[2];
                }                

                if (addressElements.Count > 3)
                {
                    entry.PropertyAddress.StreetName = addressElements[3];
                }

                this.TryAnotherAddressSource(document, entry, addressElements[0]);
            }
        }

        /// <summary>
        /// Sets property info.
        /// </summary>
        /// <param name="document">Offer html document.</param>
        /// <param name="entry">Offer entry.</param>
        private void SetPropertyInfo(HtmlDocument document, Entry entry)
        {
            var infoBox = document.DocumentNode
                .Descendants("div")
                .FirstOrDefault(d => d.GetAttributeValue("class", string.Empty).Contains("box-offer-inside mod-a mobile"));

            if (infoBox == null)
            {
                return;
            }

            var regex = new Regex("Piętro:([0-9]+)");
            var match = regex.Match(infoBox.InnerText);
            if (match.Success)
            {
                entry.PropertyDetails.FloorNumber = int.Parse(match.Groups[1].Value);
            }
            else
            {
                regex = new Regex("Piętro:parter");
                if (regex.IsMatch(infoBox.InnerText))
                {
                    entry.PropertyDetails.FloorNumber = 0;
                }
            }

            regex = new Regex("Liczba pokoi:([0-9]+)");
            match = regex.Match(infoBox.InnerText);
            if (match.Success)
            {
                entry.PropertyDetails.NumberOfRooms = int.Parse(match.Groups[1].Value);
            }

            regex = new Regex("Rok budowy:([0-9]{4})");
            match = regex.Match(infoBox.InnerText);
            if (match.Success)
            {
                entry.PropertyDetails.YearOfConstruction = int.Parse(match.Groups[1].Value);
            }
        }

        /// <summary>
        /// Sets contact information.
        /// </summary>
        /// <param name="document">Offer html document.</param>
        /// <param name="entry">Offer entry.</param>
        private void SetContactInfo(HtmlDocument document, Entry entry)
        {
            var agentBox = document.DocumentNode
                .Descendants("div")
                .FirstOrDefault(d => d.GetAttributeValue("class", string.Empty).Contains("box-agent-inner box-left"));

            if (agentBox == null)
            {
                return;
            }

            var nameProperty = agentBox.Descendants().FirstOrDefault(d => d.GetAttributeValue("class", string.Empty).Contains("name"));
            if(nameProperty != null)
            {
                entry.OfferDetails.SellerContact.Name = nameProperty.InnerText;
            }

            var fullPhone = agentBox
                                .Descendants("div")
                                .FirstOrDefault(d 
                                    => d.GetAttributeValue("class", string.Empty).Contains("phone-wrapper full"));

            if(fullPhone != null)
            {
                var phoneNum = fullPhone.Descendants().FirstOrDefault(d => d.GetAttributeValue("class", string.Empty).Contains("phone first"));
                if(phoneNum != null)
                {
                    entry.OfferDetails.SellerContact.Telephone = phoneNum.InnerText;
                }
            }
        }

        /// <summary>
        /// Sets raw description.
        /// </summary>
        /// <param name="document">Offer html document.</param>
        /// <param name="entry">Offer entry.</param>
        private void SetRawDescription(HtmlDocument document, Entry entry)
        {
            var descriptionBox = document.DocumentNode
                            .Descendants("div")
                            .FirstOrDefault(d => d.GetAttributeValue("id", string.Empty).Equals("boxCustomDesc"));

            if (descriptionBox != null)
            {
                entry.RawDescription = descriptionBox.InnerText.Replace("&nbsp;",string.Empty);
            }
        }

        /// <summary>
        /// Sometimes district or street isn't in upper part of the site. It can be found in additional section lower.
        /// Trying to parse it.
        /// </summary>
        /// <param name="document">Offer html document.</param>
        /// <param name="entry">Offer entry.</param>
        /// <param name="region">Offers region.</param>
        private void TryAnotherAddressSource(HtmlDocument document, Entry entry, string region)
        {
            var localisationBox = document.DocumentNode
                .Descendants("h3")
                .FirstOrDefault(d => d.InnerText == "Lokalizacja");

            if (localisationBox == null)
            {
                return;
            }

            localisationBox = localisationBox.ParentNode;

            var addressBox = localisationBox
                                .Descendants("li")
                                .FirstOrDefault(d => d.GetAttributeValue("class", string.Empty).Contains("adress"));

            if (addressBox == null) return;

            var adress = addressBox.Descendants("span").FirstOrDefault();
            if (adress == null) return;

            var elements = adress.InnerText.Split(',').Select(e => e.Trim()).ToList();
            elements.Reverse();
            if (elements.Count == 0 || !elements[0].Equals(region, StringComparison.InvariantCultureIgnoreCase)) return;

            var districtIndex = 0;
            if (elements.Count > 1 && elements[1].ToUpperLatinLetters().Equals(entry.PropertyAddress.City.ToString()))
            {
                districtIndex = 2;
            }
            else if (elements.Count > 2 && elements[2].ToUpperLatinLetters().Equals(entry.PropertyAddress.City.ToString()))
            {
                // happens when someone adds 'powiat' between city and region
                districtIndex = 3;
            }
            else
            {
                return;
            }

            if (elements.Count <= districtIndex) return;

            if (entry.PropertyAddress.District != null && !elements[districtIndex].Equals(entry.PropertyAddress.District))
            {
                return;
            }
            else
            {
                entry.PropertyAddress.District = elements[districtIndex];
            }

            var streetIndex = districtIndex + 1;
            if (elements.Count <= streetIndex) return;
            if (!string.IsNullOrEmpty(entry.PropertyAddress.StreetName))
            {
                var index = elements[streetIndex].ToLower().IndexOf(entry.PropertyAddress.StreetName.ToLower());
                if (index >= 0)
                {
                    var detailIndex = index + entry.PropertyAddress.StreetName.Count();
                    var addressDetail = elements[streetIndex].Substring(detailIndex).Trim();
                    if (!string.IsNullOrEmpty(addressDetail))
                    {
                        entry.PropertyAddress.DetailedAddress = addressDetail;
                    }
                }
            }
            else
            {
                entry.PropertyAddress.StreetName = elements[streetIndex];
            }
        }

        /// <summary>
        /// Tries to load additional information from offer details section of the offer.
        /// </summary>
        /// <param name="document">Offer html document.</param>
        /// <param name="entry">Offer entry.</param>
        private void TryToLoadAdditionalInfoFromOfferDetails(HtmlDocument document, Entry entry)
        {
            var offerDetailsBox = document.DocumentNode
                   .Descendants("h3")
                   .FirstOrDefault(d => d.InnerText == "Szczegóły ogłoszenia");

            if (offerDetailsBox == null)
            {
                return;
            }

            offerDetailsBox = offerDetailsBox.ParentNode;

            this.TryToGetConstructionYearFromText(offerDetailsBox.InnerText, entry);
            this.TryToGetBalconiesInfoFromText(offerDetailsBox.InnerText, entry);
            this.TryToGetGardenInfoFromText(offerDetailsBox.InnerText, entry);
            this.TryToGetBasementInfoFromText(offerDetailsBox.InnerText, entry);
            this.TryToGetIndoorParkingInfoFromText(offerDetailsBox.InnerText, entry);
            this.TryToGetOutdoorParkingInfoFromText(offerDetailsBox.InnerText, entry);
        }

        /// <summary>
        /// Tries to load additional information from entry's description.
        /// </summary>
        /// <param name="entry">Offer entry.</param>
        private void TryToLoadAdditionalInfoFromDescription(Entry entry)
        {
            if (!string.IsNullOrEmpty(entry.RawDescription))
            {
                this.TryToGetConstructionYearFromText(entry.RawDescription, entry);
                this.TryToGetBalconiesInfoFromText(entry.RawDescription, entry);
                this.TryToGetGardenInfoFromText(entry.RawDescription, entry);
                this.TryToGetBasementInfoFromText(entry.RawDescription, entry);
                this.TryToGetIndoorParkingInfoFromText(entry.RawDescription, entry);
                this.TryToGetOutdoorParkingInfoFromText(entry.RawDescription, entry);
            }
        }

        /// <summary>
        /// Tries to get construction year from text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="entry">Offer entry.</param>
        private void TryToGetConstructionYearFromText(string text, Entry entry)
        {
            if (entry.PropertyDetails.YearOfConstruction != null)
            {
                return;
            }

            var regex = new Regex("(rok budowy|zbudowano w|Rok budowy|Zbudowano w)[:&nbsp; ]*([0-9]{4})");
            var match = regex.Match(text);
            if (match.Success)
            {
                entry.PropertyDetails.YearOfConstruction = int.Parse(match.Groups[2].Value);
            }
        }

        /// <summary>
        /// Tries to get balconies information from the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="entry">Offer entry.</param>
        private void TryToGetBalconiesInfoFromText(string text, Entry entry)
        {
            if (text.Contains("brak balkonu", StringComparison.InvariantCultureIgnoreCase)
                || text.Contains("nie ma balkonu", StringComparison.InvariantCultureIgnoreCase))
            {
                entry.PropertyFeatures.Balconies = 0;
            }

            if (text.Contains("balkon", StringComparison.InvariantCultureIgnoreCase)
                || text.Contains("loggia", StringComparison.InvariantCultureIgnoreCase))
            {
                entry.PropertyFeatures.Balconies = 1;
            }
        }

        /// <summary>
        /// Tries to get garden information from the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="entry">Offer entry.</param>
        private void TryToGetGardenInfoFromText(string text, Entry entry)
        {
            if (entry.PropertyFeatures.GardenArea > 1) return;

            var regex = new Regex("(taras|ogród|ogródek)[:&nbsp; (]*([0-9]+((\\.|,)[0-9]+)?)[&nbsp; ]*m&sup2;");
            var match = regex.Match(text);
            if (match.Success)
            {
                entry.PropertyFeatures.GardenArea = decimal.Parse(match.Groups[2].Value.Replace(',','.'));
            }
            else if(text.Contains("ogród", StringComparison.InvariantCultureIgnoreCase)
                || text.Contains("taras", StringComparison.InvariantCultureIgnoreCase))
            {
                entry.PropertyFeatures.GardenArea = 1;
            }
        }

        /// <summary>
        /// Tries to get basement information from the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="entry">Offer entry.</param>
        private void TryToGetBasementInfoFromText(string text, Entry entry)
        {
            if (entry.PropertyFeatures.BasementArea > 1) return;

            var regex = new Regex("(komórka lokatorska|piwnica)[:&nbsp; (]*([0-9]+((\\.|,)[0-9]+)?)[&nbsp; ]*m&sup2;");
            var match = regex.Match(text);
            if (match.Success)
            {
                entry.PropertyFeatures.BasementArea = decimal.Parse(match.Groups[2].Value.Replace(',', '.'));
            }
            else if (text.Contains("piwnica") || text.Contains("komórka lokatorska"))
            {
                // If there's basement but there's no info about its size.
                entry.PropertyFeatures.BasementArea = 1;
            }
        }

        /// <summary>
        /// Tries to get outdoor parking information from the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="entry">Offer entry.</param>
        private void TryToGetOutdoorParkingInfoFromText(string text, Entry entry)
        {
            if(text.Contains("brak przynależnego miejsca parkingowego", StringComparison.InvariantCultureIgnoreCase))
            {
                entry.PropertyFeatures.IndoorParkingPlaces = 0;
                entry.PropertyFeatures.OutdoorParkingPlaces = 0;
                return;
            }

            var regex = new Regex("(na podjeździe|przed budynkiem|wiata garażowa)[:&nbsp; (]*([0-9]+)[&nbsp; ]*miejsc[ea]?");
            var match = regex.Match(text);
            if (match.Success)
            {
                entry.PropertyFeatures.OutdoorParkingPlaces = int.Parse(match.Groups[2].Value);
            }
        }

        /// <summary>
        /// Tries to get indoor parking information from the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="entry">Offer entry.</param>
        private void TryToGetIndoorParkingInfoFromText(string text, Entry entry)
        {
            if (text.Contains("nie ma garaż", StringComparison.InvariantCultureIgnoreCase)
                || text.Contains("brak garaż", StringComparison.InvariantCultureIgnoreCase)
                || text.Contains("garaż nie", StringComparison.InvariantCultureIgnoreCase)
                || text.Contains("garaż brak", StringComparison.InvariantCultureIgnoreCase))
            {
                entry.PropertyFeatures.IndoorParkingPlaces = 0;
                return;
            }

            if (text.Contains("garaż", StringComparison.InvariantCultureIgnoreCase)
                && !text.Contains("wiata garażowa", StringComparison.InvariantCultureIgnoreCase))
            {
                var regex = new Regex("(garaż|w garażu podziemnym|miejsca postojowe podziemne)[:&nbsp; (]*([0-9]+)[&nbsp; ]*miejsc[ea]?");
                var match = regex.Match(text);
                if (match.Success)
                {
                    entry.PropertyFeatures.IndoorParkingPlaces = int.Parse(match.Groups[2].Value);
                }
                else
                {
                    entry.PropertyFeatures.IndoorParkingPlaces = 1;
                }
            }
        }

        /// <summary>
        /// If after scanning description and offer details there are still nulls in some features fields, it assumes these features are not present in the property.
        /// </summary>
        /// <param name="entry">Offer entry.</param>
        /// <param name="offerType">Offer type.</param>
        private void AssumeNoFeatureIfNotMentioned(Entry entry, string offerType)
        {
            if (entry.PropertyFeatures.Balconies == null)
            {
                entry.PropertyFeatures.Balconies = 0;
            }

            if(!offerType.Contains("dom") && entry.PropertyFeatures.GardenArea == null)
            {
                entry.PropertyFeatures.GardenArea = 0;
            }

            if (entry.PropertyFeatures.BasementArea == null)
            {
                entry.PropertyFeatures.BasementArea = 0;
            }

            if (entry.PropertyFeatures.IndoorParkingPlaces == null)
            {
                entry.PropertyFeatures.IndoorParkingPlaces = 0;
            }
        }

        /// <summary>
        /// Parses offer info regex on given text.
        /// </summary>
        /// <param name="regex">Regex to match and parse.</param>
        /// <param name="text">Text.</param>
        /// <returns></returns>
        private decimal ParseOfferInfoRegex(Regex regex, string text)
        {
            decimal result;
            var match = regex.Match(text);
            if (match.Success)
            {
                result = decimal.Parse(match.Groups[1].Value.Replace("&nbsp;", string.Empty));
                if (match.Groups.Count > 3 && !string.IsNullOrEmpty(match.Groups[3].Value))
                {
                    result += decimal.Parse($"0.{match.Groups[3].Value.Trim(',')}");
                }
            }
            else
            {
                return -1;
            }

            return result;
        }

    }
}
