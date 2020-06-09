using Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Application.Comparers
{
    class SellerContactComparer : IEqualityComparer<SellerContact>
    {
        public bool Equals(SellerContact x, SellerContact y)
        {
            if (x.Name.Equals(y.Name) &&
                x.Telephone.Equals(y.Telephone))
                return true;
            return false;
        }

    public int GetHashCode([DisallowNull] SellerContact obj)
    {
        return (obj.Name == null ? 0 : obj.Name.GetHashCode())
                + (obj.Telephone == null ? 0 : obj.Telephone.GetHashCode());
    }
}
}
