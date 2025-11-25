using AutoMapper;
using FluentValidation;

using LibraryManagement.Domain.Entities;
using LibraryManagement.Shared.Exceptions;
using LibraryManagement.Application.Services.Interaces;
using LibraryManagement.Application.Services.DTOs.CategoryModels;
using LibraryManagement.Infrastructure.Repositories.Interfaces;

namespace LibraryManagement.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ISearchService<Category> _categorySearchService;
    private readonly ICategorySortOrderService _categorySortOrderService;
    private readonly IValidator<SearchCategoryCommand> _searchCategoryCommandValidator;
    private readonly IValidator<CreateCategoryCommand> _createCategoryCommandValidator;

    public CategoryService(
        IMapper mapper,
        ICategoryRepository categoryRepository,
        ISearchService<Category> categorySearchService,
        ICategorySortOrderService categorySortOrderService,
        IValidator<SearchCategoryCommand> searchCategoryCommandValidator,
        IValidator<CreateCategoryCommand> createCategoryCommandValidator
    )
    {
        _mapper = mapper;
        _categoryRepository = categoryRepository;
        _categorySearchService = categorySearchService;
        _categorySortOrderService = categorySortOrderService;
        _searchCategoryCommandValidator = searchCategoryCommandValidator;
        _createCategoryCommandValidator = createCategoryCommandValidator;
    }

    public async Task<List<CategoryTreeDto>> GetCategoryTreeAsync()
    {
        var categories = await _categoryRepository.GetDetailedCategoriesAsync(e => true);

        var lookup = categories.ToLookup(c => c.ParentCategoryId);
        foreach (var category in categories)
        {
            category.SubCategories = lookup[category.CategoryId].ToList();
        }

        var roots = lookup[null]
            .ToList();

        return _mapper.Map<List<CategoryTreeDto>>(roots);
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync(SearchCategoryCommand command)
    {
        var validation = await _searchCategoryCommandValidator.ValidateAsync(command);
        if (!validation.IsValid)
        {
            var message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(message);
        }

        var expression = _categorySearchService.BuildExpression<SearchCategoryCommand>(command);

        var result = await _categoryRepository.GetDetailedCategoriesAsync(expression);

        if (!result.Any())
        {
            throw new EntityNotFoundException("No results match your search criteria.");
        }

        return _mapper.Map<List<CategoryDto>>(result);
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryCommand command)
    {
        var validation = await _createCategoryCommandValidator.ValidateAsync(command);
        if (!validation.IsValid)
        {
            var message = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(message);
        }

        Category newCategory = _mapper.Map<Category>(command);

        await _categoryRepository.AddAsync(newCategory);
        await _categoryRepository.SaveAsync();

        await _categorySortOrderService.ReorderCategoriesAsync();

        var newDetailedCategory = await _categoryRepository.GetDetailedCategoryByIdAsync(newCategory.CategoryId);

        return _mapper.Map<CategoryDto>(newDetailedCategory);
    }
}
