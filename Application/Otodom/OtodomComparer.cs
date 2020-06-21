using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Application.Otodom
{

public class OtodomComparer : IEqualityComparer<Entry>
    {
        class OfferDetailsComparer
        {

            public static bool Equals(OfferDetails x, OfferDetails y)
            {
                return x.OfferKind.Equals(y.OfferKind)
                    && x.IsStillValid.Equals(y.IsStillValid);
            }
        }
        class PropertyAddressComparer
        {
            public static bool Equals(PropertyAddress x, PropertyAddress y)
            {
                return x.City.Equals(y.City)
                    && x.StreetName.Equals(y.StreetName)
                    && x.DetailedAddress.Equals(y.DetailedAddress);
            }
        }

        class PropertyDetailsComparer
        {
            public static bool Equals(PropertyDetails x, PropertyDetails y)
            {
                return x.Area.Equals(y.Area)
                    && x.NumberOfRooms.Equals(y.NumberOfRooms)
                    && x.FloorNumber.Equals(y.FloorNumber)
                    && x.YearOfConstruction.Equals(y.YearOfConstruction);
            }
        }

        public bool Equals(Entry x, Entry y)
        {
                return OfferDetailsComparer.Equals(x.OfferDetails, y.OfferDetails)
                    && PropertyAddressComparer.Equals(x.PropertyAddress, y.PropertyAddress)
                    && PropertyDetailsComparer.Equals(x.PropertyDetails, y.PropertyDetails)
                    ;
        }

        public int GetHashCode([DisallowNull] Entry obj)
        {
            return HashCode.Combine(obj.OfferDetails, obj.PropertyDetails, obj.PropertyAddress);
        }
    }
}