using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Application.Comparers
{
    class SellectContactComparer : IEqualityComparer<SellerContact>
    {
        public bool Equals(SellerContact x, SellerContact y)
        {
            if (x == y)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return string.Equals(x.Email, y.Email)
                && string.Equals(x.Telephone, y.Telephone)
                && string.Equals(x.Name, y.Name);
        }

        public int GetHashCode(SellerContact obj)
        {
            return obj.Email?.GetHashCode() ?? 0
                + obj.Telephone?.GetHashCode() ?? 0
                + obj.Name?.GetHashCode() ?? 0;
        }
    }
}
