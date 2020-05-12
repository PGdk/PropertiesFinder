using System;
using System.Collections.Generic;
using System.Text;
using Interfaces;
using Models;
using HtmlAgilityPack;
using System.Linq;
using Newtonsoft.Json;

namespace DomyPL
{
    public class DomyPLIntegration : IWebSiteIntegration
    {
        public WebPage WebPage { get; }
        public IDumpsRepository DumpsRepository { get; }
        public IEqualityComparer<Entry> EntriesComparer { get; }
        public DomyPLIntegration(IDumpsRepository dumpsRepository,
        IEqualityComparer<Entry> equalityComparer)
        {
            DumpsRepository = dumpsRepository;
            EntriesComparer = equalityComparer;
            WebPage = new WebPage
            {
                //Url do strony, która zawiera mieszkania na sprzedaz jest dość skomplikowany. Dlatego dzielimy go na 2 czesci
                Url = "https://domy.pl/mieszkania-sprzedaz--pl?page=",

                Name = "DomyPL WebSite Integration",
                WebPageFeatures = new WebPageFeatures
                {
                    HomeSale = true,
                    HomeRental = true,
                    HouseSale = true,
                    HouseRental = true
                }
            };
        }

        List<String> GetAllLinks(HtmlWeb htmlWeb)
        {
            List<String> links = new List<String>();

            String secondPartOfUrl = "&ps%5Badvanced_search%5D=1&ps%5Bsort_order%5D=rank&ps%5Blocation%5D%5Btype%5D=1&ps%5Blocation%5D%" +
              "5Bselect_level0%5D=10&ps%5Blocation%5D%5Bmap_help%5D=&ps%5Btransaction%5D=1&ps%5Btype%5D=1&ps%" +
              "5Breference%5D=&ps%5Bdescription%5D=&ps%5Bsort_asc%5D=0&limit=75&gallery_view=0";

            for (int i = 1; i < 10; i++)
            {
                String fullUrl = WebPage.Url + i + secondPartOfUrl;

                var document = htmlWeb.Load(fullUrl);

                //Szukamy noda headerTextBox
                var nodes = document.DocumentNode.SelectNodes("//*[@class = 'headerTextBox']");

                foreach (var node in nodes)
                { 
                    links.Add(node.SelectSingleNode("a").Attributes["href"].Value);
                    if (links.Count() == 200)
                    {
                        return links;
                    }
                }
            }

            return links;
        }
        void GetOtherDetails(HtmlNodeCollection nodes, ref Entry entry)
        {
         
            foreach (var node in nodes)
            {
                int value = 0;
                decimal decValue;
                if (node.InnerHtml.Contains(" Rok budowy:"))
                {
                    int.TryParse(node.SelectNodes("strong").FirstOrDefault().InnerHtml, out value);
                    entry.PropertyDetails.YearOfConstruction = value;
                }
                else if (node.InnerHtml.Contains(" Powierzchnia mieszkalna:"))
                {
                    decimal.TryParse(node.SelectNodes("strong").FirstOrDefault().InnerHtml.Split(" ").FirstOrDefault(), out decValue);
                    entry.PropertyDetails.Area = decValue;
                }
            }
        }

        
        List<Entry> GenerateEntries(List<String> links)
        {
            var Entries = new List<Entry>();
            var htmlWeb = new HtmlWeb();

            //Szukamy w skrypcie jsona znajdującego się pomiędzy tymi dwoma "znacznikami"
            String jsonBeginString = "__layer.push(";
            String jsonEndString = ");\nwindow['layerIsPushed']";
            foreach (var link in links)
            {

                try
                {
                    var doc = htmlWeb.Load(link);
                    var jsonObject = doc.DocumentNode.SelectSingleNode("//script[contains(text(), '" + jsonBeginString + "')]").InnerHtml;
                    //Szukamy w skrypice jsona, który zawiera interesujące nas dane. Niestety trzeba dokonać modifykacji stringa gdyż json nie jest
                    //bezposrednio dostepny
                    var index = jsonObject.IndexOf(jsonBeginString);

                    jsonObject = jsonObject.Substring(index + jsonBeginString.Length, jsonObject.Length - index - jsonBeginString.Length);

                    index = jsonObject.IndexOf(jsonEndString);
                    var finalStr = jsonObject.Substring(0, index);
                   
                    var entry = JsonConvert.DeserializeObject<JsonData>(finalStr).ToEntry();
                   
                    try
                    {
                        if (doc.DocumentNode.SelectSingleNode("//div[@id='description']") != null)
                        {
                            entry.RawDescription = doc.DocumentNode.SelectSingleNode("//div[@id='description']").InnerText;
                        }
                    }
                    catch(Exception)
                    {
                        entry.RawDescription = "";
                    }

                    GetOtherDetails(doc.DocumentNode.SelectNodes("//div[@class='paramsItem paramsItemRight']"), ref entry);
                    GetOtherDetails(doc.DocumentNode.SelectNodes("//div[@class='paramsItem paramsItemLeft']"), ref entry);

                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//*[@class = 'summaryDates']"))
                    {
                        foreach (HtmlNode node2 in node.SelectNodes(".//span"))
                        {
                            DateTime dateTime = new DateTime();
                            if (node2.InnerText.Contains("Data dodania:"))
                            {
                                DateTime.TryParse(node2.LastChild.InnerText, out dateTime);
                                entry.OfferDetails.CreationDateTime = dateTime;

                            }
                            else if (node2.InnerText.Contains("Data aktualizacji:"))
                            {
                                DateTime.TryParse(node2.LastChild.InnerText, out dateTime);
                                entry.OfferDetails.LastUpdateDateTime = dateTime;

                            }
                        }
                    }


                    Entries.Add(entry);
                }
                catch (Exception e)
                {
                   Console.Write("Error during parsing script from web: ");
                   Console.Write(e.StackTrace);
                }
            }
            return Entries;

        }

        public Dump GenerateDump()
        {
            var htmlWeb = new HtmlWeb();
            var links = GetAllLinks(htmlWeb);
            var Entries = GenerateEntries(links);

            Dump dump = new Dump();
            dump.Entries = Entries;
            dump.WebPage = WebPage;
            dump.DateTime = DateTime.Now;
            return dump;
        }
    }
}

