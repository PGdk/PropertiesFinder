namespace Models
{
    public class EntryId
    {
        public int Id { get; set; }
        public OfferDetailsId OfferDetails { get; set; }

        public PropertyPriceId PropertyPrice { get; set; }
    
        public PropertyDetailsId PropertyDetails { get; set; }

        public PropertyAddressId PropertyAddress { get; set; }

        public PropertyFeaturesId PropertyFeatures { get; set; }

        /// <summary>
        /// Nieprzetworzony tekst z ogłoszenia
        /// </summary>
        public string RawDescription { get; set; }
    }
}