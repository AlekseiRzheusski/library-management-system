using FluentValidation;

using LibraryManagement.Application.Services.DTOs.BorrowingModels;
using LibraryManagement.Infrastructure.Repositories.Interfaces;

namespace LibraryManagement.Application.Validation.BorrowingValidators;

public class BorrowBookCommandValidator : AbstractValidator<BorrowBookCommand>
{
    private readonly IBookRepository _bookRepository;
    public BorrowBookCommandValidator(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;

        RuleFor(b => b.UserId)
            .NotEmpty().WithMessage("UserID is requred")
            .GreaterThan(0).WithMessage("UserID must be greater than 0");

        RuleFor(b => b.daysToReturn)
            .GreaterThan(0).WithMessage("Days to return must be greater than 0")
            .When(b => b.daysToReturn != null);

        RuleFor(b => b.BookId)
            .NotEmpty().WithMessage("BookID is required.")
            .MustAsync(async (bookId, cancellation) =>
                await _bookRepository.ExistsAsync(b => b.BookId == bookId, cancellation))
            .WithMessage("Book with such Id doesn't exist")
            .DependentRules(() =>
            {
                RuleFor(b => b.BookId)
                .MustAsync(async (bookId, cancellation) =>
                {
                    var book = await _bookRepository.GetByIdAsync(bookId);
                    return book!.IsAvailable;
                })
                .WithMessage("Book is not available");
            });
    }
}
