using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Infrastructure.Repositories.Interfaces;

public interface IBorrowingRepository : IBaseRepository<Borrowing>
{
    public Task<Borrowing?> GetDetailedBorrowing(long id);
}
