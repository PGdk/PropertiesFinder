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
            var url = obj.OfferDetails.Url == null ? 0 : obj.OfferDetails.Url.GetHashCode();
            var grossPrice = obj.PropertyPrice == null ? 0 : obj.PropertyPrice.TotalGrossPrice.GetHashCode();
            var area = obj.PropertyDetails == null ? 0 : obj.PropertyDetails.Area.GetHashCode();

            return url + grossPrice + area;
        }
    }
}
