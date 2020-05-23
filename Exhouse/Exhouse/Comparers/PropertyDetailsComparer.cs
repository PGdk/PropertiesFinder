using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Models;

namespace Exhouse.Exhouse.Comparers
{
    class PropertyDetailsComparer : IEqualityComparer<PropertyDetails>
    {
        public bool Equals(PropertyDetails x, PropertyDetails y)
        {
            if (x == y)
            {
                return true;
            }

            if (null == x || null == y)
            {
                return false;
            }

            return x.Area == y.Area
                && x.NumberOfRooms == y.NumberOfRooms
                && x.FloorNumber == y.FloorNumber
                && x.YearOfConstruction == y.YearOfConstruction;
        }

        public int GetHashCode([DisallowNull] PropertyDetails obj)
        {
            return obj.Area.GetHashCode()
                + obj.NumberOfRooms == null ? 0 : obj.NumberOfRooms.GetHashCode()
                + obj.FloorNumber == null ? 0 : obj.FloorNumber.GetHashCode()
                + obj.YearOfConstruction == null ? 0 : obj.YearOfConstruction.GetHashCode();
        }
    }
}
