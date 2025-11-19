using Microsoft.EntityFrameworkCore;
using SimpleInjector;

using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Repositories;
using LibraryManagement.Infrastructure.Repositories.Interfaces;

namespace LibraryManagement.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this Container container, DbContextOptions<LibraryDbContext> dbOptions)
    {
        container.Register(() => new LibraryDbContext(dbOptions), Lifestyle.Scoped);
        container.Register<IAuthorRepository, AuthorRepository>(Lifestyle.Scoped);
        container.Register<IBookRepository, BookRepository>(Lifestyle.Scoped);
        container.Register<ICategoryRepository, CategoryRepository>(Lifestyle.Scoped);
        container.Register<IBorrowingRepository, BorrowingRepository>(Lifestyle.Scoped);
    }
}
