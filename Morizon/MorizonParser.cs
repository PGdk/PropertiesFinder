using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;

namespace Morizon {
    public class MorizonParser : IMorizonParser {
        private HtmlWeb _web;


        public MorizonParser() {
            this._web = new HtmlWeb();
        }

        public virtual HtmlDocument GetDocument(string url) {
            HtmlDocument doc;

            try {
                doc = this._web.Load(url);
            }
            catch ( System.Net.WebException ) {
                return null;
            }

            return doc;
        }

        public virtual HtmlNode GetProperty(string url) {
            var property = this._web.Load(url);

            return property.DocumentNode;
        }

        public virtual string GetNextUrl(HtmlDocument doc) {
            string nextUrl = doc.DocumentNode.SelectSingleNode(".//*[contains(@title,'następna strona')]")?.Attributes["href"]?.Value;

            return nextUrl;
        }

        public virtual List<string> GetPropertiesUrls(HtmlDocument doc) {
            List<string> propertiesUrls = new List<string>();

            try {
                foreach ( HtmlNode node in doc.DocumentNode.SelectNodes("//a[@class='property_link property-url']") ) {
                    propertiesUrls.Add(node.Attributes["href"].Value);
                }
            }
            catch ( System.NullReferenceException ) {
                // no offers on page
            }

            return propertiesUrls;
        }

        public virtual string GetDescription(HtmlNode property) {
            var description = property.SelectSingleNode("//div[@class='description']")?.InnerText;

            return description != null ? description : "";
        } 

    }
}
