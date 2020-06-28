using DatabaseConnection;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationApi
{
    public class BestEntryPicker
    {
        public List<Entry> Entries { get; set; }
        public PolishCity City { get; set; }
        public OfferKind OfferKind { get; set; }
        public int MaximumTotalPrice { get; set; }

        /// <summary>
        /// Metoda zwraca listę pięciu najkorzystniejszych ofert. Oferty są wybierane na podstawie
        /// najniższej ceny za metr kwadratowy. Metoda wymaga podania nazwy miasta, w którym szukamy
        /// ofert, infomracji czy szukamy ofert na wynajem czy na sprzedaż (RENTAL/SALE)
        /// i maksymalnej ceny całkowitej
        /// </summary>
        public BestEntryPicker(List<Entry> entries, PolishCity city, OfferKind offerKind, int maximumTotalPrice)
        {
            Entries = entries;
            City = city;
            OfferKind = offerKind;
            MaximumTotalPrice = maximumTotalPrice;
        }

        public List<Entry> GetBestFiveEntries()
        {
            var bestEntries = new List<Entry>();

            foreach (var entry in Entries)
            {
                if (entry == null
                    || entry.PropertyAddress.City != City
                    || entry.OfferDetails.OfferKind != OfferKind
                    || entry.PropertyPrice.TotalGrossPrice > MaximumTotalPrice)
                {
                    continue;
                }

                if (bestEntries.Count < 5)
                {
                    bestEntries.Add(entry);
                }
                else if (bestEntries[4].PropertyPrice.PricePerMeter > entry.PropertyPrice.PricePerMeter)
                {
                    bestEntries[4] = entry;
                }
                bestEntries = bestEntries.OrderBy(o => o.PropertyPrice.PricePerMeter).ToList();
            }

            return bestEntries;
        }
    }
}
