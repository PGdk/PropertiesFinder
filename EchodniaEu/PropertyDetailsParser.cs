using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EchodniaEu
{
    class PropertyDetailsParser : PageParser<PropertyDetails>
    {
        private decimal Area
        {
            get
            {
                return ParseToDecimal(
                    GetFieldValue(FieldLabel.Area),
                    "m"
                );
            }
        }

        private int NumberOfRooms
        {
            get
            {
                return (int) ParseToDecimal(
                    GetFieldValue(FieldLabel.NumberOfRooms)
                );
            }
        }

        private int? FloorNumber
        {
            get
            {
                return (int?)ParseToNullableDecimal(
                    GetFieldValue(FieldLabel.FloorNumber)
                );
            }
        }

        public int? YearOfConstruction
        {
            get
            {
                return (int?)ParseToNullableDecimal(
                    GetFieldValue(FieldLabel.YearOfConstruction)
                );
            }
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
