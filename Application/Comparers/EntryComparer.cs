using Application.Comparers;
using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Application
{
    class EntryComparer : IEqualityComparer<Entry>
    {
        private OfferDetailsComparer OfferDetailsComparer { get; set; } = new OfferDetailsComparer();

        private PropertyPriceComparer PropertyPriceComparer { get; set; } = new PropertyPriceComparer();

        private PropertyDetailsComparer PropertyDetailsComparer { get; set; } = new PropertyDetailsComparer();

        private PropertyAddressComparer PropertyAddressComparer { get; set; } = new PropertyAddressComparer();

        private PropertyFeaturesComparer PropertyFeaturesComparer { get; set; } = new PropertyFeaturesComparer();

        public bool Equals(Entry x, Entry y)
        {
            if (x == y)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return OfferDetailsComparer.Equals(x.OfferDetails, y.OfferDetails)
                && PropertyPriceComparer.Equals(x.PropertyPrice, y.PropertyPrice)
                && PropertyDetailsComparer.Equals(x.PropertyDetails, y.PropertyDetails)
                && PropertyAddressComparer.Equals(x.PropertyAddress, y.PropertyAddress)
                && PropertyFeaturesComparer.Equals(x.PropertyFeatures, y.PropertyFeatures)
                && String.Equals(x.RawDescription, y.RawDescription);
        }

        public int GetHashCode(Entry obj)
        {
            return (obj.OfferDetails != null ? OfferDetailsComparer.GetHashCode(obj.OfferDetails) : 0)
                + (obj.PropertyPrice != null ? PropertyPriceComparer.GetHashCode(obj.PropertyPrice) : 0)
                + (obj.PropertyDetails != null ? PropertyDetailsComparer.GetHashCode(obj.PropertyDetails) : 0)
                + (obj.PropertyAddress != null ? PropertyAddressComparer.GetHashCode(obj.PropertyAddress) : 0)
                + (obj.PropertyFeatures != null ? PropertyFeaturesComparer.GetHashCode(obj.PropertyFeatures) : 0)
                + (obj.RawDescription != null ? string.GetHashCode(obj.RawDescription) : 0);
        }
    }
}
