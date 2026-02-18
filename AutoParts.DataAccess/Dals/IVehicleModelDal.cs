using System.Linq.Expressions;
using AutoParts.Core.DataAccess;
using AutoParts.DataAccess.Contexts;
using AutoParts.DataAccess.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace AutoParts.DataAccess.Dals;

public interface IVehicleModelDal : IEntityRepository<VehicleModel>
{
    Task<VehicleModel?> GetWithIncludesAsync(int id);
    Task<PagedResult<VehicleModel>> GetPagedWithIncludesAsync(
        Expression<Func<VehicleModel, bool>>? filter = null,
        int pageNumber = 1,
        int pageSize = 20);
}

public class VehicleModelDal : EfEntityRepository<VehicleModel, AutoPartsStoreDb>, IVehicleModelDal
{
    private readonly AutoPartsStoreDb _db;

    public VehicleModelDal(AutoPartsStoreDb context) : base(context)
    {
        _db = context;
    }

    public async Task<VehicleModel?> GetWithIncludesAsync(int id)
    {
        return await _db.Set<VehicleModel>()
            .Include(m => m.Brand)
            .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
    }

    public async Task<PagedResult<VehicleModel>> GetPagedWithIncludesAsync(
        Expression<Func<VehicleModel, bool>>? filter = null,
        int pageNumber = 1,
        int pageSize = 20)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 20;

        IQueryable<VehicleModel> query = _db.Set<VehicleModel>()
            .Include(m => m.Brand)
            .AsNoTracking()
            .Where(m => !m.IsDeleted);

        if (filter != null)
            query = query.Where(filter);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<VehicleModel>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
