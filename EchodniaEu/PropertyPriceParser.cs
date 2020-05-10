using HtmlAgilityPack;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EchodniaEu
{
    class PropertyPriceParser : OfferParser<PropertyPrice>
    {
        private decimal TotalGrossPrice
        {
            get
            {
                return ParseToDecimal(
                    GetElementWithClassContent(HtmlElement.Span, "priceInfo__value")?.InnerText
                );
            }
        }
        private decimal PricePerMeter
        {
            get
            {
                return ParseToDecimal(
                    GetElementWithClassContent(HtmlElement.Span, "priceInfo__additional")?.InnerText
                );
            }
        }

        private decimal? ResidentalRent
        {
            get
            {
                return ParseToNullableDecimal(
                    GetOfferProperty(OfferPropertyLabel.ResidentalRent)
                );
            }
        }

        public PropertyPriceParser(HtmlDocument htmlDocument) : base(htmlDocument)
        {
        }

        public override PropertyPrice Dump()
        {
            return new PropertyPrice
            {
                TotalGrossPrice = TotalGrossPrice,
                PricePerMeter = PricePerMeter,
                ResidentalRent = ResidentalRent
            };
        }
    }
}
