using SimpleInjector.Lifestyles;
using System.Linq.Expressions;

using LibraryManagement.Integration.Tests.Fixtures;
using LibraryManagement.Infrastructure.Repositories.Interfaces;
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Integration.Tests.Infrastructure;

public class BookRepositoryTests : IClassFixture<SqliteTestDatabaseFixture>
{
    private readonly SqliteTestDatabaseFixture _fixture;
    public BookRepositoryTests(SqliteTestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetDetailedBookInfo_WhenIdExists_ShouldReturnEntity()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var repository = _fixture.Container.GetInstance<IBookRepository>();
            var result = await repository.GetDetailedEntityByIdAsync(2);
            Assert.NotNull(result);
            Assert.Equal(2, result.BookId);
            Assert.Equal("Structured computer architecture", result.Title);
            Assert.Equal("Tanenbaum", result.Author.LastName);
            Assert.Equal("Programming", result.Category.Name);
        }
    }

    [Fact]
    public async Task FindBooksAsync_WhenBooksExists_ShouldReturnEnumerable()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var repository = _fixture.Container.GetInstance<IBookRepository>();

            Expression<Func<Book, bool>> expression = b => b.ISBN.Contains("978030781") && b.AuthorId == 1;

            var resultList = await repository.FindDetaliedEntitiesPageAsync(expression, 100, 1);

            Assert.Single(resultList);
            var resultEntity = resultList.First();

            Assert.Equal(5, resultEntity.BookId);
            Assert.Equal(1, resultEntity.AuthorId);
        }
    }
}
