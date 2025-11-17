using System.Globalization;
using FluentValidation;
using LibraryManagement.Application.Services.DTOs.BookModels;
using LibraryManagement.Infrastructure.Repositories;
using LibraryManagement.Infrastructure.Repositories.Interfaces;

namespace LibraryManagement.Application.Validation;

public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    private readonly IBookRepository _bookRepository;
    public CreateBookCommandValidator(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;

        RuleFor(b => b.Title)
            .NotEmpty();

        RuleFor(b => b.Isbn)
            .NotEmpty().WithMessage("ISBN is required.")
            .Length(13).WithMessage("ISBN must be 13 characters long.")
            .MustAsync(async (isbn, cancellation) =>
                !await _bookRepository.ExistsAsync(b => b.ISBN == isbn, cancellation))
            .WithMessage("The ISBN should be unique");

        RuleFor(b => b.AuthorId)
            .NotEmpty().WithMessage("AuthorID is required.")
            .MustAsync(async (authorId, cancellation) =>
                await _bookRepository.ExistsAsync(b => b.AuthorId == authorId, cancellation))
            .WithMessage("Author with such Id doesn't exist");

        RuleFor(b => b.CategoryId)
            .NotEmpty().WithMessage("CategoryID is required.")
            .MustAsync(async (categoryId, cancellation) =>
                await _bookRepository.ExistsAsync(b => b.CategoryId == categoryId, cancellation))
            .WithMessage("Category with such Id doesn't exist.");

        RuleFor(b=>b.PageCount)
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
