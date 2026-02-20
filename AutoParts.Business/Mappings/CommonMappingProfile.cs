using AutoMapper;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels.Client;
using AutoParts.DataAccess.Models.DtoModels.Manufacturer;
using AutoParts.DataAccess.Models.DtoModels.Product;
using AutoParts.DataAccess.Models.DtoModels.ProductCompatibility;
using AutoParts.DataAccess.Models.DtoModels.Supplier;
using AutoParts.DataAccess.Models.DtoModels.Vehicle;

namespace AutoParts.Business.Mappings;

/// <summary>
/// Маппинг сущностей без циклических ссылок или с явным разрывом: Manufacturer, Supplier, Client, ProductCompatibility.
/// Manufacturer.Products и Supplier — без вложенных коллекций при необходимости.
/// </summary>
public class CommonMappingProfile : Profile
{
    public CommonMappingProfile()
    {
        CreateMap<Manufacturer, ManufacturerDto>()
            .ForMember(d => d.Products, o => o.Ignore()) // разрыв цикла Manufacturer.Products[].Manufacturer
            .ReverseMap();
        CreateMap<Manufacturer, ManufacturerFormDto>().ReverseMap();

        CreateMap<Supplier, SupplierDto>()
            .ForMember(d => d.StockMovements, o => o.Ignore()) // разрыв цикла Supplier.StockMovements[].Supplier
            .ReverseMap();
        CreateMap<Supplier, SupplierFormDto>().ReverseMap();

        CreateMap<Client, ClientDto>()
            .ForMember(d => d.Sales, o => o.Ignore()) // разрыв цикла Client.Sales[].Client
            .ReverseMap();
        CreateMap<Client, ClientFormDto>().ReverseMap();

        // Разрыв цикла ProductCompatibility -> Product -> Compatibilities -> Product -> ...
        CreateMap<ProductCompatibility, ProductCompatibilityDto>()
            .ForMember(d => d.Product, o => o.MapFrom((src, dest, _, ctx) =>
            {
                if (src.Product == null) return null;
                var p = ctx.Mapper.Map<ProductDto>(src.Product);
                if (p != null)
                    p.Compatibilities = [];
                return p;
            }))
            .ForMember(d => d.Vehicle, o => o.MapFrom((src, dest, _, ctx) =>
            {
                if (src.Vehicle == null) return null;
                var v = ctx.Mapper.Map<VehicleDto>(src.Vehicle);
                if (v != null)
                    v.Compatibilities = [];
                return v;
            }))
            .ReverseMap();
        CreateMap<ProductCompatibilityFormDto, ProductCompatibility>();
    }
}
