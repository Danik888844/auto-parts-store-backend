using System.Linq.Expressions;
using AutoParts.Core.DataAccess;
using AutoParts.DataAccess.Contexts;
using AutoParts.DataAccess.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace AutoParts.DataAccess.Dals;

public interface IProductCompatibilityDal : IEntityRepository<ProductCompatibility>
{
    Task<ProductCompatibility?> GetWithIncludesAsync(int id);
    Task<PagedResult<ProductCompatibility>> GetPagedWithIncludesAsync(
        Expression<Func<ProductCompatibility, bool>>? filter = null,
        int pageNumber = 1,
        int pageSize = 20);
}

public class ProductCompatibilityDal : EfEntityRepository<ProductCompatibility, AutoPartsStoreDb>, IProductCompatibilityDal
{
    private readonly AutoPartsStoreDb _db;

    public ProductCompatibilityDal(AutoPartsStoreDb context) : base(context)
    {
        _db = context;
    }

    public async Task<ProductCompatibility?> GetWithIncludesAsync(int id)
    {
        return await _db.Set<ProductCompatibility>()
            .Include(pc => pc.Product)
            .Include(pc => pc.Vehicle)
            .ThenInclude(v => v.Model)
            .ThenInclude(m => m!.Brand)
            .FirstOrDefaultAsync(pc => pc.Id == id && !pc.IsDeleted);
    }

    public async Task<PagedResult<ProductCompatibility>> GetPagedWithIncludesAsync(
        Expression<Func<ProductCompatibility, bool>>? filter = null,
        int pageNumber = 1,
        int pageSize = 20)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 20;

        IQueryable<ProductCompatibility> query = _db.Set<ProductCompatibility>()
            .Include(pc => pc.Product)
            .Include(pc => pc.Vehicle)
            .ThenInclude(v => v.Model)
            .ThenInclude(m => m!.Brand)
            .AsNoTracking()
            .Where(pc => !pc.IsDeleted);

        if (filter != null)
            query = query.Where(filter);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<ProductCompatibility>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
