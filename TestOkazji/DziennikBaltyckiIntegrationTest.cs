using DziennikBaltycki.DziennikBaltycki;
using DziennikBaltycki.Interfaces;
using PropertiesFinderTests.Models;
using HtmlAgilityPack;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace PropertiesFinderTests
{
    [TestFixture]
    public class DziennikBaltyckiIntegrationTest
    {
        private Mock<IDziennikBaltyckiIntegration> _izolator;

        private DziennikBaltyckiIntegration _dziennikBaltyckiIntegration;

        [SetUp]
        public void SetUp()
        {
            _izolator = new Mock<IDziennikBaltyckiIntegration>();
            _dziennikBaltyckiIntegration = new DziennikBaltyckiIntegration(null, null, _izolator.Object);
        }
        [Test]
        public void GenerateDump_BrakTresciStrony_RezultatNieZawieraZadnejOferty()
        {
            // Arrange

            HtmlNode htmlbody = null;

            _izolator.Setup(x => x.PobiezLinkiDoStron()).Returns(new List<string> { null });
            _izolator.Setup(x => x.PobierzLinkiDoMieszkan(new List<string> { null })).Returns(new List<string> { "www.nieIstnieje.pl" });
            _izolator.Setup(x => x.PobierzDokument("www.nieIstnieje.pl")).Returns(htmlbody);

            // Act 

            var rezultat = _dziennikBaltyckiIntegration.GenerateDump();

            //Assert

            Assert.AreEqual(rezultat.Entries.ToList().Count(), 0);
        }
        [Test]
        public void ZwrocSzczegolyWlasnosci_PrawidloweParsowanie_PoprawneDane()
        {
            // Arrange

            var model = Strona.ZwrocSlownik();

            // Act 

            var rezultat = _dziennikBaltyckiIntegration.ZwrocSzczegolyWlasnosci(model);

            //Assert

            Assert.AreEqual(rezultat.Area, 135);
            Assert.AreEqual(rezultat.NumberOfRooms, 7);
            Assert.AreEqual(rezultat.FloorNumber, 2);
            Assert.AreEqual(rezultat.YearOfConstruction, 2019);

        }
        [Test]
        public void ZwrocSzczegolyOferty_BrakSzukanychElementow_PrzypisanieWartosciDomyslnych()
        {
            // Arrange

            var htmlbody = Strona.ZwrocPustyDokument().DocumentNode.SelectSingleNode("//body");
            // Act 

            var rezultat = _dziennikBaltyckiIntegration.ZwrocSzczegolyOferty(htmlbody, null);

            //Assert

            Assert.AreEqual(rezultat.SellerContact.Telephone, "brak informacji");
            Assert.AreEqual(rezultat.SellerContact.Name, "brak informacji");

        }
        [Test]
        public void ZwrocOpis_OpisZgodnyZOczekiwaniem_RezultatJestPrawidlowy()
        {
            // Arrange

            var model = Strona.ZwrocOpis();

            var htmlbody = model.DocumentNode.SelectSingleNode("//body");

            // Act 

            var rezultat = _dziennikBaltyckiIntegration.ZwrocOpis(htmlbody);

            //Assert

            Assert.IsNotNull(rezultat);
            Assert.AreEqual(rezultat, @"Jesli ci szwankuje zdrowie mozesz wezwac pogotowie ratownicy tak
            sie spiesza ze grabarze az sie ciesza Zrobia zastrzyk z pavilonu
            i nie wrocisz juz do domu");

        }
        [Test]
        public void ZwrocParkingWewnetrzny_DaneZOpisuZostalyPrawidlowoOdczytane_PoprawneOdczytanieOpisu()
        {
            // Arrange

            var model = Strona.ZwrocInformacjeOParkinguWewnetrznymWOpisie();

            // Act 

            var rezultat = _dziennikBaltyckiIntegration.ZwrocParkingWewnetrzny(new Dictionary<string, string>(), model);

            //Assert

            Assert.IsNotNull(rezultat);
            Assert.AreEqual(rezultat, 1);
        }
        [Test]
        public void SprawdzIleMetrow_PrawidloweOdczytaniePowierzchniOgrodu_WartoscJestZgodnaZOczekiwaniem()
        {
            // Arrange

            var model = Strona.ZwocDanneDoOdczytaniaPowierzchniOgrodu();

            // Act 

            var rezultat = _dziennikBaltyckiIntegration.SprawdzIleMetrow("ogród", ref model);

            //Assert

            Assert.IsNotNull(rezultat);
            Assert.AreEqual(rezultat, 300);
        }
    }
}

