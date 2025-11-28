using FluentValidation;
using System.Globalization;

using LibraryManagement.Application.Services.DTOs.AuthorModels;

namespace LibraryManagement.Application.Validation.AuthorValidators;

public class SearchAuthorCommandValidator : AbstractValidator<SearchAuthorCommand>
{
    public SearchAuthorCommandValidator()
    {
        RuleFor(a => a.DateOfBirth)
            .Must((publishedDate) =>
                DateTime.TryParseExact(
                    publishedDate,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out _))
            .When(a => !string.IsNullOrEmpty(a.DateOfBirth))
            .WithMessage("This date cannot be parsed");
    }
}
