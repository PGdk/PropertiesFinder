using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntegrationApi;
using IntegrationApi.DataReposytory;
using System.Collections.Generic;
using Models;
using System.Linq;
using Moq;
using System.Data.Entity;
using DatabaseConnection;
using System;
using HtmlAgilityPack;
using Interfaces;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using Application;
using System.IO;

namespace UnitTestProject1
{
    [TestClass]
    public class GetBestOfferUnitTest
    {
        [TestMethod]
        public void UnexpectedReturn()
        {
            //przygotowanie
            string input = "Bia³o³êka";
            //dzia³anie
            List<EntryDB> output = new InterfaceImplementation().GetBestOffers(input).ToList();
            //weryfikacja
            Assert.IsFalse(output != null & output.Count() == 0);
        }

        [TestMethod]
        public void Output_equal_5()
        {
            //przygotowanie
            string input = "Bia³o³êka";
            //dzia³anie
            IEnumerable<EntryDB> output = new InterfaceImplementation().GetBestOffers(input).ToList();
            //weryfikacja
            Assert.AreEqual(5, output.Count(), "Nieprawid³owa iloœæ zwracanych elementów");
        }

        [TestMethod]
        public void WrongDistrictName()
        {
            //przygotowanie
            string input = "Bia³osdfkjskdalhsfuid³êka";
            //dzia³anie
            IEnumerable<EntryDB> output = new InterfaceImplementation().GetBestOffers(input);
            //weryfikacja
            Assert.AreEqual(null, output, "Z³a zwracana wartoœæ");
        }

        [TestMethod]
        public void CorrectData()
        {
            //przygotowanie
            string input = "Bia³o³êka";
            //dzia³anie
            List<EntryDB> output = new InterfaceImplementation().GetBestOffers(input).ToList();
            //weryfikacja
            Assert.AreEqual(5, output.Count());
            Assert.IsTrue(output[0].PropertyPrice.PricePerMeter <= output[1].PropertyPrice.PricePerMeter & output[2].PropertyPrice.PricePerMeter <= output[3].PropertyPrice.PricePerMeter & output[3].PropertyPrice.PricePerMeter <= output[4].PropertyPrice.PricePerMeter);
        }
    }

    [TestClass]
    public class DownloadDataWWWUnitTest
    {
        [TestMethod]
        public void FILE_FOR_TESTS_EXIST()
        {
            //przygotowanie
            string sciezka = Directory.GetCurrentDirectory() + @"\file.txt";
           // string sciezka = folder + @"\file.txt";
            //dzia³anie
            bool istniejePlik;
            istniejePlik = File.Exists(sciezka);
            //weryfikacja
            Assert.AreEqual(true, istniejePlik, "Brak pliku z testow¹ stron¹");
        }

        [TestMethod]
        public void DownloadDataWWW_Correct_price()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual(1113600, flat.cena);
        }

        [TestMethod]
        public void DownloadDataWWW_Correct_price_m2()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual(11600, flat.cena_m2);
        }

        [TestMethod]
        public void DownloadDataWWW_Correct_District()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual("Praga Pó³noc", flat.dzielnicaWies);
        }

        [TestMethod]
        public void DownloadDataWWW_Correct_Community()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual("Warszawa", flat.gmina);
        }

        [TestMethod]
        public void DownloadDataWWW_Correct_FloorNumber()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual(6, flat.iloscPieter);
        }

        [TestMethod]
        public void DownloadDataWWW_Correct_Instalations()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual("wymienione", flat.instalacje);
        }

        [TestMethod]
        public void DownloadDataWWW_Correct_PostCode()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual("-", flat.kodPocztowy);
        }

        [TestMethod]
        public void DownloadDataWWW_Correct_NumberOfRooms()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual(4, flat.liczbaPokoi);
        }

        [TestMethod]
        public void DownloadDataWWW_Correct_Link()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual("www.testowanazwa.pl/testowa_oferta.html", flat.link);
        }

        [TestMethod]
        public void DownloadDataWWW_Correct_Media()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual("pr¹d, woda, ciep³a woda miejska, telefon, internet, TV kablowa", flat.media);
        }

        [TestMethod]
        public void DownloadDataWWW_Correct_Heating()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual("miejskie", flat.ogrzewanie);
        }

        [TestMethod]
        public void DownloadDataWWW_Correct_Windows()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual("nowe PCV", flat.okna);
        }

        [TestMethod]
        public void DownloadDataWWW_Correct_Area()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual("fitness, basen, zabudowa niska, bank/bankomat, komenda policji, apteka, przychodnia/szpital, PKP, PKS, autobus, kolej podmiejska, metro, tramwaj, centrum handlowe, ¿³obek, przedszkole, plac zabaw, szko³a podstawowa", flat.okolica);
        }
        [TestMethod]
        public void DownloadDataWWW_Correct_Descryption()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual(null, flat.opis);
        }

        [TestMethod]
        public void DownloadDataWWW_Correct_PageData()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual(sourcePageString, flat.pageData);
        }

        [TestMethod]
        public void DownloadDataWWW_Correct_Flor()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual(5, flat.pietro);
        }

        [TestMethod]
        public void DownloadDataWWW_Correct_Addonal_Rooms()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual("balkon, komórka, gara¿, miejsce parkingowe", flat.pomieszczeniaDodatkowe);
        }

        [TestMethod]
        public void DownloadDataWWW_Correct_County()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual("Warszawa", flat.powiat);
        }

        [TestMethod]
        public void DownloadDataWWW_Correct_FlatArea()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual(96, flat.powierzchnia);
        }

        [TestMethod]
        public void DownloadDataWWW_Correct_BuildingType()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual("Apartamentowiec", flat.rodzajBudynku);
        }

        [TestMethod]
        public void DownloadDataWWW_Correct_BuildYear()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual(2020, flat.rokBudowy);
        }

        [TestMethod]
        public void DownloadDataWWW_Correct_Market()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual("Pierwotny", flat.rynek);
        }

        [TestMethod]
        public void DownloadDataWWW_Correct_FlatCondition()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual("do wykoñczenia", flat.stanMieszkania);
        }

        [TestMethod]
        public void DownloadDataWWW_Correct_Phone()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual("608097211", flat.telefon);
        }

        [TestMethod]
        public void DownloadDataWWW_Correct_Sterrt()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual("Szwedzka", flat.ulica);
        }

        [TestMethod]
        public void DownloadDataWWW_Correct_Voivodeship()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual("mazowieckie", flat.wojewodztwo);
        }

        [TestMethod]
        public void DownloadDataWWW_Correct_Rent()
        {
            //przygotowanie
            string strona = "www.testowanazwa.pl/testowa_oferta.html";
            int offerType = 1;
            string sourcePageString = File.ReadAllText(Directory.GetCurrentDirectory() + @"\file.txt", Encoding.UTF8);
            //dzia³anie
            Flat flat = new DownloadDataWWW().GetDataWWW(strona, offerType, sourcePageString);
            //weryfikacja
            Assert.AreEqual(0, flat.wysokoscCzynszu);
        }


    }
}
