
using LibraryManagement.Integration.Tests.Fixtures;
using LibraryManagement.Application.Services.DTOs.BookModels;
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Integration.Tests.Application;

public class SearchServiceTests : IClassFixture<SqliteTestDatabaseFixture>
{
    private readonly SqliteTestDatabaseFixture _fixture;
    public SearchServiceTests(SqliteTestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ExpressionBuilder_ReturnsCorrectExpression()
    {
        var searchService = _fixture.Container.GetInstance<ISearchService<Book>>();

        var searchBookCommand = new SearchBookCommand
        {
            ISBN = "97",
            PublishedDate = "2022-10-12",
            AuthorId = 1,
            IsAvailable = false,
            Title = "Alex"
        };
        var expression = searchService.BuildExpression<SearchBookCommand>(searchBookCommand);
        Assert.NotNull(expression);

        var strExpression = expression.ToString();

        Assert.Contains("(e.Title.Contains(\"Alex\")", strExpression);
        Assert.Contains("e.ISBN.Contains(\"97\")", strExpression);
        Assert.Contains("(e.AuthorId == 1)", strExpression);
        Assert.Contains("e.PublishedDate == 10/12/2022", strExpression);
        Assert.Contains("(e.IsAvailable == False)", strExpression);
    }
}
