using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Schema;

namespace Application.DobryAdres
{
    public class DobryAdresComparer : IEqualityComparer<Entry>
    {
        public bool Equals(Entry x, Entry y)
        {
            if (CompareOfferDetails(x,y)
                && ComparePropertyAddress(x,y)
                && ComparePropertyPrice(x,y)
                && ComparePropertyDetails(x,y))
                return true;
            return false;
        }

        private bool ComparePropertyDetails(Entry x, Entry y)
        {
            if (x.PropertyDetails.Area.Equals(y.PropertyDetails.Area)
                && x.PropertyDetails.FloorNumber.Equals(y.PropertyDetails.FloorNumber)
                && x.PropertyDetails.NumberOfRooms.Equals(y.PropertyDetails.NumberOfRooms))
                return true;
            return false;
        }

        private bool ComparePropertyPrice(Entry x, Entry y)
        {
            if (x.PropertyPrice.TotalGrossPrice.Equals(y.PropertyPrice.TotalGrossPrice)
                && x.PropertyPrice.PricePerMeter.Equals(y.PropertyPrice.PricePerMeter))
                return true;
            return false;
        }

        private bool ComparePropertyAddress(Entry x, Entry y)
        {
            if (x.PropertyAddress.City.Equals(y.PropertyAddress.City)
                && x.PropertyAddress.District.Equals(y.PropertyAddress.District)
                && x.PropertyAddress.StreetName.Equals(y.PropertyAddress.StreetName))
                return true;
            return false;
        }

        private bool CompareOfferDetails(Entry x, Entry y)
        {
            if (x.OfferDetails.Url.Equals(y.OfferDetails.Url)
                && x.OfferDetails.OfferKind.Equals(y.OfferDetails.OfferKind)
                && x.OfferDetails.SellerContact.Name.Equals(y.OfferDetails.SellerContact.Name))
                return true;
            return false;
        }


        public int GetHashCode([DisallowNull] Entry obj)
        {
            return obj.OfferDetails.Url.GetHashCode()
                + obj.OfferDetails.OfferKind.GetHashCode()
                + obj.OfferDetails.SellerContact.Name.GetHashCode()
                + obj.PropertyAddress.City.GetHashCode()
                + obj.PropertyAddress.District.GetHashCode()
                + obj.PropertyAddress.StreetName.GetHashCode()
                + obj.PropertyPrice.TotalGrossPrice.GetHashCode()
                + obj.PropertyPrice.PricePerMeter.GetHashCode()
                + obj.PropertyDetails.Area.GetHashCode()
                + obj.PropertyDetails.FloorNumber.GetHashCode()
                + obj.PropertyDetails.NumberOfRooms.GetHashCode();
        }
    }
}
