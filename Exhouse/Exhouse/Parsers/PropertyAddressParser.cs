using System;
using Exhouse.Services;
using HtmlAgilityPack;
using Models;

namespace Exhouse.Exhouse.Parsers
{
    public class PropertyAddressParser
    {
        public static PropertyAddress Parse(HtmlNode documentNode)
        {
            HtmlNode locationNode = documentNode.SelectSingleNode("//div[@class='col-md-8 offer-head-top']/h1[@class='location']/text()");

            string[] location = DiacriticsRemover.Remove(locationNode.InnerText.Trim()).ToUpper().Split(',');

            string city = location[0].Trim().Replace(' ', '_');

            PropertyAddress propertyAddress = new PropertyAddress();

            if (Enum.IsDefined(typeof(PolishCity), city))
            {
                propertyAddress.City = (PolishCity)Enum.Parse(typeof(PolishCity), city);
            }

            if (location.Length > 1)
            {
                propertyAddress.District = location[1].Trim();
            }

            return propertyAddress;
        }
    }
}
