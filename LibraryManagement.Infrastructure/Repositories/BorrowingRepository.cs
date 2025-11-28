using System.Linq.Expressions;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Repositories;

public class BorrowingRepository : BaseRepository<Borrowing>, IBorrowingRepository
{
    public BorrowingRepository(LibraryDbContext context) : base(context) { }

    public override async Task<Borrowing?> GetDetailedEntityByIdAsync(long id)
    {
        var borrowing = await _dbSet.FindAsync(id);
        if (borrowing == null) return null;

        await _context.Entry(borrowing).Reference(b => b.Book).LoadAsync();
        return borrowing;
    }

    public override async Task<IEnumerable<Borrowing>> FindDetaliedEntitiesPageAsync(
        Expression<Func<Borrowing, bool>> predicate, 
        int pageSize, 
        int pageNumber)
    {
        return await _dbSet
            .Where(predicate)
            .Include(b => b.Book)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }

    public override async Task<IEnumerable<Borrowing>> FindDetaliedEntitiesAsync(Expression<Func<Borrowing, bool>> predicate)
    {
        return await _dbSet
            .Where(predicate)
            .Include(b => b.Book)
            .AsNoTracking()
            .ToListAsync();
    }
}
