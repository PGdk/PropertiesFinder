using IntegrationApi.Models;
using PropertiesFinderTests.Models;
using Models;
using NUnit.Framework;
using System.Collections.Generic;

namespace PropertiesFinderTests
{
    [TestFixture]
    public class OkazjeTest
    {
        private Okazje _okazje;

        [SetUp]
        public void SetUp()
        {
            _okazje = new Okazje();
        }
        [Test]
        public void NajlepszeOferty_SprawdzamPojedynczeMiasto_NajnizszaCenaZPojedynczegoMiasta()
        {
            // Arrange

            var ofertyCenowe = NajlepszeOkazje.NajnizszaCenaZPojedynczegoMiasta();

            // Act 

            var rezultat = _okazje.ZwrocNajlepszeOferty(ofertyCenowe);

            //Assert

            Assert.AreEqual(rezultat[0].PropertyPrice.PricePerMeter, 50000);
            Assert.AreEqual(rezultat[0].PropertyAddress.City.ToString(), "GDANSK");

        }
        [Test]
        public void NajlepszeOferty_SprawdzamZachowanieDlaOfertPosiadajacychCenyRowne0_BrakWynikowDlaCenZaMetrRownych0()
        {
            // Arrange

            var ofertyCenowe = NajlepszeOkazje.BrakWynikowDlaCenZaMetrRownych0();

            // Act 

            var rezultat = _okazje.ZwrocNajlepszeOferty(ofertyCenowe);

            //Assert

            Assert.IsNull(rezultat);

        }
        [Test]
        public void NajlepszeOferty_SprawdzamZachowanieDlaPustejKolekcji_BrakWynikowDlaPustejKolekcji()
        {
            // Arrange

            List<Entry> ofertyCenowe = null;

            // Act  

            var rezultat = _okazje.ZwrocNajlepszeOferty(ofertyCenowe);

            //Assert

            Assert.IsNull(rezultat);
        }
        [Test]
        public void NajlepszeOferty_ZKazdegoMiastaJestGenerowanyJedenWynik_JedenWynikNaJednoMiasto()
        {
            // Arrange

            var ofertyCenowe = NajlepszeOkazje.JedenWynikNaJednoMiasto();

            // Act 

            var rezultat = _okazje.ZwrocNajlepszeOferty(ofertyCenowe);

            //Assert

            Assert.AreEqual(rezultat.Count,3);
        }
    }
}
