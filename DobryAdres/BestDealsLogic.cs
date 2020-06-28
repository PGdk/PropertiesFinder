using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DobryAdres
{
    public class BestDealsLogic
    {
        /*
         Wybierane jest maksymalnie 5 ofert z najlepsza cena za metr w danym miescie podanym przez uzytkownika
         np https://localhost:44368/entries/best/szczecin
            https://localhost:44368/entries/best/swinoujscie
         Dla nie istniejacego miasta lub braku ofert z podana cena za metr zwroci error 404
         */
        public List<Entry> FindBestDeals(List<Entry> entries, string city)
        {
           city = city.ToUpper().Trim().Replace(' ', '_');

            var bestOffers = entries
                .Where(entry => entry.PropertyAddress.City.ToString() == city && entry.PropertyPrice.PricePerMeter > 0)
                .OrderBy(_ => _.PropertyPrice.PricePerMeter)
                .ToList();

            if (bestOffers.Count() == 0)
            {
                return null;
            }
            if (bestOffers.Count()>5)
            {
                return bestOffers.Take(5).ToList();
            }
            return bestOffers;
        }
    }
}
