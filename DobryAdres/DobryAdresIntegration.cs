using Interfaces;
using Models;
using System;
using System.Collections.Generic;

using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Security.Cryptography;

namespace Application.DobryAdres
{
    public class DobryAdresIntegration : IWebSiteIntegration
    {
        public WebPage WebPage { get; }
        public IDumpsRepository DumpsRepository { get; }
        public IEqualityComparer<Entry> EntriesComparer { get; }
        private IWebDriver driver;
        private bool cancelEntry;

        public DobryAdresIntegration(IDumpsRepository dumpsRepository,
            IEqualityComparer<Entry> equalityComparer)
        {
            DumpsRepository = dumpsRepository;
            EntriesComparer = equalityComparer;
            WebPage = new WebPage
            {
                Url = "https://www.dobryadres.pl/sprzedaz/mieszkania",
                Name = "DobryAdres.pl",
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
            Dump dump = new Dump
            {
                DateTime = DateTime.Now,
                WebPage = WebPage
            };
            dump.Entries = GenerateListOfEntries();
            return dump;
        }

        public List<Entry> GenerateListOfEntries()
        {
            List<Entry> entries = new List<Entry>();

            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            chromeOptions.AddArguments("--log-level=3");

            const int NUM_OF_PAGES = 3;

            using (driver = new ChromeDriver(chromeOptions))
            {
                for (int pageNumber = 1; pageNumber <= NUM_OF_PAGES; pageNumber++)
                {
                    var offersUrls = GenerateLinksToOffers(pageNumber);
                    foreach (var u in offersUrls)
                    {
                        // Jesli w trakcie generowania oferty okaze sie ze oferta nas nie interesuje 
                        // (np. ze wzgledu na brak podanego miasta) do zmiennej cancelEntry jest przypisywane true
                        cancelEntry = false;
                        Entry entry = GenerateEntry(u);
                        if (!cancelEntry)
                            entries.Add(entry);

                        // TEST
                        //Console.WriteLine("Miasto:  " + entry.PropertyAddress.City);
                    }
                    offersUrls.Clear();
                    Console.WriteLine($"NUMER STRONY: {pageNumber}");
                }
            }
            Console.WriteLine($"Liczba ofert: {entries.Count()}");
            return entries;
        }

        public List<string> GenerateLinksToOffers(int pageNumber)
        {
            var offersUrls = new List<string>();
            var mainUrl = @"https://www.dobryadres.pl/lista-ofert.php?1=1&rodzajnieruchomosci=MIESZKANIE&rodzajtransakcji=SPRZEDA%C5%BB&strona=";
            driver.Url = mainUrl + pageNumber.ToString();

            var nonSpecialOffers = driver.FindElement(By.XPath("/ html / body / div[2] / div[4] / div / div[1] / div[3] / div / div[3]"));
            var offers = nonSpecialOffers.FindElements(By.ClassName("lista_ofert"));
            
            foreach (var e in offers)
                offersUrls.Add(e.FindElement(By.XPath("a")).GetAttribute("href"));

            return offersUrls;
        }

        public Entry GenerateEntry(string url)
        {
            driver.Url = url;
            var nameTags = driver.FindElements(By.ClassName("oferta_tabela_50_1"));
            var valueTags = driver.FindElements(By.ClassName("oferta_tabela_50_2"));
            var desc = driver.FindElement(By.ClassName("maximg")).Text;
            return new Entry
            {
                RawDescription = desc,
                OfferDetails = GenerateOfferDetailsInfo(),
                PropertyAddress = GeneratePropertyAddressInfo(),
                PropertyFeatures = GeneratePropertyFeaturesInfo(),
                PropertyPrice = GeneratePropertyPriceInfo(nameTags, valueTags),
                PropertyDetails = GeneratePropertyDetailsInfo(nameTags, valueTags)
            };
        }

        public OfferDetails GenerateOfferDetailsInfo()
        {
            return new OfferDetails
            {
                Url = driver.Url,
                // Oferty nie maja nigdzie podanych dat i zakladam ze w opisie tez nie beda podane
                // nie jest rowniez podane czy oferta jest aktualna wiec zakladam ze kazda oferta na stronie jest wazna
                CreationDateTime = default,
                LastUpdateDateTime = null,
                OfferKind = OfferKind.SALE,
                IsStillValid = true,
                SellerContact = GenerateSellerContactInfo()
            };
        }

        public SellerContact GenerateSellerContactInfo()
        {
            var name = driver.FindElement(By.XPath("//*[@id=\"oferta_margines\"]/div[1]/div[2]/div/div[2]/span[1]"));
            var tel = driver.FindElement(By.XPath("//*[@id=\"telefon\"]"));
            var mail = driver.FindElement(By.XPath("//*[@id=\"oferta_margines\"]/div[1]/div[2]/div/div[2]/a"));
            var finalMail = CheckEMail(mail);
            var finalTel = CheckTelephone(tel);

            return new SellerContact
            {
                Email = finalMail,
                Telephone = finalTel,
                // Nie jestem w stanie wyroznic imienia i nazwiska z opisu
                Name = name.GetAttribute("innerHTML")
            };
        }

        public string CheckTelephone(IWebElement tel)
        {
            string result = "NIEZNANE";
            if (tel == null)
                result = SearchDescriptionFor("email");
            else
            {
                var seperateNumbers = tel.GetAttribute("class").Split(',');
                result = GetDecimalValue(seperateNumbers[0]);
            }
            return result;
        }

        public string CheckEMail(IWebElement mail)
        {
            string result = "NIEZNANE";
            if (mail == null) 
                result = SearchDescriptionFor("email");
            else
            {
                Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase);
                //find items that matches with our pattern
                Match emailMatch = emailRegex.Match(mail.GetAttribute("innerHTML"));
                if (emailMatch.Success)
                    result = emailMatch.Value;
            }
            return result;
        }

