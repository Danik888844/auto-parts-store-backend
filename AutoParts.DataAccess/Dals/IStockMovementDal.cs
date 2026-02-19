using System.Linq.Expressions;
using AutoParts.Core.DataAccess;
using AutoParts.DataAccess.Contexts;
using AutoParts.DataAccess.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace AutoParts.DataAccess.Dals;

public interface IStockMovementDal : IEntityRepository<StockMovement>
{
    Task<StockMovement?> GetWithIncludesAsync(int id);
    Task<PagedResult<StockMovement>> GetPagedWithIncludesAsync(
        Expression<Func<StockMovement, bool>>? filter = null,
        int pageNumber = 1,
        int pageSize = 20);
}

public class StockMovementDal : EfEntityRepository<StockMovement, AutoPartsStoreDb>, IStockMovementDal
{
    private readonly AutoPartsStoreDb _db;

    public StockMovementDal(AutoPartsStoreDb context) : base(context)
    {
        _db = context;
    }

    public async Task<StockMovement?> GetWithIncludesAsync(int id)
    {
        return await _db.Set<StockMovement>()
            .Include(sm => sm.Product)
            .Include(sm => sm.Supplier)
            .FirstOrDefaultAsync(sm => sm.Id == id && !sm.IsDeleted);
    }

    public async Task<PagedResult<StockMovement>> GetPagedWithIncludesAsync(
        Expression<Func<StockMovement, bool>>? filter = null,
        int pageNumber = 1,
        int pageSize = 20)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 20;

        IQueryable<StockMovement> query = _db.Set<StockMovement>()
            .Include(sm => sm.Product)
            .Include(sm => sm.Supplier)
            .AsNoTracking()
            .Where(sm => !sm.IsDeleted);

        if (filter != null)
            query = query.Where(filter);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(x => x.OccurredAt)
            .ThenByDescending(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<StockMovement>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
