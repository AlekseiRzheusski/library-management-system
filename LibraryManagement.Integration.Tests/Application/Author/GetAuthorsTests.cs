using MediatR;
using SimpleInjector.Lifestyles;

using LibraryManagement.Integration.Tests.Fixtures;
using LibraryManagement.Application.Authors.GetAuthor;
using LibraryManagement.Application.Services.DTOs.AuthorModels;
using LibraryManagement.Application.Authors.GetAuthors;

namespace LibraryManagement.Integration.Tests.Application.Author;

[Collection("Author collection")]
public class GetAuthorsTests
{
    private readonly SqliteTestDatabaseFixture _fixture;
    public GetAuthorsTests(SqliteTestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetAuthors_WhenAuthorsExist_ShouldReturnDtos()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var mediator = _fixture.Container.GetInstance<IMediator>();
            var command = new SearchAuthorCommand
            {
                FirstName = "Ant",
                DateOfBirth = "1860-01-29",
                IsActive = false
            };

            var (totalCount, numberOfPages, result) = await mediator.Send(new GetAuthors(command, 10, 1));
            var resultItem = result.FirstOrDefault();

            Assert.Equal(1, totalCount);
            Assert.Equal(1, numberOfPages);
            Assert.Equal(command.DateOfBirth, resultItem!.DateOfBirth);

        }
    }
}
