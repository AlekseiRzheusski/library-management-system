using MediatR;
using SimpleInjector.Lifestyles;

using LibraryManagement.Integration.Tests.Fixtures;
using LibraryManagement.Application.Authors.GetAuthor;

namespace LibraryManagement.Integration.Tests.Application.Author;

[Collection("Author collection")]
public class GetAuthorTests
{
    private readonly SqliteTestDatabaseFixture _fixture;
    public GetAuthorTests(SqliteTestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetAuthor_WhenAuthorExists_ShouldReturnDto()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var mediator = _fixture.Container.GetInstance<IMediator>();
            var authorDto = await mediator.Send(new GetAuthor(2));

            Assert.Equal(2, authorDto.AuthorId);
            Assert.Equal(2, authorDto.BookCount);
        }
    }
}
