using Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Application.PolskaTimes
{
    public class Comparer : IEqualityComparer<Entry>
    {
        public bool Equals(Entry x, Entry y)
        {
            // OfferDetails
            if (x.OfferDetails.Url.Equals(y.OfferDetails.Url) &&
                x.OfferDetails.CreationDateTime.Equals(y.OfferDetails.CreationDateTime) &&
                x.OfferDetails.LastUpdateDateTime.Equals(y.OfferDetails.LastUpdateDateTime) &&
                x.OfferDetails.OfferKind.Equals(y.OfferDetails.OfferKind) &&
                x.OfferDetails.SellerContact.Telephone.Equals(y.OfferDetails.SellerContact.Telephone) &&
                x.OfferDetails.SellerContact.Name.Equals(y.OfferDetails.SellerContact.Name) &&
                x.OfferDetails.IsStillValid.Equals(y.OfferDetails.IsStillValid) &&
            // PropertyPrice
                x.PropertyPrice.TotalGrossPrice.Equals(y.PropertyPrice.TotalGrossPrice) &&
                x.PropertyPrice.PricePerMeter.Equals(y.PropertyPrice.PricePerMeter) &&
                x.PropertyPrice.ResidentalRent.Equals(y.PropertyPrice.ResidentalRent) &&
            // PropertyDetails
                x.PropertyDetails.Area.Equals(y.PropertyDetails.Area) &&
                x.PropertyDetails.NumberOfRooms.Equals(y.PropertyDetails.NumberOfRooms) &&
                x.PropertyDetails.FloorNumber.Equals(y.PropertyDetails.FloorNumber) &&
                x.PropertyDetails.YearOfConstruction.Equals(y.PropertyDetails.YearOfConstruction) &&
            // PropertyAddress
                x.PropertyAddress.City.Equals(y.PropertyAddress.City) &&
                x.PropertyAddress.District.Equals(y.PropertyAddress.District) &&
                x.PropertyAddress.StreetName.Equals(y.PropertyAddress.StreetName)
                )
                return true;
            return false;
        }
        public int GetHashCode([DisallowNull] Entry obj)
        {
            var url = obj.OfferDetails.Url;
            var phone = obj.OfferDetails.SellerContact.Telephone;
            var area = obj.PropertyDetails.Area;
            var rooms = obj.PropertyDetails.NumberOfRooms;
            var floor = obj.PropertyDetails.FloorNumber;
            PolishCity city = obj.PropertyAddress.City;

            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (url == null ? 0 : url.GetHashCode());
                hash = hash * 23 + (phone == null ? 0 : phone.GetHashCode());
                hash = hash * 23 + area.GetHashCode();
                hash = hash * 23 + rooms.GetHashCode();
                hash = hash * 23 + (floor == null ? 0 : floor.GetHashCode());
                hash = hash * 23 + city.GetHashCode();

                return hash;
            }
        }
    }
}
