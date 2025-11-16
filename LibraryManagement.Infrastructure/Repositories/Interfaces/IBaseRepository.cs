using System.Linq.Expressions;

namespace LibraryManagement.Infrastructure.Repositories.Interfaces;

public interface IBaseRepository<T> where T : class
{
    Task<T?> GetByIdAsync(long id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Delete(T entity);
    void Update(T entity);
    Task SaveAsync();
    Task<int> CountAsync();
    Task<int> GetQueryCountAsync(Expression<Func<T, bool>> predicate);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}
