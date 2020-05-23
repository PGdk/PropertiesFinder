using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Models;

namespace Exhouse.Exhouse.Comparers
{
    class PropertyPriceComparer : IEqualityComparer<PropertyPrice>
    {
        public bool Equals(PropertyPrice x, PropertyPrice y)
        {
            if (x == y)
            {
                return true;
            }

            if (null == x || null == y)
            {
                return false;
            }

            return x.TotalGrossPrice == y.TotalGrossPrice
                && x.PricePerMeter == y.PricePerMeter
                && x.ResidentalRent == y.ResidentalRent;
        }

        public int GetHashCode([DisallowNull] PropertyPrice obj)
        {
            return obj.TotalGrossPrice.GetHashCode()
                + obj.PricePerMeter.GetHashCode()
                + obj.ResidentalRent == null ? 0 : obj.ResidentalRent.GetHashCode();
        }
    }
}
