using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Models;

namespace Exhouse.Exhouse.Comparers
{
    class PropertyFeaturesComparer : IEqualityComparer<PropertyFeatures>
    {
        public bool Equals(PropertyFeatures x, PropertyFeatures y)
        {
            if (x == y)
            {
                return true;
            }

            if (null == x || null == y)
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
            return obj.GardenArea == null ? 0 : obj.GardenArea.GetHashCode()
                + obj.Balconies == null ? 0 : obj.Balconies.GetHashCode()
                + obj.BasementArea == null ? 0 : obj.BasementArea.GetHashCode()
                + obj.OutdoorParkingPlaces == null ? 0 : obj.OutdoorParkingPlaces.GetHashCode()
                + obj.IndoorParkingPlaces == null ? 0 : obj.IndoorParkingPlaces.GetHashCode();
        }
    }
}
