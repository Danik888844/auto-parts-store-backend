using System.Linq.Expressions;

namespace AutoParts.Core.DataAccess;

public sealed class PagedResult<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public required int TotalCount { get; init; }
    public required int PageNumber { get; init; }
    public required int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

public interface IEntityRepository<T> where T: class, IEntity, new()
{
    List<T> GetAll(Expression<Func<T, bool>>? filter = null);
    T? Get(Expression<Func<T, bool>> filter);
    bool Any(Expression<Func<T, bool>>? filter = null);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    void Detach(T entity);
    
    // async
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
    Task<PagedResult<T>> GetAllPagedAsync(Expression<Func<T, bool>>? filter = null, 
        int pageNumber = 1, int pageSize = 20);
    Task<T?> GetAsync(Expression<Func<T, bool>> filter);
    Task<bool> AnyAsync(Expression<Func<T, bool>>? filter = null);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}