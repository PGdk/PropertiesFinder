using AutoMapper;
using Models;

namespace IntegrationApi.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Entry, DatabaseConnection.Models.Entry>();
            CreateMap<OfferDetails, DatabaseConnection.Models.OfferDetails>();
            CreateMap<SellerContact, DatabaseConnection.Models.SellerContact>();
            CreateMap<PropertyDetails, DatabaseConnection.Models.PropertyDetails>();
            CreateMap<PropertyAddress, DatabaseConnection.Models.PropertyAddress>();
            CreateMap<PropertyFeatures, DatabaseConnection.Models.PropertyFeatures>();
            CreateMap<PropertyPrice, DatabaseConnection.Models.PropertyPrice>();

            CreateMap<DatabaseConnection.Models.Entry, Entry>();
            CreateMap<DatabaseConnection.Models.OfferDetails, OfferDetails>();
            CreateMap<DatabaseConnection.Models.SellerContact, SellerContact>();
            CreateMap<DatabaseConnection.Models.PropertyDetails, PropertyDetails>();
            CreateMap<DatabaseConnection.Models.PropertyAddress, PropertyAddress>();
            CreateMap<DatabaseConnection.Models.PropertyFeatures, PropertyFeatures>();
            CreateMap<DatabaseConnection.Models.PropertyPrice, PropertyPrice>();
        }
    }
}
