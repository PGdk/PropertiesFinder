using Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Application.Comparers
{
    class PropertyDetailsComparer : IEqualityComparer<PropertyDetails>
    {
        public bool Equals(PropertyDetails x, PropertyDetails y)
        {
            if (x.Area.Equals(y.Area) &&
                x.NumberOfRooms.Equals(y.NumberOfRooms) &&
                x.FloorNumber.Equals(y.FloorNumber) &&
                x.YearOfConstruction.Equals(y.YearOfConstruction))
                return true;
            return false;
        }

        public int GetHashCode([DisallowNull] PropertyDetails obj)
        {
            return obj.Area.GetHashCode() + obj.NumberOfRooms.GetHashCode()
                + (obj.FloorNumber == null ? 0 : obj.FloorNumber.GetHashCode())
                + (obj.YearOfConstruction == null ? 0 : obj.YearOfConstruction.GetHashCode());
        }
    }
}
