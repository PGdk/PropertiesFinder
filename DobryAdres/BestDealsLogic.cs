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
            if(!IsRealCity(city))
            {
                return null;
            }

            var cityEnum = CityToEnum(city);

            var bestOffers = entries
                .Where(entry => entry.PropertyAddress.City == cityEnum && entry.PropertyPrice.PricePerMeter > 0)
                .OrderBy(_ => _.PropertyPrice.PricePerMeter)
                .ToList();

            if(bestOffers.Count()>5)
            {
                return bestOffers.Take(5).ToList();
            }
            return bestOffers;
        }

        private bool IsRealCity(string city)
        {
            city = city.ToLower().Trim().Replace(' ', '_');
            city = TranslateCityToASCII(city).ToUpper();
            if (!Enum.IsDefined(typeof(PolishCity), city))
            {
                return false;
            }
            return true;
        }

        private PolishCity CityToEnum(string city)
        {
            city = city.ToLower().Trim().Replace(' ', '_');
            city = TranslateCityToASCII(city).ToUpper();
            return (PolishCity)Enum.Parse(typeof(PolishCity), city);
        }

        public string TranslateCityToASCII(string city)
        {
            city = city.Replace('ą', 'a');
            city = city.Replace('ę', 'e');
            city = city.Replace('ó', 'o');
            city = city.Replace('ś', 's');
            city = city.Replace('ł', 'l');
            city = city.Replace('ż', 'z');
            city = city.Replace('ź', 'z');
            city = city.Replace('ć', 'c');
            city = city.Replace('ń', 'n');
            return city;
        }
    }
}
