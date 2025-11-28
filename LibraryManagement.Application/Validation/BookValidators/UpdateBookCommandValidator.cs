using FluentValidation;
using System.Globalization;

using LibraryManagement.Application.Services.DTOs.BookModels;
using LibraryManagement.Infrastructure.Repositories.Interfaces;

namespace LibraryManagement.Application.Validation.BookValidators;

public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateBookCommandValidator(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;

        RuleFor(b => b.CategoryId)
            .MustAsync(async (categoryId, cancellation) =>
                await _categoryRepository.ExistsAsync(c => c.CategoryId == categoryId, cancellation))
            .When(b => b.CategoryId != null)
            .WithMessage("Category with such Id doesn't exist.");

        RuleFor(b => b.PageCount)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(b => b.PublishedDate)
            .Must((publishedDate) =>
                DateTime.TryParseExact(
                    publishedDate,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out _))
            .When(b => !string.IsNullOrEmpty(b.PublishedDate))
            .WithMessage("This date cannot be parsed");
    }
}
