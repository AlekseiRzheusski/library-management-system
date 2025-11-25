using LibraryManagement.Domain.Entities;
using LibraryManagement.Application.Services.Interaces;
using LibraryManagement.Infrastructure.Repositories.Interfaces;

namespace LibraryManagement.Application.Services;

public class CategorySortOrderService : ICategorySortOrderService
{
    private readonly ICategoryRepository _categoryRepository;
    private int _currentSort = 0;

    public CategorySortOrderService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

     public async Task ReorderCategoriesAsync()
    {
        var categories = await _categoryRepository.FindAndAddToContextAsync(e=>true);

        var lookup = categories.ToLookup(c => c.ParentCategoryId);

        foreach (var category in categories)
        {
            category.SubCategories = lookup[category.CategoryId]
                .OrderBy(c => c.Name)
                .ToList();
        }

        var roots = lookup[null]
            .OrderBy(c => c.Name)
            .ToList();

        _currentSort = 0;

        IterativeDFS(roots);

        await _categoryRepository.SaveAsync();
    }

    private void IterativeDFS(List<Category> roots)
    {
        var stack = new Stack<Category>();

        foreach (var root in roots.OrderByDescending(c => c.Name))
            stack.Push(root);

        while (stack.Count > 0)
        {
            var category = stack.Pop();
            category.SortOrder = _currentSort++;

            foreach (var child in category.SubCategories.OrderByDescending(c => c.Name))
                stack.Push(child);
        }
    }
}
