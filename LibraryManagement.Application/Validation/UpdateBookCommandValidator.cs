using FluentValidation;
using System.Globalization;

using LibraryManagement.Application.Services.DTOs.BookModels;
using LibraryManagement.Infrastructure.Repositories.Interfaces;

namespace LibraryManagement.Application.Validation;

public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    private readonly IBookRepository _bookRepository;

    public UpdateBookCommandValidator(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;

        RuleFor(b => b.CategoryId)
            .MustAsync(async (categoryId, cancellation) =>
                await _bookRepository.ExistsAsync(b => b.CategoryId == categoryId, cancellation))
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
