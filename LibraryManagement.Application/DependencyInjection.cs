using SimpleInjector;
using AutoMapper;
using Microsoft.Extensions.Logging;

using LibraryManagement.Application.Mappings;
using LibraryManagement.Application.Services;
using LibraryManagement.Application.Services.Interaces;
using FluentValidation;
using LibraryManagement.Application.Services.DTOs.BookModels;
using LibraryManagement.Application.Validation;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Application.Services.DTOs.BorrowingModels;
using LibraryManagement.Application.Services.DTOs.CategoryModels;
using LibraryManagement.Application.Services.DTOs.AuthorModels;

namespace LibraryManagement.Application;

public static class DependencyInjection
{
    public static void AddApplication(this Container container)
    {
        container.Register<IBookService, BookService>(Lifestyle.Scoped);
        container.Register<IBorrowingService, BorrowingService>(Lifestyle.Scoped);
        container.Register<ICategoryService, CategoryService>(Lifestyle.Scoped);
        container.Register<IAuthorService, AuthorService>(Lifestyle.Scoped);

        container.Register<ICategorySortOrderService, CategorySortOrderService>(Lifestyle.Scoped);
        container.Register(typeof(ISearchService<>), typeof(SearchService<>), Lifestyle.Singleton);

        container.Register<IValidator<CreateBookCommand>, CreateBookCommandValidator>(Lifestyle.Scoped);
        container.Register<IValidator<SearchBookCommand>, SearchBookCommandValidator>(Lifestyle.Singleton);
        container.Register<IValidator<UpdateBookCommand>, UpdateBookCommandValidator>(Lifestyle.Scoped);

        container.Register<IValidator<BorrowBookCommand>, BorrowBookCommandValidator>(Lifestyle.Scoped);
        container.Register<IValidator<UserBorrowingsCommand>, UserBorrowingsCommandValidator>(Lifestyle.Scoped);

        container.Register<IValidator<SearchCategoryCommand>, SearchCategoryCommandValidator>(Lifestyle.Scoped);
        container.Register<IValidator<CreateCategoryCommand>, CreateCategoryCommandValidator>(Lifestyle.Scoped);

        container.Register<IValidator<SearchAuthorCommand>, SearchAuthorCommandValidator>(Lifestyle.Scoped);
        container.Register<IValidator<CreateAuthorCommand>, CreateAuthorCommandValidator>(Lifestyle.Scoped);
        container.Register<IValidator<UpdateAuthorCommand>, UpdateAuthorCommandValidator>(Lifestyle.Scoped);
    }
}
