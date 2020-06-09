using Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Application.Comparers
{
    class PropertyPriceComparer : IEqualityComparer<PropertyPrice>
    {
        public bool Equals(PropertyPrice x, PropertyPrice y)
        {
            if (x.TotalGrossPrice.Equals(y.TotalGrossPrice) &&
                x.PricePerMeter.Equals(y.PricePerMeter) &&
                x.ResidentalRent.Equals(y.ResidentalRent))
                return true;
            return false;
        }

        public int GetHashCode([DisallowNull] PropertyPrice obj)
        {
            return obj.TotalGrossPrice.GetHashCode() + obj.PricePerMeter.GetHashCode() +
                (obj.ResidentalRent == null ? 0 : obj.ResidentalRent.GetHashCode());
        }
    }
}
