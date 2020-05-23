namespace DatabaseConnection
{
    public class EntryDb
    {
        public long Id { get; set; }
        public OfferDetailsDb OfferDetails { get; set; }

        public PropertyPriceDb PropertyPrice { get; set; }
    
        public PropertyDetailsDb PropertyDetails { get; set; }

        public PropertyAddressDb PropertyAddress { get; set; }

        public PropertyFeaturesDb PropertyFeatures { get; set; }

        /// <summary>
        /// Nieprzetworzony tekst z ogłoszenia
        /// </summary>
        public string RawDescription { get; set; }
    }
}