using FluentValidation;
using LibraryManagement.Application.Services.DTOs.BorrowingModels;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.Validation.BorrowingValidators;

public class UserBorrowingsCommandValidator : AbstractValidator<UserBorrowingsCommand>
{
    public UserBorrowingsCommandValidator()
    {
        RuleFor(b => b.UserId)
            .GreaterThan(0).WithMessage("UserID must be greater than 0");

        RuleFor(b => b.Status)
            .Must((status) => Enum.TryParse(typeof(BorrowingStatus), status, out _))
            .When(b => !string.IsNullOrEmpty(b.Status))
            .WithMessage("Borrowing status must be Active, Returned, Overdue");
    }
}
