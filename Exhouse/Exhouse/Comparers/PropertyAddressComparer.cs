using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Models;

namespace Exhouse.Exhouse.Comparers
{
    class PropertyAddressComparer : IEqualityComparer<PropertyAddress>
    {
        public bool Equals(PropertyAddress x, PropertyAddress y)
        {
            if (x == y)
            {
                return true;
            }

            if (null == x || null == y)
            {
                return false;
            }

            return x.City == y.City
                && string.Equals(x.District, y.District)
                && string.Equals(x.StreetName, y.StreetName)
                && string.Equals(x.DetailedAddress, y.DetailedAddress);
        }

        public int GetHashCode([DisallowNull] PropertyAddress obj)
        {
            return obj.City.GetHashCode()
                + obj.District == null ? 0 : obj.District.GetHashCode()
                + obj.StreetName == null ? 0 : obj.StreetName.GetHashCode()
                + obj.DetailedAddress == null ? 0 : obj.DetailedAddress.GetHashCode();
        }
    }
}
