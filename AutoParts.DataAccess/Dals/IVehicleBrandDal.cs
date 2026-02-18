using AutoParts.Core.DataAccess;
using AutoParts.DataAccess.Contexts;
using AutoParts.DataAccess.Models.DatabaseModels;

namespace AutoParts.DataAccess.Dals;

public interface IVehicleBrandDal : IEntityRepository<VehicleBrand> { }

public class VehicleBrandDal : EfEntityRepository<VehicleBrand, AutoPartsStoreDb>, IVehicleBrandDal
{
    public VehicleBrandDal(AutoPartsStoreDb context) : base(context)
    {
    }
}
