using System.Linq.Expressions;

using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Repositories;

public class BookRepository : BaseRepository<Book>, IBookRepository
{
    public BookRepository(LibraryDbContext context) : base(context) { }

    public async Task<Book?> GetDetailedBookInfoAsync(long id)
    {
        var book = await _dbSet.FindAsync(id);
        if (book == null) return null;

        await _context.Entry(book).Reference(b => b.Author).LoadAsync();
        await _context.Entry(book).Reference(b => b.Category).LoadAsync();
        return book;
    }

    public async Task<IEnumerable<Book>> FindBooksAsync(Expression<Func<Book, bool>> predicate, int pageSize, int pageNumber)
    {
        return await _dbSet
            .Where(predicate)
            .Include(b=>b.Author)
            .Include(b=>b.Category)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }
}
