using DatabaseConnection;
using Microsoft.EntityFrameworkCore;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationApi.Models
{
    public static class ObslugaBazyDanych
    {
        private static BazaDanych Db;
        public static void WprowadzDOBazy(ref List<Entry> oferty)
        {
            using (Db = new BazaDanych())
            {
                foreach (var rekord in oferty)
                {
                    Db.Entries.Add(rekord);
                }
                Db.SaveChanges();
            }
        }
        public static List<Entry> ZwrocWybraneOferty(List<Entry> oferty, int nrStrony)
        {
            int od = 20 * nrStrony;
            List<Entry> wybraneOferty = new List<Entry>();
            for (int i = od; i < od + 20; i++)
            {
                wybraneOferty.Add(oferty[i]);
            }
            return wybraneOferty;
        }
        public static List<Entry> ZwrocListeOfert(string scierzka)
        {
            Dump oferty;
            using (StreamReader dokument = new StreamReader(scierzka))
            {
                oferty = JsonConvert.DeserializeObject<Dump>(dokument.ReadToEnd());
            }
            return oferty.Entries.ToList();
        }
        public static string ZwrocScierzke()
        {
            string path = @"..\DziennikBaltycki\bin\Debug\netcoreapp3.1\Dziennik Bałtycki";
            string path2 = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), path));
            string[] Pliki = Directory.GetFiles(path2);
            return $"{path}{Pliki[1].Split("Dziennik Bałtycki")[1]}";
        }
        public static void ZapiszLogi(string value)
        {
            using (Db = new BazaDanych())
            {
                Db.Logs.Add(new Log
                {
                    Czas = DateTime.Now,
                    Zawartosc = value
                });
                Db.SaveChanges();
            }
        }
        public static List<Entry> ZwrocRekordy(int limitRekordow, int ktoraStrona)
        {
            using (Db = new BazaDanych())
            {
                return Db.Entries
                   .Where(x => x.ID >= ((limitRekordow * ktoraStrona) - limitRekordow + 1) && x.ID <= (limitRekordow * ktoraStrona))
                   .Include(x => x.OfferDetails).ThenInclude(od => od.SellerContact)
                   .Include(x => x.PropertyPrice)
                   .Include(x => x.PropertyDetails)
                   .Include(x => x.PropertyAddress)
                   .Include(x => x.PropertyFeatures)
                   .ToList();
            }
        }
        public static Entry ZwrocRekord(int id)
        {
            using (Db = new BazaDanych())
            {
                return Db.Entries
                   .Where(x => x.ID == id)
                   .Include(x => x.OfferDetails).ThenInclude(od => od.SellerContact)
                   .Include(x => x.PropertyPrice)
                   .Include(x => x.PropertyDetails)
                   .Include(x => x.PropertyAddress)
                   .Include(x => x.PropertyFeatures)
                   .Single();
            }
        }
        public static bool AktualizujRekord(Entry rekord)
        {
            bool czIstnieje = CzyIstnieje(rekord.ID);

            if(czIstnieje)
            {
                using (Db = new BazaDanych())
                {
                    Db.Entries.Update(rekord);
                    Db.SaveChanges();
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        public static bool CzyIstnieje(int id)
        {
            using (Db = new BazaDanych())
            {
                if(Db.Entries.FirstOrDefault(x => x.ID == id) != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