        public PropertyPrice GeneratePropertyPriceInfo(ReadOnlyCollection<IWebElement> nameTags, ReadOnlyCollection<IWebElement> valueTags)
        {
            var grossPriceTag = driver.FindElements(By.ClassName("wyroznionacena"));
            decimal grossPrice = decimal.Parse(GetDecimalValue(grossPriceTag[1].Text));

            int index = SearchForIndex(nameTags, "Cena za m2");
            decimal ppm = (index < 0) ? 
                decimal.Parse(GetDecimalValue(SearchDescriptionFor("pricePerMeter"))) : decimal.Parse(GetDecimalValue(valueTags[index].Text));

            index = SearchForIndex(nameTags, "Czynsz");
            decimal resRent = (index < 0) ? 
                decimal.Parse(GetDecimalValue(SearchDescriptionFor("rent"))) : decimal.Parse(GetDecimalValue(valueTags[index].Text));

            return new PropertyPrice
            {
                TotalGrossPrice = grossPrice,
                PricePerMeter = ppm,
                ResidentalRent = resRent
            };
        }

        public PropertyDetails GeneratePropertyDetailsInfo(ReadOnlyCollection<IWebElement> nameTags, ReadOnlyCollection<IWebElement> valueTags)
        {
            int index = SearchForIndex(nameTags, "Liczba pokoi");
            int numOfRooms = (index < 0) ?
                int.Parse(GetDecimalValue(SearchDescriptionFor("numberOfRooms"))) : int.Parse(GetDecimalValue(valueTags[index].Text));

            index = SearchForIndex(nameTags, "Powierzchnia całkowita");
            decimal area = (index < 0) ?
                decimal.Parse(GetDecimalValue(SearchDescriptionFor("area"))) : decimal.Parse(CheckArea(valueTags[index].Text));

            index = SearchForIndex(nameTags, "Rok budowy");
            int yoc = (index < 0) ?
                int.Parse(GetDecimalValue(SearchDescriptionFor("yearOfConstruction"))) : int.Parse(GetDecimalValue(valueTags[index].Text));

            index = SearchForIndex(nameTags, "Piętro");
            int floorNum;
            if (index >= 0)
            {
                if (valueTags[index].Text.ToLower() == "parter")
                    floorNum = 0;
                else
                    floorNum = int.Parse(GetDecimalValue(valueTags[index].Text));
            }
            else
                floorNum = int.Parse(GetDecimalValue(SearchDescriptionFor("floorNumber")));

            return new PropertyDetails
            {
                Area = area,
                NumberOfRooms = numOfRooms,
                FloorNumber = floorNum,
                YearOfConstruction = yoc
            };
        }

        public string CheckArea(string area)
        {
            var formattedArea = area.Split('m');
            return GetDecimalValue(formattedArea[0]);
        }

