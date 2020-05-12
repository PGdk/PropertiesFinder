using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DomyPL
{
    public class DomyPLComparer : IEqualityComparer<Entry>
    {
        //Porównujemy tylko niektóre dane 
        class CompareOfferDetails
        {
            public static bool Equals(OfferDetails x, OfferDetails y)
            {
                return x.OfferKind.Equals(y.OfferKind);
            }
        }
        class ComparePropertyAddress
        {
            public static bool Equals(PropertyAddress x, PropertyAddress y)
            {
                return x.City.Equals(y.City)
                    && x.StreetName.Equals(y.StreetName)
                    && x.DetailedAddress.Equals(y.DetailedAddress);
            }
        }

        class ComparePropertyDetails
        {
            public static bool Equals(PropertyDetails x, PropertyDetails y)
            {
                return x.Area.Equals(y.Area)
                    && x.NumberOfRooms.Equals(y.NumberOfRooms)
                    && x.FloorNumber.Equals(y.FloorNumber);
            }
        }

        public bool Equals(Entry x, Entry y)
        {
            return CompareOfferDetails.Equals(x.OfferDetails, y.OfferDetails)
                && ComparePropertyAddress.Equals(x.PropertyAddress, y.PropertyAddress)
                && ComparePropertyDetails.Equals(x.PropertyDetails, y.PropertyDetails)
                ;
        }

        public int GetHashCode([DisallowNull] Entry obj)
        {
            return HashCode.Combine(obj.PropertyPrice, obj.OfferDetails, obj.PropertyDetails, obj.PropertyAddress);
        }
    }
    
}
