using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using DatabaseConnection;
using IntegrationApi.Models;

namespace IntegrationApi.Services
{
    public class GazetaKrakowskaService : IGazetaKrakowskaService
    {
        private readonly IGazetaKrakowskaRepository databaseRepository;
        /**
         *  Okazyjne oferty beda liczone na podstawie wysokosci ceny za metr, czy posiadaja miejsce parkingowe wewnątrz budynku oraz czy zostały wybudowane nie dalej jak 2015 rok
         *  
         *  Zasada przyznawania oceny:
         *  Rok budowy > 2015 - 5pkt
         *  Posiadanie miejsca parkingowego wewnatrz budynku - 10 pkt
         *  Cena za metr mniejsza niz 9000 zl - 15 pkt
         * 
         *  Przy posiadaniu jednej z powyzszych trzech opcji suma punktow mnozona bedzie przez 5 co da nam ostateczny rezultat
         *  Przy posiadaniu dwoch z powyzszych trzech opcji suma punktow mnozona bedzie przez 10 co da nam ostateczny rezultat
         *  Przy posiadaniu trzech z powyzszych trzech opcji suma punktow mnozona bedzie przez 100 co da nam ostateczny rezultat
         */
        public GazetaKrakowskaService(IGazetaKrakowskaRepository databaseRepository)
        {
            this.databaseRepository = databaseRepository;
        }

        public IEnumerable<EntryDb> GetSpecialEntries()
        {
            IDictionary<long, int> entryIdToPoints = new Dictionary<long, int>();
            List<EntryDb> allEntries = this.databaseRepository.GetEntries().ToList();

            allEntries.ForEach(entry =>
            {
                var points = GetPointsForEntry(entry);
                entryIdToPoints.Add(entry.Id, points);
            });

            var sortedKeys = (from entry in entryIdToPoints where entry.Value != 0 orderby entry.Value descending select entry.Key).Take(10);

            return allEntries.FindAll(s => sortedKeys.Contains(s.Id));
        }

        private int GetPointsForEntry(EntryDb entry)
        {
            var points = 0;
            var optionAppliedNumber = 0;

            if(entry.PropertyPrice != null && entry.PropertyPrice.PricePerMeter <= 9000)
            {
                points += 15;
                optionAppliedNumber++;
            }

            if(entry.PropertyFeatures != null && entry.PropertyFeatures.IndoorParkingPlaces != null && entry.PropertyFeatures.IndoorParkingPlaces > 0)
            {
                points += 10;
                optionAppliedNumber++;
            }

            if (entry.PropertyDetails != null && entry.PropertyDetails.YearOfConstruction != null && entry.PropertyDetails.YearOfConstruction >= 2015)
            {
                points += 5;
                optionAppliedNumber++;
            }

            if (optionAppliedNumber == 3)
                return points * 100;
            if (optionAppliedNumber == 2)
                return points * 10;
            if (optionAppliedNumber == 1)
                return points * 5;
            return points;
        }
    }
}
