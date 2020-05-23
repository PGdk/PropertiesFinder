using Exhouse.Services;
using HtmlAgilityPack;
using Models;

namespace Exhouse.Exhouse.Parsers
{
    public class PropertyPriceParser
    {
        public static PropertyPrice Parse(HtmlNode documentNode)
        {
            HtmlNode totalGrossPriceNode = documentNode.SelectSingleNode("//div[@class='property-box']/p[@class='cena']/text()");
            HtmlNode pricePerMeterNode = documentNode.SelectSingleNode("//div[@class='property-box']/p[@class='params-short']/span[last()]/text()");
            HtmlNode residentalRentNode = documentNode.SelectSingleNode("//div[@class='propsRow vir_oferta_czynszletni']/span[@class='propValue']/text()");

            PropertyPrice propertyPrice = new PropertyPrice();

            if (null != totalGrossPriceNode)
            {
                propertyPrice.TotalGrossPrice = NumberFromTextExtractor.Extract(totalGrossPriceNode.InnerText);
            }

            if (null != pricePerMeterNode)
            {
                propertyPrice.PricePerMeter = NumberFromTextExtractor.Extract(pricePerMeterNode.InnerText);
            }

            if (null != residentalRentNode)
            {
                propertyPrice.ResidentalRent = NumberFromTextExtractor.Extract(residentalRentNode.InnerText);
            }

            return propertyPrice;
        }
    }
}
