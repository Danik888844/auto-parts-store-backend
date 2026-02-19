using System.Linq.Expressions;
using AutoParts.Core.DataAccess;
using AutoParts.DataAccess.Contexts;
using AutoParts.DataAccess.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace AutoParts.DataAccess.Dals;

public interface IStockDal : IEntityRepository<Stock>
{
    Task<Stock?> GetByProductIdAsync(int productId);
    Task<PagedResult<Stock>> GetPagedWithProductAsync(
        Expression<Func<Stock, bool>>? filter = null,
        int pageNumber = 1,
        int pageSize = 20);
}

public class StockDal : EfEntityRepository<Stock, AutoPartsStoreDb>, IStockDal
{
    private readonly AutoPartsStoreDb _db;

    public StockDal(AutoPartsStoreDb context) : base(context)
    {
        _db = context;
    }

    public async Task<Stock?> GetByProductIdAsync(int productId)
    {
        return await _db.Set<Stock>()
            .Include(s => s.Product)
            .FirstOrDefaultAsync(s => s.ProductId == productId && !s.IsDeleted);
    }

    public async Task<PagedResult<Stock>> GetPagedWithProductAsync(
        Expression<Func<Stock, bool>>? filter = null,
        int pageNumber = 1,
        int pageSize = 20)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 20;

        IQueryable<Stock> query = _db.Set<Stock>()
            .Include(s => s.Product)
            .AsNoTracking()
            .Where(s => !s.IsDeleted);

        if (filter != null)
            query = query.Where(filter);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Stock>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
