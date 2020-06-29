using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace Application
{
    public class DownloadDataWWW
    {
        public string GetOfferString(string strona)
        {
            WebClient client = new WebClient();
            var sourcePageStringTMP = client.DownloadData(strona);
            string sourcePageString = Encoding.UTF8.GetString(sourcePageStringTMP);

            return sourcePageString;
        }
        public Flat GetDataWWW(string strona, int offerType, string sourcePageString)
        {
            Flat f = new Flat();
           

            var siteFlat = new HtmlDocument();
            siteFlat.LoadHtml(sourcePageString); //HTML strony

            string ClassPhone = "g-col-3 g-show-l offers-contact-phone";

            string phone = siteFlat.DocumentNode.SelectSingleNode("//div[@class='" + ClassPhone + "']").InnerText;
            phone = Regex.Replace(phone, "tel:", "");
            phone = phone.Trim();
            f.telefon = phone;
            string main = siteFlat.DocumentNode.SelectSingleNode("//main").InnerHtml;
            siteFlat.LoadHtml(main);

            f.pageData = sourcePageString;
            //link
            f.link = strona;
            //cena
            f.cena = DownloadPrice(siteFlat);
            //cena m2
            f.cena_m2 = DownloadPriceM2(siteFlat);

            if (offerType == 1)
                f = DownloadInformationFlat(siteFlat, f);
            if (offerType == 2)
                f = DownloadInformationFlatRent(siteFlat, f);

            return f;
        }

        int DownloadPrice(HtmlDocument siteFlat)
        {
            int price;
            string ClassToGet = "offers-show-price";
            string priceString = siteFlat.DocumentNode.SelectSingleNode("//p[@class='" + ClassToGet + "']").InnerText;
            price = int.Parse(Regex.Replace(priceString, "[^0-9.]", ""));
            return price;
        }

        int DownloadPriceM2(HtmlDocument siteFlat)
        {
            int price;
            string priceString;

            string ClassToGet = "offers-show-price-m";
            priceString = siteFlat.DocumentNode.SelectSingleNode("//p[@class='" + ClassToGet + "']").InnerText;
            priceString = Regex.Replace(priceString, "m2", "");
            price = int.Parse(priceString = Regex.Replace(priceString, "[^0-9.]", ""));

            return price;
        }

        Flat DownloadInformationFlat(HtmlDocument siteFlat, Flat f)
        {
            string ClassToGet = "form-group clearfix";


            var siteToProcess = siteFlat.DocumentNode.SelectNodes("//div[@class='" + ClassToGet + "']");

            int i = 1;
            foreach (var s in siteToProcess)
            {
                switch (i)
                {
                    case 1:
                        //Zadzwon
                        break;
                    case 2:
                        //Adres e-mail
                        break;
                    case 3:
                        //Województwo
                        f.wojewodztwo = Regex.Replace(s.InnerText, "Województwo", "");
                        break;
                    case 4:
                        //Powiat
                        f.powiat = Regex.Replace(s.InnerText, "Powiat", "");
                        break;
                    case 5:
                        //Miasto / Gmina
                        f.gmina = Regex.Replace(s.InnerText, "Miasto / Gmina", "");
                        break;
                    case 6:
                        //Kod pocztowy
                        f.kodPocztowy = Regex.Replace(s.InnerText, "Kod pocztowy", "");
                        break;
                    case 7:
                        //Dzielnica / Wieś
                        f.dzielnicaWies = Regex.Replace(s.InnerText, "Dzielnica / Wieś", "");
                        break;
                    case 8:
                        //Ulica
                        f.ulica = Regex.Replace(s.InnerText, "Ulica", "");
                        break;
                    case 9:
                        //Rynek
                        f.rynek = Regex.Replace(s.InnerText, "Rynek", "");
                        break;
                    case 10:
                        //Piętro
                        try
                        {
                            f.pietro = int.Parse(Regex.Replace(s.InnerText, "Piętro", ""));
                        }
                        catch
                        {
                            f.pietro = 0;
                        }
                        break;
                    case 11:
                        //Liczba pięter
                        f.iloscPieter = int.Parse(Regex.Replace(s.InnerText, "Liczba pięter", ""));
                        break;
                    case 12:
                        //Rodzaj budynku
                        f.rodzajBudynku = Regex.Replace(s.InnerText, "Rodzaj budynku", "");
                        break;
                    case 13:
                        //Rok budowy budynku
                        try
                        {
                            f.rokBudowy = int.Parse(Regex.Replace(s.InnerText, "Rok budowy budynku", ""));
                        }
                        catch
                        {
                            f.rokBudowy = 0;
                        }
                        break;
                    case 14:
                        //Okolica
                        f.okolica = Regex.Replace(s.InnerText, "Okolica", "");
                        break;
                    case 15:
                        //Wysokość czynszu 
                        try
                        {
                            f.wysokoscCzynszu = int.Parse(Regex.Replace(Regex.Replace(s.InnerText, "zł", ""), "Wysokość czynszu", ""));
                        }
                        catch
                        {
                            f.wysokoscCzynszu = 0;
                        }

                        break;
                    case 16:
                        //Liczba pokoi 

                        try
                        {
                            f.liczbaPokoi = int.Parse(Regex.Replace(s.InnerText, "Liczba pokoi", ""));
                        }
                        catch
                        {
                            f.liczbaPokoi = 0;
                        }
                        break;
                    case 17:
                        //Powierzchnia użytkowa
                        try
                        {
                            f.powierzchnia = int.Parse(Regex.Replace(Regex.Replace(s.InnerText, "m2", ""), "Powierzchnia użytkowa", ""));
                        }
                        catch
                        {
                            f.powierzchnia = 0;
                        }

                        break;
                    case 18:
                        //Stan mieszkania 
                        f.stanMieszkania = Regex.Replace(s.InnerText, "Stan mieszkania", "");
                        break;
                    case 19:
                        //Okna 
                        f.okna = Regex.Replace(s.InnerText, "Okna", "");
                        break;
                    case 20:
                        //Instalacje
                        f.instalacje = Regex.Replace(s.InnerText, "Instalacje", "");
                        break;
                    case 21:
                        //Ogrzewanie 
                        f.ogrzewanie = Regex.Replace(s.InnerText, "Ogrzewanie", "");
                        break;
                    case 22:
                        //Media 
                        f.media = Regex.Replace(s.InnerText, "Media", "");
                        break;
                    case 23:
                        //Pomieszczenia dodatkowe
                        f.pomieszczeniaDodatkowe = Regex.Replace(s.InnerText, "Pomieszczenia dodatkowe", "");
                        break;
                }

                i++;
            }

            return f;
        }

        Flat DownloadInformationFlatRent(HtmlDocument siteFlat, Flat f)
        {
            string ClassToGet = "form-group clearfix";


            var siteToProcess = siteFlat.DocumentNode.SelectNodes("//div[@class='" + ClassToGet + "']");

            int i = 1;
            foreach (var s in siteToProcess)
            {
                switch (i)
                {
                    case 1:
                        //Zadzwon
                        break;
                    case 2:
                        //Adres e-mail
                        break;
                    case 3:
                        //Województwo
                        f.wojewodztwo = Regex.Replace(s.InnerText, "Województwo", "");
                        break;
                    case 4:
                        //Powiat
                        f.powiat = Regex.Replace(s.InnerText, "Powiat", "");
                        break;
                    case 5:
                        //Miasto / Gmina
                        f.gmina = Regex.Replace(s.InnerText, "Miasto / Gmina", "");
                        break;
                    case 6:
                        //Kod pocztowy
                        f.kodPocztowy = Regex.Replace(s.InnerText, "Kod pocztowy", "");
                        break;
                    case 7:
                        //Dzielnica / Wieś
                        f.dzielnicaWies = Regex.Replace(s.InnerText, "Dzielnica / Wieś", "");
                        break;
                    case 8:
                        //Ulica
                        f.ulica = Regex.Replace(s.InnerText, "Ulica", "");
                        break;
                    case 9:
                        //Rynek
                        f.rynek = Regex.Replace(s.InnerText, "Rynek", "");
                        break;
                    case 10:
                        //Piętro
                        try
                        {
                            f.pietro = int.Parse(Regex.Replace(s.InnerText, "Piętro", ""));
                        }
                        catch
                        {
                            f.pietro = 0;
                        }
                        break;
                    case 11:
                        //Liczba pięter
                        f.iloscPieter = int.Parse(Regex.Replace(s.InnerText, "Liczba pięter", ""));
                        break;
                    case 12:
                        //Rodzaj budynku
                        f.rodzajBudynku = Regex.Replace(s.InnerText, "Rodzaj budynku", "");
                        break;
                    case 13:
                        //Rok budowy budynku
                        try
                        {
                            f.rokBudowy = int.Parse(Regex.Replace(s.InnerText, "Rok budowy budynku", ""));
                        }
                        catch
                        {
                            f.rokBudowy = 0;
                        }
                        break;
                    case 14:
                        //Okolica
                        f.okolica = Regex.Replace(s.InnerText, "Okolica", "");
                        break;
                    case 15:
                        //Liczba pokoi 

                        try
                        {
                            f.liczbaPokoi = int.Parse(Regex.Replace(s.InnerText, "Liczba pokoi", ""));
                        }
                        catch
                        {
                            f.liczbaPokoi = 0;
                        }
                        break;
                    case 16:
                        //Powierzchnia użytkowa 
                        try
                        {
                            f.powierzchnia = int.Parse(Regex.Replace(Regex.Replace(s.InnerText, "m2", ""), "Powierzchnia użytkowa", ""));
                        }
                        catch
                        {
                            f.powierzchnia = 0;
                        }

                        break;
                    case 17:
                        //Stan mieszkania 
                        f.stanMieszkania = Regex.Replace(s.InnerText, "Stan mieszkania", "");
                        break;
                    case 18:
                        //Okna 
                        f.okna = Regex.Replace(s.InnerText, "Okna", "");
                        break;
                    case 19:
                        //Instalacje 
                        f.instalacje = Regex.Replace(s.InnerText, "Instalacje", "");
                        break;
                    case 20:
                        //Ogrzewanie 
                        f.ogrzewanie = Regex.Replace(s.InnerText, "Ogrzewanie", "");
                        break;
                    case 21:
                        //Media 
                        f.media = Regex.Replace(s.InnerText, "Media", "");
                        break;
                    case 22:
                        //Pomieszczenia dodatkowe
                        f.pomieszczeniaDodatkowe = Regex.Replace(s.InnerText, "Pomieszczenia dodatkowe", "");
                        break;
                }
                i++;
            }

            return f;
        }
    }
}
