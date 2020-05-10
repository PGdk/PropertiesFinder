using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Application.Comparers
{
    class PropertyFeaturesComparer : IEqualityComparer<PropertyFeatures>
    {
        public bool Equals(PropertyFeatures x, PropertyFeatures y)
        {
            if (x == y)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.GardenArea == y.GardenArea
                && x.Balconies == y.Balconies
                && x.BasementArea == y.BasementArea
                && x.OutdoorParkingPlaces == y.OutdoorParkingPlaces
                && x.IndoorParkingPlaces == y.IndoorParkingPlaces;
        }

        public int GetHashCode([DisallowNull] PropertyFeatures obj)
        {
            return (int)(obj.GardenArea ?? 0)
                + obj.Balconies ?? 0
                + (int)(obj.BasementArea ?? 0)
                + obj.OutdoorParkingPlaces ?? 0
                + obj.IndoorParkingPlaces ?? 0;
        }
    }
}
