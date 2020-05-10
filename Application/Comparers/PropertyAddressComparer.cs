using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Application.Comparers
{
    class PropertyAddressComparer : IEqualityComparer<PropertyAddress>
    {
        public bool Equals(PropertyAddress x, PropertyAddress y)
        {
            if (x == y)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.City == y.City
                && string.Equals(x.District, y.District)
                && string.Equals(x.StreetName, y.StreetName)
                && string.Equals(x.DetailedAddress, y.DetailedAddress);
        }

        public int GetHashCode(PropertyAddress obj)
        {
            return (int) obj.City
                + obj.District?.GetHashCode() ?? 0
                + obj.StreetName?.GetHashCode() ?? 0
                + obj.DetailedAddress?.GetHashCode() ?? 0;
        }
    }
}
