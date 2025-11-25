using FluentValidation;

using LibraryManagement.Application.Services.DTOs.CategoryModels;
using LibraryManagement.Infrastructure.Repositories.Interfaces;

namespace LibraryManagement.Application.Validation;

public class SearchCategoryCommandValidator : AbstractValidator<SearchCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    public SearchCategoryCommandValidator(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;

        RuleFor(c=>c.ParentCategoryId)
            .MustAsync(async (categoryId, cancellation)=>
                await _categoryRepository.ExistsAsync(c=>c.CategoryId == categoryId, cancellation))
            .WithMessage("Category with such Id doesn't exist.")
            .When(c => c.ParentCategoryId != null);
    }
}
