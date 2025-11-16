using System.Linq.Expressions;

using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Infrastructure.Repositories.Interfaces;

public interface IBookRepository: IBaseRepository<Book>
{
    Task<Book?> GetDetailedBookInfo(long id);
    Task<IEnumerable<Book>> FindBooksAsync(Expression<Func<Book, bool>> predicate, int pageSize, int pageNumber);
}
