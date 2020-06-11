using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationApi.Models
{
    public class OfferFilter
    {
        public int MinYearOfConstruction { get; set; }
        public bool IsCheapestOffer { get; set; }
        public bool IsIndoorParkingPlaces { get; set; }
    }
}
