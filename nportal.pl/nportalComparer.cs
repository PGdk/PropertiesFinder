using System;
using System.Collections.Generic;
using Models;

namespace nportal.pl
{
    class NportalComparer : IEqualityComparer<Entry>
    {
        public bool Equals(Entry x, Entry y)
        {
            if (x is null || y is null) return false;
            return Equals(x.OfferDetails.Url, y.OfferDetails.Url) &&
                   Equals(x.OfferDetails.OfferKind, y.OfferDetails.OfferKind) &&
                   Equals(x.PropertyPrice.TotalGrossPrice, y.PropertyPrice.TotalGrossPrice) &&
                   Equals(x.PropertyDetails.Area, y.PropertyDetails.Area) &&
                   Equals(x.PropertyDetails.FloorNumber, y.PropertyDetails.FloorNumber) &&
                   Equals(x.PropertyDetails.NumberOfRooms, y.PropertyDetails.NumberOfRooms) &&
                   Equals(x.PropertyDetails.YearOfConstruction, y.PropertyDetails.YearOfConstruction) &&
                   Equals(x.PropertyAddress.City, y.PropertyAddress.City) &&
                   Equals(x.PropertyAddress.District, y.PropertyAddress.District) &&
                   Equals(x.PropertyAddress.StreetName, y.PropertyAddress.StreetName);
        }

        public int GetHashCode(Entry entry)
        {
            return HashCode.Combine(entry.OfferDetails, entry.PropertyPrice, entry.PropertyDetails, entry.PropertyAddress, entry.PropertyFeatures, entry.RawDescription);
        }
    }
}
