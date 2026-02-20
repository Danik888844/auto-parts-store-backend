using AutoMapper;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels.Vehicle;
using AutoParts.DataAccess.Models.DtoModels.VehicleBrand;
using AutoParts.DataAccess.Models.DtoModels.VehicleModel;

namespace AutoParts.Business.Mappings;

/// <summary>
/// Маппинг VehicleBrand, VehicleModel, Vehicle с разрывом циклических ссылок при сериализации.
/// </summary>
public class VehicleMappingProfile : Profile
{
    public VehicleMappingProfile()
    {
        // VehicleBrand: при маппинге Models каждый VehicleModelDto.Brand — без списка Models
        CreateMap<VehicleBrand, VehicleBrandDto>()
            .ForMember(d => d.Models, o => o.MapFrom(s => s.Models.Select(m => new VehicleModelDto
            {
                Id = m.Id,
                BrandId = m.BrandId,
                Name = m.Name,
                CreatedDate = m.CreatedDate,
                ModifiedDate = m.ModifiedDate,
                IsDeleted = m.IsDeleted,
                Brand = new VehicleBrandDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    CreatedDate = s.CreatedDate,
                    ModifiedDate = s.ModifiedDate,
                    IsDeleted = s.IsDeleted,
                    Models = new List<VehicleModelDto>()
                },
                Vehicles = new List<VehicleDto>()
            })));
        CreateMap<VehicleBrand, VehicleBrandFormDto>().ReverseMap();

        // VehicleModel: Brand без Models, чтобы не было цикла Model -> Brand -> Models -> Model
        CreateMap<VehicleModel, VehicleModelDto>()
            .ForMember(d => d.Brand, o => o.MapFrom(s => s.Brand == null ? null : new VehicleBrandDto
            {
                Id = s.Brand.Id,
                Name = s.Brand.Name,
                CreatedDate = s.Brand.CreatedDate,
                ModifiedDate = s.Brand.ModifiedDate,
                IsDeleted = s.Brand.IsDeleted,
                Models = new List<VehicleModelDto>()
            }))
            .ForMember(d => d.Vehicles, o => o.MapFrom(s => new List<VehicleDto>()));
        CreateMap<VehicleModel, VehicleModelFormDto>().ReverseMap();

        // Vehicle: Model без Vehicles и с Brand без Models
        CreateMap<Vehicle, VehicleDto>()
            .ForMember(d => d.Model, o => o.MapFrom(s => s.Model == null ? null : new VehicleModelDto
            {
                Id = s.Model.Id,
                BrandId = s.Model.BrandId,
                Name = s.Model.Name,
                CreatedDate = s.Model.CreatedDate,
                ModifiedDate = s.Model.ModifiedDate,
                IsDeleted = s.Model.IsDeleted,
                Brand = s.Model.Brand == null ? null : new VehicleBrandDto
                {
                    Id = s.Model.Brand.Id,
                    Name = s.Model.Brand.Name,
                    CreatedDate = s.Model.Brand.CreatedDate,
                    ModifiedDate = s.Model.Brand.ModifiedDate,
                    IsDeleted = s.Model.Brand.IsDeleted,
                    Models = new List<VehicleModelDto>()
                },
                Vehicles = new List<VehicleDto>()
            }))
            .ForMember(d => d.Compatibilities, o => o.Ignore());
        CreateMap<Vehicle, VehicleFormDto>().ReverseMap();
    }
}
