using Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DziennikBaltycki.DziennikBaltycki
{
    public class DziennikBaltyckiComparer : IEqualityComparer<Entry>
    {
        public bool Equals([AllowNull] Entry x, [AllowNull] Entry y)
        {
            if (x.GetHashCode().Equals(y.GetHashCode()))
                return true;

            return false;
        }
        public int GetHashCode([DisallowNull] Entry obj)
        {
            return obj.OfferDetails.OfferKind.GetHashCode()
                + obj.PropertyPrice.TotalGrossPrice.GetHashCode()
                + obj.PropertyPrice.ResidentalRent.GetHashCode()
                + obj.OfferDetails.SellerContact.Name != null ? obj.OfferDetails.SellerContact.Name.GetHashCode() : 0
                + obj.OfferDetails.SellerContact.Email != null ? obj.OfferDetails.SellerContact.Email.GetHashCode() : 0
                + obj.PropertyFeatures.BasementArea != null ? obj.PropertyFeatures.BasementArea.GetHashCode() : 0
                + obj.PropertyFeatures.OutdoorParkingPlaces != null ? obj.PropertyFeatures.OutdoorParkingPlaces.GetHashCode() : 0
                + obj.PropertyFeatures.IndoorParkingPlaces != null ? obj.PropertyFeatures.IndoorParkingPlaces.GetHashCode() : 0;
        }
    }

}
