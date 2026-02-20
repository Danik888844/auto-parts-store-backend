using AutoMapper;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels.Category;
using AutoParts.DataAccess.Models.DtoModels.Manufacturer;
using AutoParts.DataAccess.Models.DtoModels.Product;
using AutoParts.DataAccess.Models.DtoModels.ProductCompatibility;
using AutoParts.DataAccess.Models.DtoModels.SaleItem;
using AutoParts.DataAccess.Models.DtoModels.StockMovement;

namespace AutoParts.Business.Mappings;

/// <summary>
/// Маппинг Category и Product с разрывом циклических ссылок (Category.Products[].Category, Product.Category.Products, Manufacturer.Products).
/// </summary>
public class CategoryProductMappingProfile : Profile
{
    public CategoryProductMappingProfile()
    {
        CreateMap<Category, CategoryDto>()
            .ForMember(d => d.Products, o => o.MapFrom(s => s.Products.Select(p => new ProductDto
            {
                Id = p.Id,
                Sku = p.Sku,
                Name = p.Name,
                CategoryId = p.CategoryId,
                ManufacturerId = p.ManufacturerId,
                Price = p.Price,
                PurchasePrice = p.PurchasePrice,
                Description = p.Description,
                IsActive = p.IsActive,
                CreatedDate = p.CreatedDate,
                ModifiedDate = p.ModifiedDate,
                IsDeleted = p.IsDeleted,
                Category = new CategoryDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    CreatedDate = s.CreatedDate,
                    ModifiedDate = s.ModifiedDate,
                    IsDeleted = s.IsDeleted,
                    Products = new List<ProductDto>()
                },
                Manufacturer = p.Manufacturer == null ? null! : new ManufacturerDto
                {
                    Id = p.Manufacturer.Id,
                    Name = p.Manufacturer.Name,
                    Country = p.Manufacturer.Country,
                    CreatedDate = p.Manufacturer.CreatedDate,
                    ModifiedDate = p.Manufacturer.ModifiedDate,
                    IsDeleted = p.Manufacturer.IsDeleted,
                    Products = new List<ProductDto>()
                },
                Stock = null,
                Compatibilities = new List<ProductCompatibilityDto>(),
                SaleItems = new List<SaleItemDto>(),
                StockMovements = new List<StockMovementDto>()
            })));
        CreateMap<Category, CategoryFormDto>().ReverseMap();

        CreateMap<Product, ProductDto>()
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category == null ? null! : new CategoryDto
            {
                Id = s.Category.Id,
                Name = s.Category.Name,
                CreatedDate = s.Category.CreatedDate,
                ModifiedDate = s.Category.ModifiedDate,
                IsDeleted = s.Category.IsDeleted,
                Products = new List<ProductDto>()
            }))
            .ForMember(d => d.Manufacturer, o => o.MapFrom(s => s.Manufacturer == null ? null! : new ManufacturerDto
            {
                Id = s.Manufacturer.Id,
                Name = s.Manufacturer.Name,
                Country = s.Manufacturer.Country,
                CreatedDate = s.Manufacturer.CreatedDate,
                ModifiedDate = s.Manufacturer.ModifiedDate,
                IsDeleted = s.Manufacturer.IsDeleted,
                Products = new List<ProductDto>()
            }));
        CreateMap<Product, ProductFormCreateDto>().ReverseMap();
    }
}
