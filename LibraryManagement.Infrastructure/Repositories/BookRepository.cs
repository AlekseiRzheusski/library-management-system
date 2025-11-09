using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Repositories.Interfaces;

namespace LibraryManagement.Infrastructure.Repositories;

public class BookRepository : BaseRepository<Book>, IBookRepository
{
    public BookRepository(LibraryDbContext context) : base(context) { }

    public async Task<Book?> GetDetailedBookInfo(long id)
    {
        var book = await _dbSet.FindAsync(id);
        if (book == null) return null;

        await _context.Entry(book).Reference(b => b.Author).LoadAsync();
        await _context.Entry(book).Reference(b => b.Category).LoadAsync();
        return book;
    }
}
