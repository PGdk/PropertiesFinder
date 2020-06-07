namespace NieruchomosciOnline
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using F23.StringSimilarity;
    using Models;

    public class NieruchomosciOnlineComparer : IEqualityComparer<Entry>
    {
        public bool Equals(Entry x, Entry y)
        {
            // Check all the mandatory same fields
            if (x.OfferDetails?.OfferKind != y.OfferDetails?.OfferKind
                || x.OfferDetails?.SellerContact?.Telephone?.RemoveAllWhiteSpaces() != y.OfferDetails?.SellerContact?.Telephone?.RemoveAllWhiteSpaces()
                || x.PropertyAddress?.City != y.PropertyAddress?.City
                || (x.PropertyAddress?.District != null && y.PropertyAddress?.District != null && x.PropertyAddress?.District.Trim() != y.PropertyAddress?.District.Trim())
                || (x.PropertyAddress?.StreetName != null && y.PropertyAddress?.StreetName != null && x.PropertyAddress?.StreetName.Trim() != y.PropertyAddress?.StreetName.Trim())
                || (x.PropertyDetails?.FloorNumber != null && y.PropertyDetails?.FloorNumber != null && x.PropertyDetails?.FloorNumber != y.PropertyDetails?.FloorNumber)
                )
            {
                return false;
            }

            // Some minor area difference is allowed
            var areaDifference = x.PropertyDetails.Area - y.PropertyDetails.Area;
            if (areaDifference < -2 || areaDifference > 2)
            {
                return false;
            }

            // Compare raw description
            var rd1 = x.RawDescription.RemoveAllWhiteSpaces();
            var rd2 = y.RawDescription.RemoveAllWhiteSpaces();
            var l = new NormalizedLevenshtein();
            var normalizedDistance = l.Distance(rd1, rd2);
            if (normalizedDistance > 0.15)
            {
                return false;
            }

            return true;
        }

        public int GetHashCode([DisallowNull] Entry obj)
        {
            return obj.OfferDetails.OfferKind.GetHashCode()
                + obj.PropertyAddress.City.GetHashCode()
                + obj.PropertyAddress.District == null ? 0 : obj.PropertyAddress.District.GetHashCode()
                + obj.PropertyAddress.StreetName == null ? 0 : obj.PropertyAddress.StreetName.GetHashCode()
                + obj.PropertyDetails.FloorNumber == null ? 0 : obj.PropertyDetails.FloorNumber.GetHashCode()
                + obj.OfferDetails.SellerContact.Telephone == null ? 0 : obj.OfferDetails.SellerContact.Telephone.RemoveAllWhiteSpaces().GetHashCode();
        }
    }
}
