using System.Linq.Expressions;
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Infrastructure.Repositories.Interfaces;

public interface IBorrowingRepository : IBaseRepository<Borrowing>
{
    public Task<Borrowing?> GetDetailedBorrowing(long id);
    Task<IEnumerable<Borrowing>> FindBorrowingsAsync(Expression<Func<Borrowing, bool>> predicate, int pageNumber, int pageSize);
}
