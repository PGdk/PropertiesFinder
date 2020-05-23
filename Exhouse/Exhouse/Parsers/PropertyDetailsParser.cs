using Exhouse.Services;
using HtmlAgilityPack;
using Models;

namespace Exhouse.Exhouse.Parsers
{
    public class PropertyDetailsParser
    {
        public static PropertyDetails Parse(HtmlNode documentNode)
        {
            HtmlNode areaNode = documentNode.SelectSingleNode("//div[@class='property-box']/p[@class='params-short']/span[@class='area']/text()");
            HtmlNode numberOfRoomsNode = documentNode.SelectSingleNode("//div[@class='property-box']/p[@class='params-short']/span[@class='rooms']/text()");
            HtmlNode floorNumberNode = documentNode.SelectSingleNode("//div[@class='property-box']/p[@class='params-short']/span[@class='floor']/text()");
            HtmlNode yearOfConstructionNode = documentNode.SelectSingleNode("//div[@class='propsRow vir_oferta_rokbudowy']/span[@class='propValue']/text()");

            PropertyDetails propertyDetails = new PropertyDetails();

            if (null != areaNode)
            {
                propertyDetails.Area = NumberFromTextExtractor.Extract(areaNode.InnerText);
            }

            if (null != numberOfRoomsNode)
            {
                propertyDetails.NumberOfRooms = (int)NumberFromTextExtractor.Extract(numberOfRoomsNode.InnerText);
            }

            if (null != floorNumberNode)
            {
                propertyDetails.FloorNumber = floorNumberNode.InnerText.Contains("parter")
                    ? 0
                    : (int)NumberFromTextExtractor.Extract(floorNumberNode.InnerText);
            }

            if (null != yearOfConstructionNode)
            {
                propertyDetails.YearOfConstruction = (int)NumberFromTextExtractor.Extract(yearOfConstructionNode.InnerText);
            }

            return propertyDetails;
        }
    }
}
