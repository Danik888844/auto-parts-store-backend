using AutoMapper;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels.Sale;
using AutoParts.DataAccess.Models.DtoModels.SaleItem;

namespace AutoParts.Business.Mappings;

/// <summary>
/// Маппинг Sale и SaleItem. SaleItem — ProductSku/ProductName из Product.
/// </summary>
public class SaleMappingProfile : Profile
{
    public SaleMappingProfile()
    {
        CreateMap<Sale, SaleDto>().ReverseMap();

        CreateMap<SaleItem, SaleItemDto>()
            .ForMember(d => d.ProductSku, o => o.MapFrom(s => s.Product != null ? s.Product.Sku : ""))
            .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product != null ? s.Product.Name : ""));
    }
}
