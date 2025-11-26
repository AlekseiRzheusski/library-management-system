using System.Linq.Expressions;
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Infrastructure.Repositories.Interfaces;

public interface IAuthorRepository : IBaseRepository<Author>
{
    public Task<Author?> GetDetailedAuthorByIdAsync(long id);

    public Task<IEnumerable<Author>> FindAuthorAsync(
        Expression<Func<Author, bool>> predicate, 
        int pageSize, 
        int pageNumber);
}
