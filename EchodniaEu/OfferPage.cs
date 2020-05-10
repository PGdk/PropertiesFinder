using HtmlAgilityPack;
using Microsoft.VisualBasic;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EchodniaEu
{
    class OfferPage : Page
    {
        private PageParser<OfferDetails> OfferDetailsParser { get; }

        private PageParser<PropertyPrice> PropertyPriceParser { get; }
        
        private PageParser<PropertyDetails> PropertyDetailsParser { get; }

        private PageParser<PropertyAddress> PropertyAddressParser { get; }

        private string RawDescription { get
            {
                return HtmlDocument.DocumentNode
                    .Descendants(HtmlElement.Div)
                    .Where(div => div.HasClass("description__rolled"))
                    .FirstOrDefault()?
                    .InnerText;
            } 
        }


        public OfferPage(string url, HtmlNode offerHeaderNode) : base(url)
        {
            OfferDetailsParser = new OfferDetailsParser
            {
                HtmlDocument = HtmlDocument,
                OfferHeader = offerHeaderNode,
                Url = Url
            };
            PropertyPriceParser = new PropertyPriceParser
            {
                HtmlDocument = HtmlDocument
            };
            PropertyDetailsParser = new PropertyDetailsParser
            {
                HtmlDocument = HtmlDocument
            };
            PropertyAddressParser = new PropertyAddressParser
            {
                HtmlDocument = HtmlDocument
            };
        }

        public Entry Dump()
        {
            return new Entry
            {
                OfferDetails = OfferDetailsParser.Dump(),
                PropertyPrice = PropertyPriceParser.Dump(),
                PropertyDetails = PropertyDetailsParser.Dump(),
                PropertyAddress = PropertyAddressParser.Dump(),
                RawDescription = RawDescription
            };
        }

    }
}
