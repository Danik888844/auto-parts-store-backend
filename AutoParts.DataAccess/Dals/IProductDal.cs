using System.Linq.Expressions;
using AutoParts.Core.DataAccess;
using AutoParts.DataAccess.Contexts;
using AutoParts.DataAccess.Models.DatabaseModels;

namespace AutoParts.DataAccess.Dals;

public interface IProductDal : IEntityRepository<Product>
{
    Task<Product?> GetWithIncludesAsync(int id);
    Task<PagedResult<Product>> GetPagedWithIncludesAsync(
        Expression<Func<Product, bool>>? filter,
        int pageNumber = 1,
        int pageSize = 20);
}
