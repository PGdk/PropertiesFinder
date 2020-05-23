using AutoMapper;
using DatabaseConnection;
using Models;

namespace IntegrationApi
{
    public class AutoMapping: Profile
    {
        public AutoMapping()
        {
            CreateMap<Entry, EntryDb>();
            CreateMap<OfferDetails, OfferDetailsDb>();
            CreateMap<SellerContact, SellerContactDb>();
            CreateMap<PropertyAddress, PropertyAddressDb>();
            CreateMap<PropertyDetails, PropertyDetailsDb>();
            CreateMap<PropertyFeatures, PropertyFeaturesDb>();
            CreateMap<PropertyPrice, PropertyPriceDb>();
        }
    }
}
