using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Application
{
    class OfferComparer : IEqualityComparer<Entry>
    {
        public bool Equals(Entry x, Entry y)
        {
            if (!x.OfferDetails.Url.Equals(y.OfferDetails.Url)) return false;

            if (!x.OfferDetails.LastUpdateDateTime.Equals(y.OfferDetails.LastUpdateDateTime)) return false;

            if (!x.PropertyAddress.City.Equals(y.PropertyAddress.City)) return false;

            if (!x.OfferDetails.SellerContact.Telephone.Equals(y.OfferDetails.SellerContact.Telephone)) return false;

            if (!x.RawDescription.Equals(y.RawDescription)) return false;

            return true;
        }

        public int GetHashCode([DisallowNull] Entry obj)
        {
            return obj.OfferDetails.Url == null ? 0 : obj.OfferDetails.Url.GetHashCode();
        }
    }
}