        public PropertyAddress GeneratePropertyAddressInfo()
        {
            var titleTag = driver.FindElement(By.XPath("//*[@id=\"CALY\"]/div/div/div[1]/div[2]/div[2]/h2"));
            string[] address = titleTag.Text.Split(',');

            string city = GenerateCity(address);
            string district = null;
            string street = null;
            if (address.Length > 1
                && address[1].ToUpper().IndexOf("UL. ") == -1
                && address[1].ToUpper().IndexOf("OSIEDLE ") == -1)
            {
                district = address[1].ToUpper().Trim();
            }
            district ??= SearchDescriptionFor("district");

            if (address.Length > 1 && address[1].ToUpper().IndexOf("UL. ") != -1)
            {
                street = GenerateDistrictOrStreet(address[1].ToUpper(), "UL. ").Trim();
            }
            else if (address.Length > 2 && address[1].ToUpper().IndexOf("OSIEDLE ") == -1)
            {
                street = GenerateDistrictOrStreet(address[2].ToUpper(), "UL. ").Trim();
            }
            street ??= SearchDescriptionFor("street");

            /*string district = (address.Length < 2) ? null : address[1].Trim().ToLower();
            district ??= SearchDescriptionFor("district");
            string street = (address.Length < 3) ? null : address[2].Trim().ToLower();
            street ??= SearchDescriptionFor("street");*/

            string detailedAddress = SearchDescriptionFor("detailedAddress");
            return new PropertyAddress
            {
                City = (PolishCity)Enum.Parse(typeof(PolishCity), city),
                District = district,
                StreetName = street,
                DetailedAddress = detailedAddress
            };
        }

        public string GenerateDistrictOrStreet(string address, string substrToRemove)
        {
            int index = address.IndexOf(substrToRemove);
            string clearedStr = (index < 0) ? address : address.Remove(index, substrToRemove.Length);
            return clearedStr;
        }

        /*public string GenerateDistrict(string[] address)
        {
            string district = address[1].ToUpper();
            string substrToRemove = "OSIEDLE ";
            int index = district.IndexOf(substrToRemove);
            string clearedStr = (index < 0) ? district : district.Remove(index, substrToRemove.Length);
            return clearedStr;
        }

        public string GenerateStreet(string[] address)
        {
            string street = address[2].ToUpper();
            string substrToRemove = "UL. ";
            int index = street.IndexOf(substrToRemove);
            string clearedStr = (index < 0) ? street : street.Remove(index, substrToRemove.Length);
            return clearedStr;
        }*/

        public string GenerateCity(string[] address)
        {
            string city = "NIEZNANE";
            string substrToRemove = "Mieszkanie na sprzedaż - ";
            foreach (var a in address)
            {
                int index = a.IndexOf(substrToRemove);
                string clearedStr = (index < 0) ? a : a.Remove(index, substrToRemove.Length);
                clearedStr = clearedStr.Trim().Replace(' ', '_');
                clearedStr = TranslateCityToASCII(clearedStr.ToLower()).ToUpper();
                if (Enum.IsDefined(typeof(PolishCity), clearedStr))
                {
                    city = clearedStr;
                    return city;
                }
            }
            city = SearchDescriptionFor("city").ToLower();
            city = TranslateCityToASCII(city.Trim().Replace(' ', '_')).ToUpper();
            if (!Enum.IsDefined(typeof(PolishCity), city))
            {
                cancelEntry = true;
                city = PolishCity.GDANSK.ToString();
            }
            return city;
        }

        public string TranslateCityToASCII(string city)
        {
            city = city.Replace('ą', 'a');
            city = city.Replace('ę', 'e');
            city = city.Replace('ó', 'o');
            city = city.Replace('ś', 's');
            city = city.Replace('ł', 'l');
            city = city.Replace('ż', 'z');
            city = city.Replace('ź', 'z');
            city = city.Replace('ć', 'c');
            city = city.Replace('ń', 'n');
            return city;
        }

        public PropertyFeatures GeneratePropertyFeaturesInfo()
        {
            return new PropertyFeatures
            {
                GardenArea = null,
                Balconies = null,
                BasementArea = null,
                OutdoorParkingPlaces = null,
                IndoorParkingPlaces = null
            };
        }

        public string SearchDescriptionFor(string property)
        {
            string result = "NIEZNANE";
            List<string> keyWords = new List<string>();
            switch (property)
            {
                case "email":
                    // RegEx do wyszukiwania adresów email
                    result = FindEMailInDescription();
                    break;
                case "telephone":
                    keyWords.Add("tel.");
                    keyWords.Add("telefonu");
                    result = SearchForwardsInDescription(keyWords, 1);
                    break;
                case "pricePerMeter":
                    keyWords.Add("cena za metr kwadratowy");
                    keyWords.Add("cena za m2");
                    result = SearchForwardsInDescription(keyWords, 2);
                    break;
                case "rent":
                    // sprawdzac czy do konca zdania nie bylo liczby, jak tak to sprawdzic czy przynajmniej 3 cyfrowa
                    keyWords.Add("czynsz");
                    result = SearchForwardsInDescription(keyWords, 2);
                    break;
                case "numberOfRooms":
                    // dwupokojowe/3-pokojowe/4 pokojowe
                    keyWords.Add("pokojowe");
                    keyWords.Add("pokoi");
                    result = SearchBackwardsInDescription(keyWords, 1);
                    break;
                case "area":
                    keyWords.Add("mieszkanie o powierzchni");
                    keyWords.Add("powierzchnia mieszkania");
                    keyWords.Add("na powierzchnię");
                    result = SearchForwardsInDescription(keyWords, 2);
                    break;
                case "yearOfConstruction":
                    keyWords.Add("rok budowy");
                    result = SearchForwardsInDescription(keyWords, 1);
                    break;
                case "floorNumber":
                    // X piętrze, X piętro, sprawdzam tylko przed bo po może być podana liczba pięter w budynku
                    keyWords.Add("piętrze");
                    keyWords.Add("piętro");
                    keyWords.Add("parterze");
                    result = SearchBackwardsInDescription(keyWords, 1);
                    break;
                case "district":
                    keyWords.Add("dzielnica");
                    break;
                case "street":
                    // Nie wiem jak odróżnić adres oferty od adresu firmy
                    keyWords.Add("mieszkanie znajduje się przy ul.");
                    keyWords.Add("mieszkanie znajduje się przy ulicy");
                    break;
                case "detailedAddress":
                    keyWords.Add("adres:");
                    // Nie mam pojęcia jak i w żadnej sprawdzanej przeze mnie ofercie nie był podany nr mieszkania itp
                    break;
            }
            return result;
        }

