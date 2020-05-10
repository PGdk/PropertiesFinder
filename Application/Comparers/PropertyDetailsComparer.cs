using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Application.Comparers
{
    class PropertyDetailsComparer : IEqualityComparer<PropertyDetails>
    {
        public bool Equals(PropertyDetails x, PropertyDetails y)
        {
            if (x == y)
            {
                return true;
            }

            if (x == null || y == null)
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
            return (int) obj.Area
                + obj.NumberOfRooms
                + obj.FloorNumber ?? 0
                + obj.YearOfConstruction ?? 0;
        }
    }
}
