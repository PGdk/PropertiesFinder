using HtmlAgilityPack;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EchodniaEu
{
    class PropertyFeaturesParser : OfferParser<PropertyFeatures>
    {
        public PropertyFeaturesParser(HtmlDocument htmlDocument) : base(htmlDocument)
        {
        }

        public override PropertyFeatures Dump()
        {
            return new PropertyFeatures
            {
                GardenArea = null,
                Balconies = null,
                BasementArea = null,
                IndoorParkingPlaces = null,
                OutdoorParkingPlaces = null
            };
        }
    }
}