        public string FindEMailInDescription()
        {
            string desc = driver.FindElement(By.ClassName("maximg")).Text;
            string result = "NIEZNANE";

            Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase);
            //find items that matches with our pattern
            Match emailMatch = emailRegex.Match(desc);
            if (emailMatch.Success)
                result = emailMatch.Value;

            return result;
        }

        public string SearchForwardsInDescription(List<string> keyWords, int distance)
        {
            string desc = driver.FindElement(By.ClassName("maximg")).Text;
            string result = "NIEZNANE";
            desc = desc.ToLower();
            foreach (var word in keyWords)
            {
                string nextWord = GetNextWord(word);
                string numericValue = GetDecimalValue(nextWord);
                if (numericValue != "0")
                    return numericValue;
                else if (distance > 1)
                {
                    nextWord = GetNextWord(nextWord);
                    numericValue = GetDecimalValue(nextWord);
                    if (numericValue != "")
                        return numericValue;
                }
            }
            return result;
        }

        public string GetNextWord(string word)
        {
            string desc = driver.FindElement(By.ClassName("maximg")).Text;
            string result = "NIEZNANE";
            desc = desc.ToLower();

            int startingIndex = desc.IndexOf(word);
            if (startingIndex < 0) 
                return result;
            startingIndex += word.Length;
            startingIndex++;
            int endingIndex = desc.IndexOf(' ', startingIndex);
            if (startingIndex >= 0 && endingIndex >= 0)
            {
                return desc[startingIndex..endingIndex];
            }
            return result;
        }

        public string SearchBackwardsInDescription(List<string> keyWords, int distance)
        {
            string desc = driver.FindElement(By.ClassName("maximg")).Text;
            string result = "NIEZNANE";
            desc = desc.ToLower();
            foreach (var word in keyWords)
            {
                string prevWord = GetPrevWord(word);
                string numericValue = GetDecimalValue(prevWord);
                if (numericValue != "0")
                    return numericValue;
                else if (distance > 1)
                {
                    prevWord = GetPrevWord(prevWord);
                    numericValue = GetDecimalValue(prevWord);
                    if (numericValue != "")
                        return numericValue;
                }
            }
            return result;
        }

        public string GetPrevWord(string word)
        {
            string desc = driver.FindElement(By.ClassName("maximg")).Text;
            string result = "NIEZNANE";
            desc = desc.ToLower();

            int startingIndex = desc.IndexOf(word);
            if (startingIndex < 0)
                return result;
            int endingIndex = desc.LastIndexOf(' ', startingIndex);
            if (startingIndex >= 0 && endingIndex >= 0)
            {
                return desc[endingIndex..startingIndex];
            }
            return result;
        }

        public string GetDecimalValue(string str)
        {
            var valueTag = Regex.Split(str, @"[^0-9\.]+").Where(c => c != "." && c.Trim() != "");
            string value = string.Join("", valueTag);
            var valueSplit = value.Split('.');
            value = String.Join(".", valueSplit.Where(s => !string.IsNullOrEmpty(s)));
            if (value == "") value = "0";
            return value;
        }

        public string GetIntegerValue(string str)
        {
            string number = new String(str.Where(Char.IsDigit).ToArray());
            if (number == "") number = "0";
            return number;
        }

        public int SearchForIndex(ReadOnlyCollection<IWebElement> tags, string phrase)
        {
            int index = 0;
            foreach(var t in tags)
            {
                if (t.Text == phrase)
                    return index;
                index++;
            }
            return -1;
        }
    }
}