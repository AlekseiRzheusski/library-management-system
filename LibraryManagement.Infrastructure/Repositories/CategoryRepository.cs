using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Repositories.Interfaces;

namespace LibraryManagement.Infrastructure.Repositories;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(LibraryDbContext context) : base(context) { }

    public override async Task<Category?> GetDetailedEntityByIdAsync(long id)
    {
        var category = await _dbSet.FindAsync(id);
        if (category == null) return null;

        await _context.Entry(category).Reference(c => c.ParentCategory).LoadAsync();
        await _context.Entry(category).Collection(c => c.Books).LoadAsync();
        await _context.Entry(category).Collection(c => c.SubCategories).LoadAsync();
        return category;
    }

    public override async Task<IEnumerable<Category>> FindDetaliedEntitiesPageAsync(
        Expression<Func<Category, bool>> predicate, 
        int pageSize, 
        int pageNumber)
    {
        return await _dbSet
            .Where(predicate)
            .Include(c=>c.ParentCategory)
            .Include(c => c.Books)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }

    public override async Task<IEnumerable<Category>> FindDetaliedEntitiesAsync(Expression<Func<Category, bool>> predicate)
    {
        return await _dbSet
            .Where(predicate)
            .Include(c=>c.ParentCategory)
            .Include(c => c.Books)
            .AsNoTracking()
            .ToListAsync();
    }
}
