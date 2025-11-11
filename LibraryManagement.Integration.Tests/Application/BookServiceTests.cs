using SimpleInjector.Lifestyles;

using LibraryManagement.Application.Services.Interaces;
using LibraryManagement.Integration.Tests.Fixtures;

namespace LibraryManagement.Integration.Tests.Application;

public class BookServiceTests : IClassFixture<SqliteTestDatabaseFixture>
{
    private readonly SqliteTestDatabaseFixture _fixture;
    public BookServiceTests(SqliteTestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetBookAsync_WhenIdExists_ShouldReturnBookDto()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<IBookService>();
            var result = await service.GetBookAsync(2);

            Assert.NotNull(result);
            Assert.Equal("Andrew Tanenbaum", result.AuthorName);
            Assert.Equal("Programming", result.CategoryName);
            Assert.Equal("2012-07-25", result.PublishedDate);
        }
    }

    [Fact]
    public async Task GetBookAsync_WhenIdDoesNotExist_ShouldReturnNull()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<IBookService>();
            var result = await service.GetBookAsync(10);

            Assert.Null(result);
        }
    }
}
