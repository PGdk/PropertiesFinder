using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Models;

namespace Exhouse.Exhouse.Comparers
{
    class OfferDetailsComparer : IEqualityComparer<OfferDetails>
    {
        private SellerContactComparer SellerContactComparer { get; set; }

        public OfferDetailsComparer()
        {
            SellerContactComparer = new SellerContactComparer();
        }

        public bool Equals(OfferDetails x, OfferDetails y)
        {
            if (x == y)
            {
                return true;
            }

            if (null == x || null == y)
            {
                return false;
            }

            return string.Equals(x.Url, y.Url)
                && x.OfferKind == y.OfferKind
                && SellerContactComparer.Equals(x.SellerContact, y.SellerContact)
                && x.IsStillValid == y.IsStillValid;
        }

        public int GetHashCode([DisallowNull] OfferDetails obj)
        {
            return obj.Url.GetHashCode()
                + obj.OfferKind.GetHashCode()
                + (obj.SellerContact == null ? 0 : SellerContactComparer.GetHashCode(obj.SellerContact))
                + obj.IsStillValid.GetHashCode();
        }
    }
}
