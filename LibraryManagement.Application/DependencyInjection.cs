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

namespace LibraryManagement.Application;

public static class DependencyInjection
{
    public static void AddApplication(this Container container)
    {
        container.Register<IBookService, BookService>(Lifestyle.Scoped);
        container.Register<IBorrowingService, BorrowingService>(Lifestyle.Scoped);

        container.Register<IValidator<CreateBookCommand>, CreateBookCommandValidator>(Lifestyle.Scoped);
        container.Register<IValidator<SearchBookCommand>, SearchBookCommandValidator>(Lifestyle.Singleton);
        container.Register(typeof(ISearchService<>), typeof(SearchService<>), Lifestyle.Singleton);
        container.Register<IValidator<UpdateBookCommand>, UpdateBookCommandValidator>(Lifestyle.Scoped);
        container.Register<IValidator<BorrowBookCommand>, BorrowBookCommandValidator>(Lifestyle.Scoped);
        container.Register<IValidator<UserBorrowingsCommand>, UserBorrowingsCommandValidator>(Lifestyle.Scoped);
    }
}
