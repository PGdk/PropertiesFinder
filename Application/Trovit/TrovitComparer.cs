using Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Application.Trovit
{
    public class TrovitComparer : IEqualityComparer<Entry>
    {
        public bool Equals(Entry x, Entry y)
        {
            return GetHashCode(x) == GetHashCode(y);
        }

        public int GetHashCode([DisallowNull] Entry obj)
        {
            
            var area = obj.PropertyDetails == null ? 0 : obj.PropertyDetails.Area.GetHashCode();
            var floor = obj.PropertyDetails == null ? 0 : obj.PropertyDetails.FloorNumber.GetHashCode();
            var city = obj.PropertyAddress == null ? 0 : obj.PropertyAddress.City.GetHashCode();
            var address = obj.PropertyAddress == null ? 0 : obj.PropertyAddress.DetailedAddress.GetHashCode();
            return floor + city + area + address;
        }
    }
}
