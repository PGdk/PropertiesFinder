using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationApi.Models
{
    public class Okazje
    {
        public List<Entry> ZwrocNajlepszeOferty(List<Entry> oferty)
        {
            if (oferty != null)
            {
                string miasto = null;
                decimal cenaZaMetr = 100000;
                int licznik = 0;

                List<Entry> NajlepszeOferty = new List<Entry>();

                Entry najlepszaOferta = null;

                foreach (var oferta in oferty)
                {

                    if (oferta.PropertyAddress.City.ToString() != miasto)
                    {
                        miasto = oferta.PropertyAddress.City.ToString();
                        cenaZaMetr = oferta.PropertyPrice.PricePerMeter == 0 ? 100000 : oferta.PropertyPrice.PricePerMeter;

                        if (najlepszaOferta != null)
                            NajlepszeOferty.Add(najlepszaOferta);

                        if (cenaZaMetr != 100000)
                            najlepszaOferta = oferta;

                    }
                    else
                    {
                        if (cenaZaMetr > oferta.PropertyPrice.PricePerMeter && oferta.PropertyPrice.PricePerMeter > 0)
                        {
                            cenaZaMetr = oferta.PropertyPrice.PricePerMeter;
                            najlepszaOferta = oferta;
                        }
                    }
                    if(++licznik == oferty.Count && oferta.PropertyAddress.City.ToString() == miasto && najlepszaOferta != null)
                    {
                        NajlepszeOferty.Add(najlepszaOferta);
                    }
                }
                return NajlepszeOferty.Count > 0 ? NajlepszeOferty : null;
            }
            return null;
        }
    }
}
