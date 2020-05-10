using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace EchodniaEu
{
    abstract class OfferParser<T>
    {
        public HtmlDocument HtmlDocument { get; set; }

        public abstract T Dump();

        protected string RawDescription
        {
            get
            {
                return GetElementWithClassContent(HtmlElement.Div, "description__rolled");
            }
        }

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

        protected string GetElementWithClassContent(string element, string elementClass)
        {
            return HtmlDocument.DocumentNode
                .Descendants(element)
                .Where(span => span.HasClass(elementClass))
                .FirstOrDefault()?
                .InnerText;
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
    }
}
