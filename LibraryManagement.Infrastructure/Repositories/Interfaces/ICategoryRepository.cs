using System.Linq.Expressions;
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Infrastructure.Repositories.Interfaces;

public interface ICategoryRepository : IBaseRepository<Category>
{
    public Task<IEnumerable<Category>> GetDetailedCategoriesAsync(Expression<Func<Category, bool>> predicate);
    public Task<Category?> GetDetailedCategoryByIdAsync(long id);
}
