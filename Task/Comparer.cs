using Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;


    public class Comparer : IEqualityComparer<Entry>
    {//powinno sie to wykonac na adresie + ew roku, i cenie
     //niestety na stronie te informacje nie sa dostepne
        public bool Equals(Entry x, Entry y)
        {
        if(GetHashCode(x) == GetHashCode(y))
        {
            return true;
        }
        else
        {
            return false;
        }
        }

        public int GetHashCode([DisallowNull] Entry obj)
        {
        var preHashString = $"{obj.PropertyPrice.TotalGrossPrice}" +
            $"{obj.PropertyPrice.PricePerMeter}" +
            $"{obj.PropertyAddress.City}" +
            $"{obj.PropertyDetails.Area}" +
            $"{obj.PropertyDetails.NumberOfRooms}";
        return preHashString.GetHashCode();
        }
    }
