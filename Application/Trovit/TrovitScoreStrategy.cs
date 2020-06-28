using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Trovit
{
    public interface ITrovitScoreStrategy {
        int Score(Entry entry);
    }

    public class TrovitScoreStrategy : ITrovitScoreStrategy
    {
        public int Score(Entry entry)
        {
            int score = 0;

            if (entry.PropertyAddress?.StreetName == null)
                return 0;

            if (entry.OfferDetails?.CreationDateTime.AddDays(14) > DateTime.Now)
                score += 50;

            if (entry.PropertyDetails?.FloorNumber < 4)
                score += 20;

            if (entry.PropertyFeatures?.Balconies > 0)
                score += 10;

            if (entry.PropertyPrice?.PricePerMeter > 0 && entry.PropertyPrice.PricePerMeter < 8000) 
                score += 20;
            
            return score;
        }
    }
}
