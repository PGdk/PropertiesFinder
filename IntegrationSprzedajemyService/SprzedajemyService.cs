using System.Collections.Generic;
using System.Linq;
using DatabaseConnection;
using Models;

namespace IntegrationSprzedajemyService
{
    public class SprzedajemyService: ISprzedajemyService
    {
        private readonly ISprzedajemyRepository dbRepository;
        /**
         *  Najlepsze 5 ofert bedzie liczone na zasadzie:
         *  
         *  Pole street (niepuste) - waga 5
         *  Pole balconies (> 0) - waga 10
         *  Pole outdoorParkingPlaces (> 0) - waga 15
         *  Pole price per meter < 10000 - waga 20
         * 
         *  Kazda waga mnozona x100 jezeli pole spelnia kryteria podane wyzej. Suma dzielona jest przez sume wag.
         *  Najlepsza 5 ofert bedzie posortowana wedlug powyzszego wyniku.
         */
        public SprzedajemyService(ISprzedajemyRepository dbRepository)
        {
            this.dbRepository = dbRepository;
        }

        public IEnumerable<Entry> GetSpecialOffers()
        {
            IDictionary<Entry, double> bestOfferDict = new Dictionary<Entry, double>();
            List<Entry> allDbOffers = this.dbRepository.GetEntries().ToList();

            allDbOffers.ForEach(singleOffer =>
            {
                var points = CalculatePoints(singleOffer);
                bestOfferDict.Add(singleOffer, points);
            });

            var allSpecialOffers = (from offer
                                     in bestOfferDict
                                     orderby offer.Value
                                     descending
                                     select offer.Key)
                                     .Take(5)
                                     .ToList();

            if (allSpecialOffers.Count == 0)
                return null;

            return allSpecialOffers;
        }

        public double CalculatePoints(Entry entry)
        {
            var points = 0;

            if (entry.PropertyPrice?.PricePerMeter < 10000 && entry.PropertyPrice?.PricePerMeter > 0)
                points += 20 * 100;

            if (entry.PropertyFeatures?.Balconies > 0)
                points += 10 * 100;

            if (entry.PropertyFeatures?.OutdoorParkingPlaces > 0)
                points += 15 * 100;

            if (entry.PropertyAddress?.StreetName != null)
                points += 5 * 100;

            return points / 50;
        }
    }
}
