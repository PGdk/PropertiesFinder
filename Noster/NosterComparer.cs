using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Application
{
    class NosterComparer : IEqualityComparer<Entry>
    {
        public bool Equals(Entry x, Entry y)
        {
            if (OfferDetailsEquals(x.OfferDetails, y.OfferDetails)
                && PropertyPriceEquals(x.PropertyPrice, y.PropertyPrice)
                && PropertyDetailsEquals(x.PropertyDetails, y.PropertyDetails)
                && PropertyAddressEquals(x.PropertyAddress, y.PropertyAddress)
                && PropertyFeaturesEquals(x.PropertyFeatures, y.PropertyFeatures)
                && RawDescriptionEquals(x.RawDescription, y.RawDescription))
                return true;
            return false;
        }

        public int GetHashCode([DisallowNull] Entry obj)
        {
            int hash = 17;
            hash += 23 * OfferDetailsGetHashCode(obj.OfferDetails);
            hash += 23 * PropertyPriceGetHashCode(obj.PropertyPrice);
            hash += 23 * PropertyDetailsGetHashCode(obj.PropertyDetails);
            hash += 23 * PropertyAddressGetHashCode(obj.PropertyAddress);
            hash += 23 * PropertyFeaturesGetHashCode(obj.PropertyFeatures);
            hash += 23 * RawDescriptionGetHashCode(obj.RawDescription);
            return hash;
        }


        private bool OfferDetailsEquals(OfferDetails x, OfferDetails y)
        {
            if (x.Url.Equals(y.Url)
                && x.CreationDateTime.Equals(y.CreationDateTime)
                && x.LastUpdateDateTime.Equals(y.LastUpdateDateTime)
                && x.OfferKind.Equals(y.OfferKind)
                && x.SellerContact.Equals(y.SellerContact)
                && x.IsStillValid.Equals(y.IsStillValid))
                return true;
            else
                return false;
        }

        private bool PropertyPriceEquals(PropertyPrice x, PropertyPrice y)
        {
            if (x.TotalGrossPrice.Equals(y.TotalGrossPrice)
                && x.PricePerMeter.Equals(y.PricePerMeter)
                && x.ResidentalRent.Equals(y.ResidentalRent))
                return true;
            return false;
        }

        private bool PropertyDetailsEquals(PropertyDetails x, PropertyDetails y)
        {
            if (x.Area.Equals(y.Area)
                && x.NumberOfRooms.Equals(y.NumberOfRooms)
                && x.FloorNumber.Equals(y.FloorNumber)
                && x.YearOfConstruction.Equals(y.YearOfConstruction))
                return true;
            return false;
        }

        private bool PropertyAddressEquals(PropertyAddress x, PropertyAddress y)
        {
            if (x.City.Equals(y.City)
                && x.District.Equals(y.District)
                && x.StreetName.Equals(y.StreetName)
                && x.DetailedAddress.Equals(y.DetailedAddress))
                return true;
            return false;
        }

        private bool PropertyFeaturesEquals(PropertyFeatures x, PropertyFeatures y)
        {
            if (x.GardenArea.Equals(y.GardenArea)
                && x.Balconies.Equals(y.Balconies)
                && x.BasementArea.Equals(y.BasementArea)
                && x.OutdoorParkingPlaces.Equals(y.OutdoorParkingPlaces)
                && x.IndoorParkingPlaces.Equals(y.IndoorParkingPlaces))
                return true;
            return false;
        }

        private bool RawDescriptionEquals(string x, string y)
        {
            if (x.Equals(y))
                return true;
            return false;
        }


        private int OfferDetailsGetHashCode([DisallowNull] OfferDetails obj)
        {
            int hash = 17;
            hash += obj.Url == null ? 0 : 23 * obj.Url.GetHashCode();
            hash += obj.CreationDateTime == null ? 0 : 23 * obj.CreationDateTime.GetHashCode();
            hash += obj.LastUpdateDateTime == null ? 0 : 23 * obj.LastUpdateDateTime.GetHashCode();
            hash += 23 * obj.OfferKind.GetHashCode();
            hash += obj.SellerContact == null ? 0 : 23 * obj.SellerContact.GetHashCode();
            hash += 23 * obj.IsStillValid.GetHashCode();
            return hash;
        }

        private int PropertyPriceGetHashCode([DisallowNull] PropertyPrice obj)
        {
            int hash = 17;
            hash += 23 * obj.TotalGrossPrice.GetHashCode();
            hash += 23 * obj.PricePerMeter.GetHashCode();
            hash += obj.ResidentalRent == null ? 0 : 23 * obj.ResidentalRent.GetHashCode();
            return hash;
        }

        private int PropertyDetailsGetHashCode([DisallowNull] PropertyDetails obj)
        {
            int hash = 17;
            hash += 23 * obj.Area.GetHashCode();
            hash += 23 * obj.NumberOfRooms.GetHashCode();
            hash += obj.FloorNumber == null ? 0 : 23 * obj.FloorNumber.GetHashCode();
            hash += obj.YearOfConstruction == null ? 0 : 23 * obj.YearOfConstruction.GetHashCode();
            return hash;
        }

        private int PropertyAddressGetHashCode([DisallowNull] PropertyAddress obj)
        {
            int hash = 17;
            hash += 23 * obj.City.GetHashCode();
            hash += obj.District == null ? 0 : 23 * obj.District.GetHashCode();
            hash += obj.StreetName == null ? 0 : 23 * obj.StreetName.GetHashCode();
            hash += obj.DetailedAddress == null ? 0 : 23 * obj.DetailedAddress.GetHashCode();
            return hash;
        }

        private int PropertyFeaturesGetHashCode([DisallowNull] PropertyFeatures obj)
        {
            int hash = 17;
            hash += obj.GardenArea == null ? 0 : 23 * obj.GardenArea.GetHashCode();
            hash += obj.Balconies == null ? 0 : 23 * obj.Balconies.GetHashCode();
            hash += obj.BasementArea == null ? 0 : 23 * obj.BasementArea.GetHashCode();
            hash += obj.OutdoorParkingPlaces == null ? 0 : 23 * obj.OutdoorParkingPlaces.GetHashCode();
            hash += obj.IndoorParkingPlaces == null ? 0 : 23 * obj.IndoorParkingPlaces.GetHashCode();
            return hash;
        }

        private int RawDescriptionGetHashCode([DisallowNull] string rawDescription)
        {
            int hash = 17;
            hash += rawDescription == null ? 0 : 23 * rawDescription.GetHashCode();
            return hash;
        }

    }
}
