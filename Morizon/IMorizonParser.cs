using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morizon {
    public interface IMorizonParser {

        public HtmlDocument GetDocument(string url);

        public HtmlNode GetProperty(string url);

        public string GetNextUrl(HtmlDocument doc);

        public List<string> GetPropertiesUrls(HtmlDocument doc);
        public string GetDescription(HtmlNode property);


    }
}
