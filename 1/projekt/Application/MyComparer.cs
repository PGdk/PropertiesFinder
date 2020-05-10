using Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Application.Sample
{
    public class MyComparer : IEqualityComparer<Entry>
    {
        public bool Equals(Entry x, Entry y)
        {
            //adresy strony internetowej ogloszenia takie same
            if (x.OfferDetails.Url.Equals(y.OfferDetails.Url))
            {
                return true;
            }
                
            //dokladny adres mieszkania/domu taki sam
            if (x.PropertyAddress.Equals(y.PropertyAddress) && x.PropertyDetails.Equals(y.PropertyDetails))
            {
                return true;
            }

            //adres i ceny takie same
            if (x.PropertyAddress.Equals(y.PropertyAddress) && x.PropertyPrice.Equals(y.PropertyPrice)) 
            { 
                return true; 
            }
            //adres i czesciowe dane takie same, w przypadku gdy w jednym ogloszeniu nie zostaly one podane
            if (x.PropertyAddress.District.Equals(y.PropertyAddress.District) 
                && x.PropertyAddress.StreetName.Equals(y.PropertyAddress.StreetName) 
                && x.PropertyDetails.Area.Equals(y.PropertyDetails.Area) 
                && x.PropertyDetails.NumberOfRooms.Equals(y.PropertyDetails.NumberOfRooms) 
                && x.PropertyDetails.FloorNumber.Equals(y.PropertyDetails.FloorNumber) 
                && x.PropertyPrice.TotalGrossPrice.Equals(y.PropertyPrice.TotalGrossPrice))
            {
                return true;
            }
            //Opis identyczny
            if (x.RawDescription.Equals(y.RawDescription))
            {
                return true;
            }

            return false;
        }

        public int GetHashCode([DisallowNull] Entry obj)
        {
            return obj.OfferDetails.Url == null ? 0 : obj.OfferDetails.Url.GetHashCode();
        }
    }
}

