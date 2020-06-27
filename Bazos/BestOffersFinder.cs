using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bazos
{
    // - Szukam ofert wynajmu, które posiadają jednocześnie parking i balkon.
    // - Wyświetlane jest 5 najtańszych ofert (oferty są już przekazywane do tej metody posortowane po cenie)
    // - Zbieram oferty wśród Ofert z bazy danych, więc w celu przetestowania dodałem nowy
    //   endpoint /pages/{liczbaStron}, dzieki ktoremu w można od razu zparsować dużą ilość ogłoszeń
    public class BestOffersFinder
    {
        public static List<Entry> GetBestOffers(List<Entry> entries)
        {
            List<Entry> bestOffers = new List<Entry>();

            if (entries.Count==0 || entries == null)
            {
                return null;
            }

            foreach(Entry entry in entries)
            {
                if (entry.OfferDetails.OfferKind == OfferKind.RENTAL &&
                    (entry.PropertyFeatures.IndoorParkingPlaces > 0 || entry.PropertyFeatures.OutdoorParkingPlaces > 0) &&
                    entry.PropertyFeatures.Balconies > 0)
                {
                    bestOffers.Add(entry);
                }
            }

            if (bestOffers.Count == 0 || bestOffers == null)
            {
                return null;
            }

            if(bestOffers.Count>5)
            {
                bestOffers.RemoveRange(5, bestOffers.Count - 5);
            }

            return bestOffers;
        }
    }
}
