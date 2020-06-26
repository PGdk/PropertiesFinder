using System;
using System.Collections.Generic;

namespace Models
{
    public class PropertyPrice
    {
        public int ID { get; set; }
        /// <summary>
        /// Cenna brutto oferty. Nie powinna uwzględniać opłat eksploatacyjnych/czynszu itp. przy wynajmie
        /// </summary>
        public decimal TotalGrossPrice { get; set; }

        /// <summary>
        /// Cena przypadająca na jeden metr powierchni mieszkalnej
        /// </summary>
        public decimal PricePerMeter { get; set; }

        /// <summary>
        /// Szacowany koszt miesięcznych opłat.
        /// </summary>
        public decimal? ResidentalRent { get; set; }

        protected bool Equals(PropertyPrice other)
        {
            return ID == other.ID && TotalGrossPrice == other.TotalGrossPrice && PricePerMeter == other.PricePerMeter && ResidentalRent == other.ResidentalRent;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PropertyPrice)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, TotalGrossPrice, PricePerMeter, ResidentalRent);
        }
    }
}