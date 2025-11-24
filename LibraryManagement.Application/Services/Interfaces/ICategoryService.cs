using LibraryManagement.Application.Services.DTOs.CategoryModels;

namespace LibraryManagement.Application.Services.Interaces;

public interface ICategoryService
{
    public Task<List<CategoryTreeDto>> GetCategoryTreeAsync();
    public Task<List<CategoryDto>> GetCategoriesAsync(SearchCategoryCommand command);
    public Task<CategoryDto> CreateCategoryAsync(CreateCategoryCommand command);
}
