using System.Text.RegularExpressions;
using Exhouse.Services;
using HtmlAgilityPack;
using Models;

namespace Exhouse.Exhouse.Parsers
{
    public class PropertyFeaturesParser
    {
        private static string INDOOR_PARKING_PLACE_PATTERN = "(w garażu|hali garażowej|garaż|garażu|garażowania)";
        private static string OUTDOOR_PARKING_PLACE_PATTERN = "(miejsca parkingowe|miejsce parkingowe|parking)";

        public static PropertyFeatures Parse(HtmlNode documentNode)
        {
            PropertyFeatures propertyFeatures = new PropertyFeatures();

            HtmlNode balconyAreaNode = documentNode.SelectSingleNode("//div[@class='propsRow vir_oferta_PowierzchniaTarasBalkon']/span[@class='propValue']/text()");
            HtmlNode balconiesNumberNode = documentNode.SelectSingleNode("//div[@class='propsRow vir_oferta_ilosctarasow']/span[@class='propValue']/text()");
            HtmlNode gardenAreaNode = documentNode.SelectSingleNode("//div[@class='propsRow vir_klientoferta_powierzchniadzialki']/span[@class='propValue']/text()");

            string rawContent = documentNode.SelectSingleNode("//div[@class='offer-description']/div[@class='add-padding']").InnerText.Trim();

            if (null != balconiesNumberNode)
            {
                propertyFeatures.Balconies = (int)NumberFromTextExtractor.Extract(balconiesNumberNode.InnerText);
            }
            else if (null != balconyAreaNode)
            {
                propertyFeatures.Balconies = 1;
            }

            if (null != gardenAreaNode)
            {
                propertyFeatures.GardenArea = NumberFromTextExtractor.Extract(gardenAreaNode.InnerText);
            }

            propertyFeatures.IndoorParkingPlaces = Regex.IsMatch(rawContent, INDOOR_PARKING_PLACE_PATTERN) ? 1 : 0;
            propertyFeatures.OutdoorParkingPlaces = Regex.IsMatch(rawContent, OUTDOOR_PARKING_PLACE_PATTERN) ? 1 : 0;

            return propertyFeatures;
        }
    }
}
