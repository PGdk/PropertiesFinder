using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace IntegrationApi {
    public class OccasionsFinder {
        public List<string> Cities { get; }

        // Advertisements with the lowest price per meter are searched for each city in list passed to constructor.
        // If there are no offers in the database for a given city, we skip.Only ads that contain the price are considered.
        public OccasionsFinder(List<string> cities) {
            this.Cities = cities;
        }

        public List<Entry> FindOccasions(List<Entry> entries) {
            List<Entry> occasions = new List<Entry>();


            foreach (string city in this.Cities ) {
                try {
                    PolishCity polishCity = (PolishCity)Enum.Parse(typeof(PolishCity), city.ToUpper());
                    Entry occasion = entries.Where(x => x.PropertyAddress.City.Equals(polishCity)).Where(x => x.PropertyPrice.PricePerMeter != -1).OrderBy(x => x.PropertyPrice.PricePerMeter).First();
                    occasions.Add(occasion);
                }
                catch ( System.InvalidOperationException ) {
                    // no entries with current city
                    continue;
                }
                catch ( System.ArgumentException ) {
                    // city does not exist in a PolishCity enum
                    continue;
                }
            }

            return occasions;
        }
    }
}
