using System.Linq.Expressions;
using AutoParts.Core.DataAccess;
using AutoParts.DataAccess.Contexts;
using AutoParts.DataAccess.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace AutoParts.DataAccess.Dals;

public interface ISaleDal : IEntityRepository<Sale>
{
    Task<Sale?> GetWithIncludesAsync(int id);
    Task<PagedResult<Sale>> GetPagedWithIncludesAsync(
        Expression<Func<Sale, bool>>? filter = null,
        int pageNumber = 1,
        int pageSize = 20);
}

public class SaleDal : EfEntityRepository<Sale, AutoPartsStoreDb>, ISaleDal
{
    private readonly AutoPartsStoreDb _db;

    public SaleDal(AutoPartsStoreDb context) : base(context)
    {
        _db = context;
    }

    public async Task<Sale?> GetWithIncludesAsync(int id)
    {
        return await _db.Set<Sale>()
            .Include(s => s.Client)
            .Include(s => s.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
    }

    public async Task<PagedResult<Sale>> GetPagedWithIncludesAsync(
        Expression<Func<Sale, bool>>? filter = null,
        int pageNumber = 1,
        int pageSize = 20)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 20;

        IQueryable<Sale> query = _db.Set<Sale>()
            .Include(s => s.Client)
            .Include(s => s.Items)
            .ThenInclude(i => i.Product)
            .AsNoTracking()
            .Where(s => !s.IsDeleted);

        if (filter != null)
            query = query.Where(filter);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(x => x.SoldAt)
            .ThenByDescending(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Sale>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
