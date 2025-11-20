using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Repositories.Interfaces;

namespace LibraryManagement.Infrastructure.Repositories;

public class BorrowingRepository : BaseRepository<Borrowing>, IBorrowingRepository
{
    public BorrowingRepository(LibraryDbContext context) : base(context) { }

    public async Task<Borrowing?> GetDetailedBorrowing(long id)
    {
        var borrowing = await _dbSet.FindAsync(id);
        if (borrowing == null) return null;

        await _context.Entry(borrowing).Reference(b => b.Book).LoadAsync();
        return borrowing;
    }
}
