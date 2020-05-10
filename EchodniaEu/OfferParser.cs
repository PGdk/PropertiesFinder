using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EchodniaEu
{
    abstract class OfferParser<T>
    {
        public HtmlDocument HtmlDocument { get; set; }

        protected string RawDescription { get; set; }

        public OfferParser(HtmlDocument htmlDocument)
        {
            HtmlDocument = htmlDocument;
            RawDescription = GetElementWithClassContent(HtmlElement.Div, "description__rolled")?.InnerText;
        }

        public abstract T Dump();

        protected string GetFieldValue(string label)
        {
            return HtmlDocument.DocumentNode
                .Descendants(HtmlElement.Span)
                .Where(span => span.InnerText.Contains(label))
                .FirstOrDefault()?
                .ParentNode
                .Descendants(HtmlElement.B)
                .First()
                .InnerText;
        }

        protected HtmlNode GetElementWithClassContent(string element, string elementClass)
        {
            return HtmlDocument.DocumentNode
                .Descendants(element)
                .Where(span => span.HasClass(elementClass))
                .FirstOrDefault();
        }

        protected HtmlNode GetElementWithId(string element, string elementId)
        {
            return HtmlDocument.DocumentNode
                .Descendants(element)
                .Where(span => span.Id == elementId)
                .FirstOrDefault();
        }

        protected decimal ParseToDecimal(string value, string divider = "z")
        {
            decimal valueAsDecimal;
            TryParseStringToDecimal(value, out valueAsDecimal, divider);
            return valueAsDecimal;
        }

        protected decimal? ParseToNullableDecimal(string value, string divider = "z")
        {
            return TryParseStringToDecimal(value, out var result, divider) ? (decimal?) result : null;
        }

        protected bool TryParseStringToDecimal(string value, out decimal result, string divider = "z")
        {
            return decimal.TryParse(
                value?
                    .Replace("\n", "")
                    .Split(divider)
                    .First()
                    .Trim()
                    .Replace(" ", ""),
                out result
            );
        }

        protected string MatchRegex(string pattern, string value, bool log = false)
        {
            if (value == null)
            {
                return null;
            }

            var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var match = regex.Match(value);

            return match.Success ? match.Value : null;
        }

        protected string[] MatchesRegex(string pattern, string value, bool log = false)
        {
            if (value == null)
            {
                return new string[0];
            }

            var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var match = regex.Matches(value);

            return match.Count > 0 ? match.Select(a => a.Value).ToArray() : new string[0];
        }
    }
}
