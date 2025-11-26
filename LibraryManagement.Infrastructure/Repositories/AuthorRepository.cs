using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Repositories.Interfaces;

namespace LibraryManagement.Infrastructure.Repositories;

public class AuthorRepository : BaseRepository<Author>, IAuthorRepository
{
    public AuthorRepository(LibraryDbContext context) : base(context) { }

    public async Task<Author?> GetDetailedAuthorByIdAsync(long id)
    {
        var author = await _dbSet.FindAsync(id);
        if (author is null) return null;

        await _context.Entry(author).Collection(a => a.Books).LoadAsync();
        return author;
    }

    public async Task<IEnumerable<Author>> FindAuthorAsync(
        Expression<Func<Author, bool>> predicate, 
        int pageSize, 
        int pageNumber)
    {
        return await _dbSet
            .Where(predicate)
            .Include(a => a.Books)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }
}
