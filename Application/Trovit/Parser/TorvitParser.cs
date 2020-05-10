using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces.Trovit;
using Models;

namespace Application.Trovit
{

    internal class TrovitProviderDetails
    {
        public string Domain { get; set; }
        public string URL { get; set; }
    }

    internal class TrovitEntry : Entry
    {
        public TrovitProviderDetails ProviderDetails { get; set; }
    }

    internal class TorvitParser : ITrovitParser<List<TrovitEntry>>
    {
        public List<TrovitEntry> Parse(string content) {
            var entries = new List<TrovitEntry>();

            content = content.Replace("\n", "").Replace("\r", "");

            foreach (var element in ExtractElements(content)) {
                // var postalAddress = ExtractPostalAddress(element);
                var description = ExtractDescription(element);
                var price = ExtractPrice(element);
                var domain = ExtractDomain(element);
                var url = ExtractURL(element);
                var floorSize = ExtractFloorSize(element);

                entries.Add(new TrovitEntry { 
                    ProviderDetails = new TrovitProviderDetails { 
                        Domain = domain,
                        URL = url,
                    },
                    PropertyDetails = new PropertyDetails { 
                        Area = floorSize == "" ? 0 : Convert.ToDecimal(floorSize),
                    },
                    OfferDetails = new OfferDetails{ 
                        IsStillValid = true,
                        Url = url,
                    },
                    PropertyPrice = new PropertyPrice {
                        PricePerMeter = CalculatePricePerMeter(price, floorSize),
                        TotalGrossPrice = price == "" ? 0 : Convert.ToDecimal(price),
                    },
                    RawDescription = description,
                });
            }

            return entries;
        }

        private decimal CalculatePricePerMeter(string price, string floorSize) {
            if (price == "" || floorSize == "")
                return 0;

            return Convert.ToDecimal(price.Replace(" ", "")) / Convert.ToDecimal(floorSize.Replace(" ", ""));
        }

        private string ExtractFloorSize(string s) {
            return extract(s, "<meta itemprop=\"floorSize\" content=\"", "\"")
                .TrimStart(' ')
                .TrimEnd(' ')
                .Replace(" ", "");
        }

        private string ExtractURL(string s)
        {
            return extract(s, "<h4 itemprop=\"description\"> <a href=\"", "\"")
                .TrimStart(' ')
                .TrimEnd(' ');
        }

        private string ExtractDomain(string s)
        {
            return extract(s, "<small class=\"source\"><span>", "</span></small>")
                .TrimStart(' ')
                .TrimEnd(' ');
        }

        private string ExtractPrice(string s)
        {
            return extract(s, "<span class=\"amount\">", "</span>")
                .Replace("PLN", "")
                .TrimStart(' ')
                .TrimEnd(' ')
                .Replace(" ", "");
        }

        private string ExtractPostalAddress(string s) {
            return extract(s, "<h5 itemscope itemprop=\"address\" itemtype=\"http://schema.org/PostalAddress\">", "</h5>")
                .Replace("<span >", "")
                .Replace("</span>", "")
                .TrimStart(' ')
                .TrimEnd(' ');
        }

        private string ExtractDescription(string s)
        {
            return extract(s, "rel=\"nofollow\">", "</a>")
                .Replace("<strong>", "")
                .Replace("</strong>", "")
                .TrimStart(' ')
                .TrimEnd(' ');
        }

        private IEnumerable<string> ExtractElements(string s) {
            return extract(s, "<ul id=\"wrapper_listing\">", "</ul>")
                    .Replace("</li>", "<li>")
                    .Split("<li>")
                    .Skip(1)
                    .Reverse()
                    .Skip(1)
                    .Reverse()
                    .Where((_, i) => i % 2 == 0);
        }

        private string extract(string s, string begining, string ending)
        {
            if (s.IndexOf(begining) == -1)
                return "";

            s = s.Substring(s.IndexOf(begining) + begining.Length);

            if (s.IndexOf(ending) == -1)
                return "";

            s = s.Substring(0, s.IndexOf(ending));

            return s;
        }
    }
}
