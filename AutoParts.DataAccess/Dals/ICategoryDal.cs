using AutoParts.Core.DataAccess;
using AutoParts.DataAccess.Contexts;
using AutoParts.DataAccess.Models.DatabaseModels;

namespace AutoParts.DataAccess.Dals;

public interface ICategoryDal : IEntityRepository<Category> {}

public class CategoryDal : EfEntityRepository<Category, AutoPartsStoreDb>, ICategoryDal
{
    public CategoryDal(AutoPartsStoreDb context) : base(context)
    {
    }
}