using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Models;

namespace Exhouse.Exhouse.Comparers
{
    public class EntryComparer : IEqualityComparer<Entry>
    {
        private OfferDetailsComparer OfferDetailsComparer { get; set; }

        private PropertyAddressComparer PropertyAddressComparer { get; set; }

        private PropertyDetailsComparer PropertyDetailsComparer { get; set; }

        private PropertyFeaturesComparer PropertyFeaturesComparer { get; set; }

        private PropertyPriceComparer PropertyPriceComparer { get; set; }

        public EntryComparer()
        {
            OfferDetailsComparer = new OfferDetailsComparer();
            PropertyAddressComparer = new PropertyAddressComparer();
            PropertyDetailsComparer = new PropertyDetailsComparer();
            PropertyFeaturesComparer = new PropertyFeaturesComparer();
            PropertyPriceComparer = new PropertyPriceComparer();
        }

        public bool Equals(Entry x, Entry y)
        {
            if (x == y)
            {
                return true;
            }

            if (null == x || null == y)
            {
                return false;
            }

            return OfferDetailsComparer.Equals(x.OfferDetails, y.OfferDetails)
                && PropertyAddressComparer.Equals(x.PropertyAddress, y.PropertyAddress)
                && PropertyDetailsComparer.Equals(x.PropertyDetails, y.PropertyDetails)
                && PropertyFeaturesComparer.Equals(x.PropertyFeatures, y.PropertyFeatures)
                && PropertyPriceComparer.Equals(x.PropertyPrice, y.PropertyPrice)
                && string.Equals(x.RawDescription, y.RawDescription);
        }

        public int GetHashCode([DisallowNull] Entry obj)
        {
            return (obj.OfferDetails == null ? 0 : OfferDetailsComparer.GetHashCode(obj.OfferDetails))
                + (obj.PropertyAddress == null ? 0 : PropertyAddressComparer.GetHashCode(obj.PropertyAddress))
                + (obj.PropertyDetails == null ? 0 : PropertyDetailsComparer.GetHashCode(obj.PropertyDetails))
                + (obj.PropertyFeatures == null ? 0 : PropertyFeaturesComparer.GetHashCode(obj.PropertyFeatures))
                + (obj.PropertyPrice == null ? 0 : PropertyPriceComparer.GetHashCode(obj.PropertyPrice))
                + (obj.RawDescription == null ? 0 : obj.RawDescription.GetHashCode());
        }
    }
}
