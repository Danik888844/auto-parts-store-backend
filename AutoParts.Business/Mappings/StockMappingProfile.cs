using AutoMapper;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels.Category;
using AutoParts.DataAccess.Models.DtoModels.Manufacturer;
using AutoParts.DataAccess.Models.DtoModels.Product;
using AutoParts.DataAccess.Models.DtoModels.Stock;
using AutoParts.DataAccess.Models.DtoModels.ProductCompatibility;
using AutoParts.DataAccess.Models.DtoModels.SaleItem;
using AutoParts.DataAccess.Models.DtoModels.StockMovement;

namespace AutoParts.Business.Mappings;

/// <summary>
/// Маппинг Stock и StockMovement. Product — без вложенных коллекций (Stock, StockMovements и т.д.), чтобы избежать цикла при сериализации.
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

        CreateMap<StockMovement, StockMovementDto>()
            .ForMember(d => d.Quantity, o => o.MapFrom(sm => sm.Quantity))
            .ForMember(d => d.Product, o => o.MapFrom(s => s.Product == null ? null! : new ProductDto
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
                IsDeleted = s.Product.IsDeleted,
                Category = s.Product.Category == null ? null! : new CategoryDto
                {
                    Id = s.Product.Category.Id,
                    Name = s.Product.Category.Name,
                    CreatedDate = s.Product.Category.CreatedDate,
                    ModifiedDate = s.Product.Category.ModifiedDate,
                    IsDeleted = s.Product.Category.IsDeleted,
                    Products = new List<ProductDto>()
                },
                Manufacturer = s.Product.Manufacturer == null ? null! : new ManufacturerDto
                {
                    Id = s.Product.Manufacturer.Id,
                    Name = s.Product.Manufacturer.Name,
                    Country = s.Product.Manufacturer.Country,
                    CreatedDate = s.Product.Manufacturer.CreatedDate,
                    ModifiedDate = s.Product.Manufacturer.ModifiedDate,
                    IsDeleted = s.Product.Manufacturer.IsDeleted,
                    Products = new List<ProductDto>()
                },
                Stock = null,
                Compatibilities = new List<ProductCompatibilityDto>(),
                SaleItems = new List<SaleItemDto>(),
                StockMovements = new List<StockMovementDto>()
            }))
            .ReverseMap();
        CreateMap<StockMovementFormDto, StockMovement>();
    }
}
