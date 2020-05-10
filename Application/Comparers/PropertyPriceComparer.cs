using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Application.Comparers
{
    class PropertyPriceComparer : IEqualityComparer<PropertyPrice>
    {
        public bool Equals(PropertyPrice x, PropertyPrice y)
        {
            if (x == y)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.TotalGrossPrice == y.TotalGrossPrice
                && x.PricePerMeter == y.PricePerMeter
                && x.ResidentalRent == y.ResidentalRent;
        }

        public int GetHashCode([DisallowNull] PropertyPrice obj)
        {
            return (int)(obj.TotalGrossPrice
                + obj.PricePerMeter
                + obj.ResidentalRent ?? 0);
        }
    }
}
