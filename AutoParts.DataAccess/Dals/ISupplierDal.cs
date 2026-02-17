using AutoParts.Core.DataAccess;
using AutoParts.DataAccess.Contexts;
using AutoParts.DataAccess.Models.DatabaseModels;

namespace AutoParts.DataAccess.Dals;

public interface ISupplierDal : IEntityRepository<Supplier> {}

public class SupplierDal : EfEntityRepository<Supplier, AutoPartsStoreDb>, ISupplierDal
{
    public SupplierDal(AutoPartsStoreDb context) : base(context)
    {
    }
}
