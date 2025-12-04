using FluentValidation;
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Validation.BorrowingValidators;

public class EmptyCategoryValidator: AbstractValidator<Category>
{
    public EmptyCategoryValidator()
    {
        RuleFor(c=>c.Books)
            .Must((books) => books.Count()==0)
            .WithMessage("This category has related books");
        
        RuleFor(c=>c.SubCategories)
            .Must((subCategories)=> subCategories.Count()==0)
            .WithMessage("This category has related subcategories");
    }
}
