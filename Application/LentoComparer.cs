using Application.Comparers;
using Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Application
{
    class LentoComparer : IEqualityComparer<Entry>
    {
        private OfferDetailsComparer OfferDetailsComparer = new OfferDetailsComparer();
        private PropertyPriceComparer PropertyPriceComparer = new PropertyPriceComparer();
        private PropertyDetailsComparer PropertyDetailsComparer = new PropertyDetailsComparer();
        private PropertyAddressComparer PropertyAddressComparer = new PropertyAddressComparer();
        private PropertyFeaturesComparer PropertyFeaturesComparer = new PropertyFeaturesComparer();
        public bool Equals(Entry x, Entry y)
        {
            if (OfferDetailsComparer.Equals(x.OfferDetails, y.OfferDetails) &&
                PropertyPriceComparer.Equals(x.PropertyPrice, y.PropertyPrice) &&
                PropertyDetailsComparer.Equals(x.PropertyDetails, y.PropertyDetails) &&
                PropertyAddressComparer.Equals(x.PropertyAddress, y.PropertyAddress) &&
                PropertyFeaturesComparer.Equals(x.PropertyFeatures, y.PropertyFeatures) &&
                x.RawDescription.Equals(y.RawDescription))
                return true;
            return false;
        }

        public int GetHashCode([DisallowNull] Entry obj)
        {
            return (obj.OfferDetails == null ? 0 : OfferDetailsComparer.GetHashCode(obj.OfferDetails))
                + (obj.PropertyPrice == null ? 0 : PropertyPriceComparer.GetHashCode(obj.PropertyPrice))
                + (obj.PropertyDetails == null ? 0 : PropertyDetailsComparer.GetHashCode(obj.PropertyDetails))
                + (obj.PropertyAddress == null ? 0 : PropertyAddressComparer.GetHashCode(obj.PropertyAddress))
                + (obj.PropertyFeatures == null ? 0 : PropertyFeaturesComparer.GetHashCode(obj.PropertyFeatures))
                + (obj.RawDescription == null ? 0 : obj.RawDescription.GetHashCode());
        }
    }
}
