using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Models;

namespace Exhouse.Exhouse.Comparers
{
    class SellerContactComparer : IEqualityComparer<SellerContact>
    {
        public bool Equals(SellerContact x, SellerContact y)
        {
            if (x == y)
            {
                return true;
            }

            if (null == x || null == y)
            {
                return false;
            }

            return string.Equals(x.Email, y.Email)
                && string.Equals(x.Telephone, y.Telephone)
                && string.Equals(x.Name, y.Name);
        }

        public int GetHashCode([DisallowNull] SellerContact obj)
        {
            return obj.Email == null ? 0 : obj.Email.GetHashCode()
                + obj.Telephone == null ? 0 : obj.Telephone.GetHashCode()
                + obj.Name == null ? 0 : obj.Name.GetHashCode();
        }
    }
}
