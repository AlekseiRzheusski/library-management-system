using System.Globalization;
using FluentValidation;
using LibraryManagement.Application.Services.DTOs.BookModels;

namespace LibraryManagement.Application.Validation;

public class SearchBookCommandValidator : AbstractValidator<SearchBookCommand>
{
    public SearchBookCommandValidator()
    {
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
        
        RuleFor(b=>b.PageCount)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");
    }
}
