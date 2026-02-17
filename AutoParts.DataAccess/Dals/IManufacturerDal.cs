using AutoParts.Core.DataAccess;
using AutoParts.DataAccess.Contexts;
using AutoParts.DataAccess.Models.DatabaseModels;

namespace AutoParts.DataAccess.Dals;

public interface IManufacturerDal : IEntityRepository<Manufacturer> {}

public class ManufacturerDal : EfEntityRepository<Manufacturer, AutoPartsStoreDb>, IManufacturerDal
{
    public ManufacturerDal(AutoPartsStoreDb context) : base(context)
    {
    }
}
