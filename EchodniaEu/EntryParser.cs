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

        private OfferParser<PropertyFeatures> PropertyFeaturesParser { get; }


        public EntryParser(string url, HtmlNode offerHeaderNode): base(new HtmlWeb().Load(url))
        {
            Url = url;
            OfferDetailsParser = new OfferDetailsParser(HtmlDocument)
            {
                OfferHeader = offerHeaderNode,
                Url = Url
            };
            PropertyPriceParser = new PropertyPriceParser(HtmlDocument);
            PropertyDetailsParser = new PropertyDetailsParser(HtmlDocument);
            PropertyAddressParser = new PropertyAddressParser(HtmlDocument);
            PropertyFeaturesParser = new PropertyFeaturesParser(HtmlDocument);
        }

        public override Entry Dump()
        {
            return new Entry
            {
                OfferDetails = OfferDetailsParser.Dump(),
                PropertyPrice = PropertyPriceParser.Dump(),
                PropertyDetails = PropertyDetailsParser.Dump(),
                PropertyAddress = PropertyAddressParser.Dump(),
                PropertyFeatures = PropertyFeaturesParser.Dump(),
                RawDescription = RawDescription
            };
        }

        public bool PageExists()
        {
            return GetElementWithId(HtmlElement.Div, "offer-card") != null;
        }

    }
}
