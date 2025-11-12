using SimpleInjector;
using AutoMapper;
using Microsoft.Extensions.Logging;

using LibraryManagement.Application.Mappings;
using LibraryManagement.Application.Services;
using LibraryManagement.Application.Services.Interaces;

namespace LibraryManagement.Application;

public static class DependencyInjection
{
    public static void AddApplication(this Container container)
    {
        container.Register<IBookService, BookService>(Lifestyle.Scoped);
    }
}
