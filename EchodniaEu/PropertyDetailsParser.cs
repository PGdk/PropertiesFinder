using HtmlAgilityPack;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EchodniaEu
{
    class PropertyDetailsParser : OfferParser<PropertyDetails>
    {
        private decimal Area
        {
            get
            {
                return ParseToDecimal(
                    GetOfferProperty(OfferPropertyLabel.Area),
                    "m"
                );
            }
        }

        private int NumberOfRooms
        {
            get
            {
                return (int) ParseToDecimal(
                    GetOfferProperty(OfferPropertyLabel.NumberOfRooms)
                );
            }
        }

        private int? FloorNumber
        {
            get
            {
                var floorNumber = GetOfferProperty(OfferPropertyLabel.FloorNumber);
                if (floorNumber == "parter")
                {
                    return 0;
                }
                return (int?)ParseToNullableDecimal(floorNumber);
            }
        }

        private int? YearOfConstruction
        {
            get
            {
                return (int?)ParseToNullableDecimal(
                    GetOfferProperty(OfferPropertyLabel.YearOfConstruction)
                );
            }
        }

        public PropertyDetailsParser(HtmlDocument htmlDocument): base(htmlDocument)
        {
        }

        public override PropertyDetails Dump()
        {
            return new PropertyDetails
            {
                Area = Area,
                NumberOfRooms = NumberOfRooms,
                FloorNumber = FloorNumber,
                YearOfConstruction = YearOfConstruction,
            };
        }
    }
}
