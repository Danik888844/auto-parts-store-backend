using System.Linq.Expressions;
using AutoParts.Core.DataAccess;
using AutoParts.DataAccess.Contexts;
using AutoParts.DataAccess.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace AutoParts.DataAccess.Dals;

public interface IVehicleDal : IEntityRepository<Vehicle>
{
    Task<Vehicle?> GetWithIncludesAsync(int id);
    Task<PagedResult<Vehicle>> GetPagedWithIncludesAsync(
        Expression<Func<Vehicle, bool>>? filter = null,
        int pageNumber = 1,
        int pageSize = 20);
}

public class VehicleDal : EfEntityRepository<Vehicle, AutoPartsStoreDb>, IVehicleDal
{
    private readonly AutoPartsStoreDb _db;

    public VehicleDal(AutoPartsStoreDb context) : base(context)
    {
        _db = context;
    }

    public async Task<Vehicle?> GetWithIncludesAsync(int id)
    {
        return await _db.Set<Vehicle>()
            .Include(v => v.Model)
            .ThenInclude(m => m!.Brand)
            .FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);
    }

    public async Task<PagedResult<Vehicle>> GetPagedWithIncludesAsync(
        Expression<Func<Vehicle, bool>>? filter = null,
        int pageNumber = 1,
        int pageSize = 20)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 20;

        IQueryable<Vehicle> query = _db.Set<Vehicle>()
            .Include(v => v.Model)
            .ThenInclude(m => m!.Brand)
            .AsNoTracking()
            .Where(v => !v.IsDeleted);

        if (filter != null)
            query = query.Where(filter);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Vehicle>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
