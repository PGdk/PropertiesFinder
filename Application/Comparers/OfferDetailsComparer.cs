using Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Application.Comparers
{
    class OfferDetailsComparer : IEqualityComparer<OfferDetails>
    {
        private SellerContactComparer SellerContactComparer = new SellerContactComparer();
        public bool Equals(OfferDetails x, OfferDetails y)
        {
            if (x.Url.Equals(y.Url) &&
                x.CreationDateTime.Equals(y.CreationDateTime) &&
                x.LastUpdateDateTime.Equals(y.LastUpdateDateTime) &&
                x.OfferKind.Equals(y.OfferKind) &&
                 SellerContactComparer.Equals(x.SellerContact, y.SellerContact))
                return true;
            return false;
        }

        public int GetHashCode([DisallowNull] OfferDetails obj)
        {
            return (obj.Url == null ? 0 : obj.Url.GetHashCode())
                + (obj.CreationDateTime == null ? 0 : obj.CreationDateTime.GetHashCode())
                + (obj.LastUpdateDateTime == null ? 0 : obj.LastUpdateDateTime.GetHashCode())
                + (obj.OfferKind.GetHashCode())
                + (obj.SellerContact == null ? 0 : SellerContactComparer.GetHashCode(obj.SellerContact));
        }
    }
}
