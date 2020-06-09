using Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Application.Comparers
{
    class PropertyFeaturesComparer : IEqualityComparer<PropertyFeatures>
    {
        public bool Equals(PropertyFeatures x, PropertyFeatures y)
        {
            if (x.GardenArea.Equals(y.GardenArea) &&
                x.Balconies.Equals(y.Balconies) &&
                x.BasementArea.Equals(y.BasementArea) &&
                x.OutdoorParkingPlaces.Equals(y.OutdoorParkingPlaces) &&
                x.IndoorParkingPlaces.Equals(y.IndoorParkingPlaces))
                return true;
            return false;
            /*
             * GardenArea = GardenArea,
                            Balconies = Balconies,
                            BasementArea = BasementArea,
                            OutdoorParkingPlaces = OutdoorParkingPlaces,
                            IndoorParkingPlaces = IndoorParkingPlaces
             */
        }

        public int GetHashCode([DisallowNull] PropertyFeatures obj)
        {
            return (obj.GardenArea == null ? 0 : obj.GardenArea.GetHashCode())
                + (obj.Balconies == null ? 0 : obj.Balconies.GetHashCode())
                + (obj.BasementArea == null ? 0 : obj.BasementArea.GetHashCode())
                + (obj.OutdoorParkingPlaces == null ? 0 : obj.OutdoorParkingPlaces.GetHashCode())
                + (obj.IndoorParkingPlaces == null ? 0 : obj.IndoorParkingPlaces.GetHashCode());
        }
    }
}
