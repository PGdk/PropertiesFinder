using HtmlAgilityPack;
using Models;
using NUnit.Framework;
using ZnajdzTo;

namespace ZnajdzToTests
{
    [TestFixture]
    public class ZnajdzToHomeSalePageTest
    {
        private ZnajdzToHomeSalePage _znajdzToHomeSalePage;
        private HtmlDocument _homeSalePage;
        private string _url;

        [SetUp]
        public void Setup()
        {
            _homeSalePage = new HtmlDocument();
            _url = "https://nieruchomosci.znajdzto.pl/oferta/testowa";
        }

        [Test]
        public void IsCity_WhenPolishCityInHtmlDocument_ReturnsTrue()
        {
            // Arrange
            string cityHtml = "<div id=\"left-col\"><p>lokalizacja: Wrocław</p></div>";
            HtmlNode node = HtmlNode.CreateNode(cityHtml);
            _homeSalePage.DocumentNode.AppendChild(node);
            
            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.IsTrue(_znajdzToHomeSalePage.IsCity);
        }

        [Test]
        public void IsCity_WhenNoPolishCityInHtmlDocument_ReturnsFalse()
        {
            // Act
            string cityHtml = "<div id=\"left-col\"><p></p></div>";
            HtmlNode node = HtmlNode.CreateNode(cityHtml);
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.IsFalse(_znajdzToHomeSalePage.IsCity);
        }

