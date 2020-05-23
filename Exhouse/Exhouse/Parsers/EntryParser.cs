using HtmlAgilityPack;
using Models;

namespace Exhouse.Exhouse.Parsers
{
    public class EntryParser
    {
        public static Entry Parse(HtmlNode documentNode)
        {
            return new Entry
            {
                OfferDetails = OfferDetailsParser.Parse(documentNode),
                PropertyPrice = PropertyPriceParser.Parse(documentNode),
                PropertyDetails = PropertyDetailsParser.Parse(documentNode),
                PropertyAddress = PropertyAddressParser.Parse(documentNode),
                PropertyFeatures = PropertyFeaturesParser.Parse(documentNode),
                RawDescription = documentNode.SelectSingleNode("//div[@class='offer-description']/div[@class='add-padding']").InnerHtml.Trim()
            };
        }
    }
}
