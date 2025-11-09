namespace LibraryManagement.Infrastructure.Repositories.Interfaces;

public interface IBaseRepository<T> where T : class
{
    Task<T?> GetByIdAsync(long id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Delete(T entity);
    void Update(T entity);
    Task SaveAsync();
    Task<int> CountAsync();
}
