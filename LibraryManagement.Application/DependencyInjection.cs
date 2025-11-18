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

namespace LibraryManagement.Application;

public static class DependencyInjection
{
    public static void AddApplication(this Container container)
    {
        container.Register<IBookService, BookService>(Lifestyle.Scoped);

        container.Register<IValidator<CreateBookCommand>, CreateBookCommandValidator>(Lifestyle.Scoped);
        container.Register<IValidator<SearchBookCommand>, SearchBookCommandValidator>(Lifestyle.Scoped);
        container.Register(typeof(ISearchService<>), typeof(SearchService<>), Lifestyle.Singleton);
        container.Register<IValidator<UpdateBookCommand>, UpdateBookCommandValidator>(Lifestyle.Scoped);
    }
}
