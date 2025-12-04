using SimpleInjector.Lifestyles;
using MediatR;

using LibraryManagement.Integration.Tests.Fixtures;
using LibraryManagement.Application.Services.DTOs.AuthorModels;
using LibraryManagement.Application.Authors.CreateAuthor;

namespace LibraryManagement.Integration.Tests.Application.Author;
[Collection("Author collection")]
public class CreateAuthorTests
{
    private readonly SqliteTestDatabaseFixture _fixture;
    public CreateAuthorTests(SqliteTestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateAuthor_WhenCommandIsCorrect_ShouldAdd()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var mediator = _fixture.Container.GetInstance<IMediator>();

            var command = new CreateAuthorCommand
            {
                FirstName = "Andrei",
                LastName = "Platonov",
                Biography = "Was a Soviet Russian novelist, short story writer, philosopher, playwright, and poet. Although Platonov regarded himself as a communist, his principal works remained unpublished in his lifetime because of their skeptical attitude toward collectivization of agriculture (1929-1940) and other Stalinist policies, as well as for their experimental, avant-garde form infused with existentialism which was not in line with the dominant socialist realism doctrine.",
                DateOfBirth = "1899-08-28"
            };

            var newAuthorDto = await mediator.Send(new CreateAuthor(command));

            Assert.Equal(command.FirstName, newAuthorDto.FirstName);
            Assert.Equal(command.DateOfBirth, newAuthorDto.DateOfBirth);
        }
    }
}
