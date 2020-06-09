using Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Application.Comparers
{
    class PropertyAddressComparer : IEqualityComparer<PropertyAddress>
    {
        public bool Equals(PropertyAddress x, PropertyAddress y)
        {
            if (x.City.Equals(y.City) &&
                x.District.Equals(y.District) &&
                x.StreetName.Equals(y.StreetName) &&
                x.DetailedAddress.Equals(y.DetailedAddress))
                return true;
            return false;
        }

        public int GetHashCode([DisallowNull] PropertyAddress obj)
        {
            return obj.City.GetHashCode() + obj.District.GetHashCode() + obj.StreetName.GetHashCode() + obj.DetailedAddress.GetHashCode();
        }
    }
}
