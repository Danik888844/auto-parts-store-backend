using AutoMapper;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels.Product;
using AutoParts.DataAccess.Models.DtoModels.Stock;
using AutoParts.DataAccess.Models.DtoModels.StockMovement;

namespace AutoParts.Business.Mappings;

/// <summary>
/// Маппинг Stock и StockMovement. Stock.Product — без вложенных коллекций, чтобы избежать цикла при сериализации.
/// </summary>
public class StockMappingProfile : Profile
{
    public StockMappingProfile()
    {
        CreateMap<Stock, StockDto>()
            .ForMember(d => d.Product, o => o.MapFrom(s => s.Product == null ? null : new ProductDto
            {
                Id = s.Product.Id,
                Sku = s.Product.Sku,
                Name = s.Product.Name,
                CategoryId = s.Product.CategoryId,
                ManufacturerId = s.Product.ManufacturerId,
                Price = s.Product.Price,
                PurchasePrice = s.Product.PurchasePrice,
                Description = s.Product.Description,
                IsActive = s.Product.IsActive,
                CreatedDate = s.Product.CreatedDate,
                ModifiedDate = s.Product.ModifiedDate,
                IsDeleted = s.Product.IsDeleted
            }));

        CreateMap<StockMovement, StockMovementDto>().ReverseMap();
        CreateMap<StockMovementFormDto, StockMovement>();
    }
}
