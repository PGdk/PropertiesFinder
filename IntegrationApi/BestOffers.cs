using DatabaseConnection;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationApi
{
    public class BestOffers
    {
        private List<Entry> AllEntries;
        public BestOffers()
        {
            BezposrednieIntegrationRepo repo = new BezposrednieIntegrationRepo();
            AllEntries = repo.GetEntries().ToList();
        }

/*        public List<Entry> CheapestPropertyInCentrum()
        {
            List<Entry> CheapestPropertyInCentrum = new List<Entry>();
            var AllPropInCentrum = AllEntries.FindAll(x => x.PropertyAddress.District.Contains("Centrum"));
            AllPropInCentrum = AllPropInCentrum.FindAll(x => x.OfferDetails.OfferKind == OfferKind.SALE);
            if (AllPropInCentrum.Count == 0)
            {
                return CheapestPropertyInCentrum;
            }
            else
            {
                while (CheapestPropertyInCentrum.Count < 10 && AllPropInCentrum.Count != 0)
                {
                    var property = AllPropInCentrum[0];
                    foreach (var entry in AllPropInCentrum)
                    {
                        if (entry.PropertyPrice.TotalGrossPrice <= property.PropertyPrice.TotalGrossPrice)
                        {
                            property = entry;
                        }
                    }
                    CheapestPropertyInCentrum.Add(property);
                    AllPropInCentrum.Remove(property);
                }
                return CheapestPropertyInCentrum;
            }
        }*/

        public List<Entry> CheapestInGdansk()
        {
            var AllPropertiesInGdansk = AllEntries.FindAll(x => x.PropertyAddress.City == PolishCity.GDANSK);
            AllPropertiesInGdansk = AllPropertiesInGdansk.FindAll(x => x.OfferDetails.OfferKind == OfferKind.SALE);
            List<Entry> CheapestPropertyInGdansk = new List<Entry>();
            if (AllPropertiesInGdansk.Count == 0)
            {
                return CheapestPropertyInGdansk;
            }
            else
            {
                while (CheapestPropertyInGdansk.Count < 5 && AllPropertiesInGdansk.Count != 0)
                {
                    var property = AllPropertiesInGdansk[0];
                    foreach (var entry in AllPropertiesInGdansk)
                    {
                        if (property.PropertyPrice.TotalGrossPrice >= entry.PropertyPrice.TotalGrossPrice)
                        {
                            property = entry;
                        }
                    }
                    CheapestPropertyInGdansk.Add(property);
                    AllPropertiesInGdansk.Remove(property);
                }
                return CheapestPropertyInGdansk;
            }
        }

        public List<Entry> BestOfPricest()
        {
            List<Entry> Pricest = new List<Entry>();
            var AllForSale = AllEntries.FindAll(x => x.OfferDetails.OfferKind == OfferKind.SALE);
            while (Pricest.Count < 20)
            {
                var property = AllForSale[0];
                foreach (var entry in AllForSale)
                {
                    if (property.PropertyPrice.TotalGrossPrice >= entry.PropertyPrice.TotalGrossPrice)
                    {
                        property = entry;
                    }
                }
                Pricest.Add(property);
                AllForSale.Remove(property);
            }

            List<Entry> BiggestofPricest = new List<Entry>();
            while (BiggestofPricest.Count < 5)
            {
                var property = Pricest[0];
                foreach (var entry in Pricest)
                {
                    if (property.PropertyDetails.Area <= entry.PropertyDetails.Area)
                    {
                        property = entry;
                    }
                }
                BiggestofPricest.Add(property);
                Pricest.Remove(property);
            }

            return BiggestofPricest;
        }
    }
}
