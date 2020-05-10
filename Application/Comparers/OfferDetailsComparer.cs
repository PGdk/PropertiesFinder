using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Application.Comparers
{
    class OfferDetailsComparer : IEqualityComparer<OfferDetails>
    {
        private SellectContactComparer SellectContactComparer { get; set; } = new SellectContactComparer();
        public bool Equals(OfferDetails x, OfferDetails y)
        {
            if (x == y)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return string.Equals(x.Url, y.Url)
                && DateTime.Equals(x.CreationDateTime, y.CreationDateTime)
                && DateTime.Equals(x.LastUpdateDateTime, y.LastUpdateDateTime)
                && OfferKind.Equals(x.OfferKind, y.OfferKind)
                && SellectContactComparer.Equals(x.SellerContact, y.SellerContact)
                && x.IsStillValid == y.IsStillValid;
        }

        public int GetHashCode(OfferDetails obj)
        {
            return obj.CreationDateTime.GetHashCode()
                + obj.LastUpdateDateTime?.GetHashCode() ?? 0
                + obj.OfferKind.GetHashCode()
                + (obj.SellerContact != null
                    ? SellectContactComparer.GetHashCode(obj.SellerContact)
                    : 0);

        }
    }
}
