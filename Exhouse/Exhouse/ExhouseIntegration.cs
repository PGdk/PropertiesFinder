using System;
using System.Collections.Generic;
using Exhouse.Exhouse.Parsers;
using HtmlAgilityPack;
using Interfaces;
using Models;

namespace Exhouse.Exhouse
{
    class ExhouseIntegration : IWebSiteIntegration
    {
        public WebPage WebPage { get; }

        public IDumpsRepository DumpsRepository { get; }

        public IEqualityComparer<Entry> EntriesComparer { get; }

        private HtmlWeb HtmlWeb;

        public ExhouseIntegration(
            IDumpsRepository dumpsRepository,
            IEqualityComparer<Entry> entriesComparer
        ) {
            DumpsRepository = dumpsRepository;
            EntriesComparer = entriesComparer;
            WebPage = new ExhouseWebPage();
            HtmlWeb = new HtmlWeb();
        }

        public Dump GenerateDump()
        {
            List<Entry> entries = new List<Entry>();

            string pageUrl = GenerateUrl(ExhouseWebPage.OFFERS_PATH);

            while (null != pageUrl)
            {
                HtmlNode documentNode = FetchDocumentNode(pageUrl);

                foreach (HtmlNode link in documentNode.SelectNodes("//div[@class='offersListHolder']/div/div/div/a[@class='overlay-link']"))
                {
                    entries.Add(CreateEntry(GenerateUrl(link.Attributes["href"].Value)));
                }

                pageUrl = FindNextPageUrl(documentNode);
            }

            return CreateDump(entries);
        }

        private Entry CreateEntry(string url)
        {
            Entry entry = EntryParser.Parse(FetchDocumentNode(url));

            entry.OfferDetails.Url = url;

            return entry;
        }

        private Dump CreateDump(List<Entry> entries)
        {
            return new Dump
            {
                DateTime = DateTime.Now,
                WebPage = WebPage,
                Entries = entries
            };
        }

        private HtmlNode FetchDocumentNode(string url)
        {
            return HtmlWeb.Load(url).DocumentNode;
        }

        private string FindNextPageUrl(HtmlNode documentNode)
        {
            HtmlNode nextPageButton = documentNode.SelectSingleNode("//a[@aria-label='następna']");

            return null == nextPageButton ? null : GenerateUrl(nextPageButton.Attributes["href"].Value);
        }

        private string GenerateUrl(string path)
        {
            return WebPage.Url + path;
        }
    }
}
