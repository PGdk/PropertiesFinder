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
                    GetElementWithClassContent(HtmlElement.Span, "priceInfo__value")
                );
            }
        }
        private decimal PricePerMeter
        {
            get
            {
                return ParseToDecimal(
                    GetElementWithClassContent(HtmlElement.Span, "priceInfo__additional")
                );
            }
        }

        private decimal? ResidentalRent
        {
            get
            {
                return ParseToNullableDecimal(
                    GetFieldValue(FieldLabel.ResidentalRent)
                );
            }
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
