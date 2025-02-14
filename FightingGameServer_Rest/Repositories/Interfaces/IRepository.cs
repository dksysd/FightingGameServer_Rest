namespace FightingGameServer_Rest.Repositories.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> CreateAsync(T entity);
    Task<T?> GetByIdAsync(int id, Func<IQueryable<T>, IQueryable<T>>? includeFunc = null);
    Task<List<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>>? includeFunc = null);
    Task<T> UpdateAsync(T entity);
    Task<T> DeleteAsync(T entity);
}