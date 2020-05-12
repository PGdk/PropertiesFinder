using System.Collections.Generic;
using Models;
using System.Diagnostics.CodeAnalysis;

namespace Application
{
        public class DomiportaComparer : IEqualityComparer<Entry>
        {
            public bool Equals([AllowNull] Entry x, [AllowNull] Entry y)
            {
                if (GetHashCode(x) == GetHashCode(y))
                    return true;

                return false;
            }

            public int GetHashCode([DisallowNull] Entry obj)
            {
            //return obj.OfferDetails.Url == null ? 0 : obj.OfferDetails.Url.GetHashCode();
            return obj.OfferDetails.Url == null ? 0 : obj.OfferDetails.Url.GetHashCode() +
                   obj.PropertyPrice.TotalGrossPrice.GetHashCode() +
                   obj.PropertyPrice.PricePerMeter.GetHashCode() +
                   obj.PropertyAddress.City.GetHashCode() +
                   obj.PropertyAddress.StreetName.GetHashCode() +
                   obj.PropertyDetails.Area.GetHashCode() +
                   obj.PropertyDetails.NumberOfRooms.GetHashCode() +
                   obj.PropertyDetails.YearOfConstruction.GetHashCode();
        }
        }
}
