using System.Linq.Expressions;
using AutoParts.Core.DataAccess;
using AutoParts.DataAccess.Contexts;
using AutoParts.DataAccess.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace AutoParts.DataAccess.Dals;

public class ProductDal : EfEntityRepository<Product, AutoPartsStoreDb>, IProductDal
{
    private readonly AutoPartsStoreDb _storeDb;

    public ProductDal(AutoPartsStoreDb context) : base(context)
    {
        _storeDb = context;
    }

    public async Task<Product?> GetWithIncludesAsync(int id)
    {
        return await _storeDb.Set<Product>()
            .Include(p => p.Category)
            .Include(p => p.Manufacturer)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task<PagedResult<Product>> GetPagedWithIncludesAsync(
        Expression<Func<Product, bool>>? filter = null,
        int pageNumber = 1,
        int pageSize = 20)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 20;

        IQueryable<Product> query = _storeDb.Set<Product>()
            .Include(p => p.Category)
            .Include(p => p.Manufacturer)
            .AsNoTracking()
            .Where(p => !p.IsDeleted);

        if (filter != null)
            query = query.Where(filter);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Product>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
