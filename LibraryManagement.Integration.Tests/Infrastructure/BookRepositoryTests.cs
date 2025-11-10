using Microsoft.EntityFrameworkCore;

using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Repositories;
using LibraryManagement.Integration.Tests.Fixtures;

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
        using (var context = _fixture.CreateContext())
        {
            var repository = new BookRepository(context);
            var result = await repository.GetDetailedBookInfo(2);
            Assert.NotNull(result);
            Assert.Equal(2, result.BookId);
            Assert.Equal("Structured computer architecture", result.Title);
            Assert.Equal("Tanenbaum", result.Author.LastName);
            Assert.Equal("Programming", result.Category.Name);
        }
    }
}
