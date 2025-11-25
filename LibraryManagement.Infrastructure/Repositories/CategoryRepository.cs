using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Repositories.Interfaces;

namespace LibraryManagement.Infrastructure.Repositories;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(LibraryDbContext context) : base(context) { }

    public async Task<Category?> GetDetailedCategoryByIdAsync(long id)
    {
        var category = await _dbSet.FindAsync(id);
        if (category == null) return null;

        await _context.Entry(category).Reference(c => c.ParentCategory).LoadAsync();
        await _context.Entry(category).Collection(c => c.Books).LoadAsync();
        return category;
    }

    public async Task<IEnumerable<Category>> GetDetailedCategoriesAsync(Expression<Func<Category, bool>> predicate)
    {
        return await _dbSet
            .Where(predicate)
            .Include(c=>c.ParentCategory)
            .Include(c => c.Books)
            .AsNoTracking()
            .ToListAsync();
    }
}
