using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Infrastructure.Repositories.Interfaces;

public interface IBookRepository: IBaseRepository<Book>
{
    Task<Book?> GetDetailedBookInfo(long id);
}
