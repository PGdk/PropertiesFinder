using System;
using System.Collections.Generic;
using System.Text;
using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;
namespace Application.Otodom
{
        class OtodomIntegration : IWebSiteIntegration
        {
            int maximumAds = 200; //maksymalna ilosc ofert do pobrania
            List<String> linksToAdvertisements;
            public WebPage WebPage { get; }
            public IDumpsRepository DumpsRepository { get; }
            public IEqualityComparer<Entry> EntriesComparer { get; }
            public OtodomIntegration(IDumpsRepository dumpsRepository,
            IEqualityComparer<Entry> equalityComparer)
            {
                DumpsRepository = dumpsRepository;
                EntriesComparer = equalityComparer;
                WebPage = new WebPage
                {
  
                    Url = "https://www.otodom.pl/sprzedaz/mieszkanie/?nrAdsPerPage=72",
                    Name = "Otodom WebSite Integration",
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
                linksToAdvertisements = new List<String>();
                var Entries = new List<Entry>();
                var htmlWeb = new HtmlWeb();
                int maxLoopCounter = maximumAds / 72;
                for (int i = 1; i < maxLoopCounter + 1; i++)
                {
                    if (linksToAdvertisements.Count() == maximumAds)
                    {
                        break;
                    }
                    
                    var document = htmlWeb.Load(WebPage.Url + "&page=" + i);

                    var nodes = document.DocumentNode.SelectNodes("//*[@class = 'offer-item-details']");

                    foreach (var node in nodes)
                    {
                        var htmlNode = node.Descendants("h3").Select(node => node.SelectSingleNode("a[@href]")).Where(node => node.Attributes["href"] != null).First();
                        {
                            linksToAdvertisements.Add(htmlNode.Attributes["href"].Value);
                        }
                    }
                }

                foreach (var link in linksToAdvertisements)
                {

                    var documentWithDetails = htmlWeb.Load(link);
              
                    var jsonObject = documentWithDetails.DocumentNode.Descendants().Where(n => n.Name == "script" && n.Id == "server-app-state").First();
                    OtodomJsonParser otodomJsonParser = new OtodomJsonParser();
             
                    Entries.Add(otodomJsonParser.GetEntry(jsonObject.InnerText.ToString()));
                }

                Dump dump = new Dump();
                dump.Entries = Entries;
                dump.WebPage = WebPage;
                dump.DateTime = DateTime.Now;
                return dump;
            }
        }

}