        [Test]
        public void City_WhenPolishCityInHtmlDocument_ReturnsPolishCity()
        {
            // Arrange
            string cityHtml = "<div id=\"left-col\"><p>lokalizacja: Wrocław</p></div>";
            HtmlNode node = HtmlNode.CreateNode(cityHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.AreEqual(PolishCity.WROCLAW, _znajdzToHomeSalePage.City);
        }

        [Test]
        public void StreetName_WhenStreetNameInHtmlDocument_ReturnsStreetName()
        {
            // Arrange
            string expected = "Testowa ulica";
            string streetNameHtml = $"<div id=\"left-col\"><p>dzielnica/adres: TestowaDzielnica, {expected} </p></div>";
            HtmlNode node = HtmlNode.CreateNode(streetNameHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.AreEqual(expected, _znajdzToHomeSalePage.StreetName);
        }

        [Test]
        public void StreetName_WhenNoStreetNameInHtmlDocument_ReturnsNull()
        {
            // Arrange
            string streetNameHtml = $"<div id=\"left-col\"><p>dzielnica: TestowaDzielnica </p></div>";
            HtmlNode node = HtmlNode.CreateNode(streetNameHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.IsNull(_znajdzToHomeSalePage.StreetName);
        }

        [Test]
        public void District_WhenDistrictInHtmlDocument_ReturnsDistrict()
        {
            // Arrange
            string expected = "Testowa ulica";
            string districtHtml = $"<div id=\"left-col\"><p>dzielnica/adres: {expected}, Testowa Ulica </p></div>";
            HtmlNode node = HtmlNode.CreateNode(districtHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.AreEqual(expected, _znajdzToHomeSalePage.District);
        }

        [Test]
        public void District_WhenNoDistrictInHtmlDocument_ReturnsNull()
        {
            // Arrange
            string districtHtml = $"<div id=\"left-col\"><p>adres: Testowa Ulica </p></div>";
            HtmlNode node = HtmlNode.CreateNode(districtHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.IsNull(_znajdzToHomeSalePage.District);
        }

        [Test]
        public void FloorNumber_WhenNineDdElementsAndFloorNumberInHtmlDocument_ReturnsFloorNumberInSeventhDd()
        {
            // Arrange
            int expected = 4;
            string floorNumberHtml = $"<div class=\"offerDetails\"><dl><dd></dd><dd></dd><dd></dd><dd></dd><dd></dd><dd></dd><dd>{expected}</dd><dd></dd><dd></dd></dl></div>";
            HtmlNode node = HtmlNode.CreateNode(floorNumberHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.AreEqual(expected, _znajdzToHomeSalePage.FloorNumber);
        }

        [Test]
        public void FloorNumber_WhenNoNineDdElementsAndFloorNumberInHtmlDocument_ReturnsFloorNumberInSixthDd()
        {
            // Arrange
            int expected = 4;
            string floorNumberHtml = $"<div class=\"offerDetails\"><dl><dd></dd><dd></dd><dd></dd><dd></dd><dd></dd><dd>{expected}</dd><dd></dd><dd></dd></dl></div>";
            HtmlNode node = HtmlNode.CreateNode(floorNumberHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.AreEqual(expected, _znajdzToHomeSalePage.FloorNumber);
        }

        [Test]
        public void FloorNumber_WhenNoFloorNumberInHtmlDocument_ReturnsNull()
        {
            // Arrange
            string floorNumberHtml = $"<div class=\"offerDetails\"><dl><dd></dd></dl></div>";
            HtmlNode node = HtmlNode.CreateNode(floorNumberHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.IsNull(_znajdzToHomeSalePage.FloorNumber);
        }

        [Test]
        public void NumberOfRooms_WhenNumberOfRoomsInHtmlDocument_ReturnsNumberOfRooms()
        {
            // Arrange
            int expected = 2;
            string numberOfRoomsHtml = $"<div class=\"offerDetails\"><dl><dd></dd><dd></dd><dd></dd><dd>{expected}</dd></dl></div>";
            HtmlNode node = HtmlNode.CreateNode(numberOfRoomsHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.AreEqual(expected, _znajdzToHomeSalePage.NumberOfRooms);
        }

        [Test]
        public void NumberOfRooms_WhenNoNumberOfRoomsInHtmlDocument_ReturnsZero()
        {
            // Arrange
            int expected = 0;
            string numberOfRoomsHtml = $"<div class=\"offerDetails\"><dl><dd></dd><dd></dd><dd></dd><dd></dd></dl></div>";
            HtmlNode node = HtmlNode.CreateNode(numberOfRoomsHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.AreEqual(expected, _znajdzToHomeSalePage.NumberOfRooms);
        }

        [Test]
        public void YearOfConstruction_WhenNineDdElementsAndYearOfConstructionInHtmlDocument_ReturnsYearOfConstructionInEighthDd()
        {
            // Arrange
            int expected = 2020;
            string yearOfConstructionHtml = $"<div class=\"offerDetails\"><dl><dd></dd><dd></dd><dd></dd><dd></dd><dd></dd><dd></dd><dd></dd><dd>{expected}</dd><dd></dd></dl></div>";
            HtmlNode node = HtmlNode.CreateNode(yearOfConstructionHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.AreEqual(expected, _znajdzToHomeSalePage.YearOfConstruction);
        }

        [Test]
        public void YearOfConstruction_WhenNoNineDdElementsAndYearOfConstructionInHtmlDocument_ReturnsYearOfConstructionInSeventhDd()
        {
            // Arrange
            int expected = 2020;
            string yearOfConstructionHtml = $"<div class=\"offerDetails\"><dl><dd></dd><dd></dd><dd></dd><dd></dd><dd></dd><dd></dd><dd>{expected}</dd><dd></dd></dl></div>";
            HtmlNode node = HtmlNode.CreateNode(yearOfConstructionHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.AreEqual(expected, _znajdzToHomeSalePage.YearOfConstruction);
        }

        [Test]
        public void YearOfConstruction_WhenNoYearOfConstructionInHtmlDocument_ReturnsNull()
        {
            // Arrange
            string yearOfConstructionHtml = $"<div class=\"offerDetails\"><dl><dd></dd></dl></div>";
            HtmlNode node = HtmlNode.CreateNode(yearOfConstructionHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.IsNull(_znajdzToHomeSalePage.YearOfConstruction);
        }

        [Test]
        public void Email_WhenEmailInHtmlDocument_ReturnsEmail()
        {
            // Arrange
            string expected = "test@test.pl";
            string emailHtml = $"<div class=\"offerDescription\">email: {expected}</div>";
            HtmlNode node = HtmlNode.CreateNode(emailHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.AreEqual(expected, _znajdzToHomeSalePage.Email);
        }

        [Test]
        public void Email_WhenNoEmailInHtmlDocument_ReturnsNull()
        {
            // Arrange
            string emailHtml = $"<div class=\"offerDescription\"></div>";
            HtmlNode node = HtmlNode.CreateNode(emailHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.IsNull(_znajdzToHomeSalePage.Email);
        }

        [Test]
        public void Name_WhenNoNameInHtmlDocument_ReturnsNull()
        {
            // Arrange
            string nameHtml = $"<div class=\"offerDescription\"></div>";
            HtmlNode node = HtmlNode.CreateNode(nameHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.IsNull(_znajdzToHomeSalePage.Name);
        }

        [Test]
        public void Telephone_WhenTelephoneInHtmlDocument_ReturnTelephone()
        {
            // Arrange
            string expected = "111111111";
            string telephoneHtml = $"<div class=\"offerSection\"><div class=\"phone-box\"><div class=\"hidden-phone\"><h1>{expected}<br></h1></div></div></div>";
            HtmlNode node = HtmlNode.CreateNode(telephoneHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.AreEqual(expected, _znajdzToHomeSalePage.Telephone);
        }

        [Test]
        public void Telephone_WhenNoTelephoneInHtmlDocument_ReturnNull()
        {
            // Arrange
            string telephoneHtml = $"<div class=\"offerSection\"><div class=\"phone-box\"><div class=\"hidden-phone\"></div></div></div>";
            HtmlNode node = HtmlNode.CreateNode(telephoneHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.IsNull(_znajdzToHomeSalePage.Telephone);
        }

        [Test]
        public void PricePerMeter_WhenPricePerMeterInHtmlDocument_ReturnPricePerMeter()
        {
            // Arrange
            int expected = 1;
            string pricePerMeterHtml = $"<div class=\"offerDetails\"><dl><dd></dd><dd>{expected}</dd></dl></div>";
            HtmlNode node = HtmlNode.CreateNode(pricePerMeterHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.AreEqual(expected, _znajdzToHomeSalePage.PricePerMeter);
        }

        [Test]
        public void PricePerMeter_WhenNoPricePerMeterInHtmlDocument_ReturnsZero()
        {
            // Arrange
            int expected = 0;
            string pricePerMeterHtml = $"<div class=\"offerDetails\"><dl><dd></dd><dd></dd></dl></div>";
            HtmlNode node = HtmlNode.CreateNode(pricePerMeterHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.AreEqual(expected, _znajdzToHomeSalePage.PricePerMeter);
        }

        [Test]
        public void TotalGrossPrice_WhenTotalGrossPriceInHtmlDocument_ReturnTotalGrossPrice()
        {
            // Arrange
            int expected = 10;
            string totalGrossPriceHtml = $"<div class=\"offerDetails\"><dl><dd>{expected}</dd></dl></div>";
            HtmlNode node = HtmlNode.CreateNode(totalGrossPriceHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.AreEqual(expected, _znajdzToHomeSalePage.TotalGrossPrice);
        }

        [Test]
        public void TotalGrossPrice_WhenNoTotalGrossPriceInHtmlDocument_ReturnsZero()
        {
            // Arrange
            int expected = 0;
            string totalGrossPriceHtml = $"<div class=\"offerDetails\"><dl><dd></dd></dl></div>";
            HtmlNode node = HtmlNode.CreateNode(totalGrossPriceHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.AreEqual(expected, _znajdzToHomeSalePage.TotalGrossPrice);
        }

        [Test]
        public void Area_WhenTotalGrossPriceAndPricePerMeterInHtmlDocument_ReturnsArea()
        {
            // Arrange
            int expected = 10;
            string areaHtml = $"<div class=\"offerDetails\"><dl><dd>10</dd><dd>1</dd></dl></div>";
            HtmlNode node = HtmlNode.CreateNode(areaHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.AreEqual(expected, _znajdzToHomeSalePage.Area);
        }

        [Test]
        public void Area_WhenNoPricePerMeterInHtmlDocument_ReturnsZero()
        {
            // Arrange
            int expected = 0;
            string areaHtml = $"<div class=\"offerDetails\"><dl><dd>10</dd><dd></dd></dl></div>";
            HtmlNode node = HtmlNode.CreateNode(areaHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.AreEqual(expected, _znajdzToHomeSalePage.Area);
        }

        [Test]
        public void Area_WhenNoTotalGrossPriceInHtmlDocument_ReturnsZero()
        {
            // Arrange
            int expected = 0;
            string areaHtml = $"<div class=\"offerDetails\"><dl><dd></dd><dd>1</dd></dl></div>";
            HtmlNode node = HtmlNode.CreateNode(areaHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.AreEqual(expected, _znajdzToHomeSalePage.Area);
        }

        [Test]
        public void Balconies_WhenTextBalkonInHtmlDocument_ReturnsOne()
        {
            // Arrange
            int expected = 1;
            string balconiesHtml = $"<div class=\"offerDescription\">balkon</div>";
            HtmlNode node = HtmlNode.CreateNode(balconiesHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.AreEqual(expected, _znajdzToHomeSalePage.Balconies);
        }

        [Test]
        public void Balconies_WhenTextBrakBalkonuInHtmlDocument_ReturnsZero()
        {
            // Arrange
            int expected = 0;
            string balconiesHtml = $"<div class=\"offerDescription\">brak balkonu</div>";
            HtmlNode node = HtmlNode.CreateNode(balconiesHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.AreEqual(expected, _znajdzToHomeSalePage.Balconies);
        }

        [Test]
        public void Balconies_WhenNoBalconiesInHtmlDocument_ReturnsNull()
        {
            // Arrange
            string balconiesHtml = $"<div class=\"offerDescription\"></div>";
            HtmlNode node = HtmlNode.CreateNode(balconiesHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.IsNull(_znajdzToHomeSalePage.Balconies);
        }

        [Test]
        public void BasementArea_WhenNoBasementAreaInHtmlDocument_ReturnsNull()
        {
            // Arrange
            string basemenetAreaHtml = $"<div class=\"offerDescription\"></div>";
            HtmlNode node = HtmlNode.CreateNode(basemenetAreaHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.IsNull(_znajdzToHomeSalePage.BasementArea);
        }

        [Test]
        public void RawDescription_WhenRawDescriptionInHtmlDocument_ReturnsRawDescription()
        {
            // Arrange
            string expected = "Raw description";
            string rawDescriptionHtml = $"<div class=\"offerDescription\">{expected}</div>";
            HtmlNode node = HtmlNode.CreateNode(rawDescriptionHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.AreEqual(expected, _znajdzToHomeSalePage.RawDescription);
        }

        [Test]
        public void RawDescription_WhenNoRawDescriptionInHtmlDocument_ReturnsEmptyString()
        {
            // Arrange
            string rawDescriptionHtml = $"<div class=\"offerDescription\"></div>";
            HtmlNode node = HtmlNode.CreateNode(rawDescriptionHtml);
            _homeSalePage.DocumentNode.AppendChild(node);

            // Act
            _znajdzToHomeSalePage = new ZnajdzToHomeSalePage(_homeSalePage, _url);

            // Assert
            Assert.IsEmpty(_znajdzToHomeSalePage.RawDescription);
        }
    }
}
