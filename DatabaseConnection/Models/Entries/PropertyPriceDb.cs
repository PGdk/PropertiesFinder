﻿namespace DatabaseConnection
{
    public class PropertyPriceDb
    {
        public long Id { get; set; }
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
    }
}