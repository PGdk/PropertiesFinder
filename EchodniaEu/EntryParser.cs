using HtmlAgilityPack;
using Microsoft.VisualBasic;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EchodniaEu
{
    class EntryParser : OfferParser<Entry>
    {
        public string Url { get; }

        private OfferParser<OfferDetails> OfferDetailsParser { get; }

        private OfferParser<PropertyPrice> PropertyPriceParser { get; }
        
        private OfferParser<PropertyDetails> PropertyDetailsParser { get; }

        private OfferParser<PropertyAddress> PropertyAddressParser { get; }


        public EntryParser(string url, HtmlNode offerHeaderNode)
        {
            Url = url;
            HtmlDocument = new HtmlWeb().Load(url); ;
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

        public override Entry Dump()
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

        public bool Exists()
        {
            return GetElementWithId(HtmlElement.Div, "offer-card") != null;
        }

    }
}
