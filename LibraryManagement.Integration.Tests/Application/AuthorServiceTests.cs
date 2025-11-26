using SimpleInjector.Lifestyles;

using LibraryManagement.Application.Services.DTOs.AuthorModels;
using LibraryManagement.Application.Services.Interaces;
using LibraryManagement.Integration.Tests.Fixtures;

namespace LibraryManagement.Integration.Tests.Application;

public class AuthorServiceTests : IClassFixture<SqliteTestDatabaseFixture>
{
    private readonly SqliteTestDatabaseFixture _fixture;
    public AuthorServiceTests(SqliteTestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetAuthorAsync_WhenIdExists_ShouldReturn()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<IAuthorService>();

            var authorDto = await service.GetAuthorAsync(2);

            Assert.Equal(2, authorDto.AuthorId);
            Assert.Equal(2, authorDto.BookCount);
        }
    }

    [Fact]
    public async Task GetAuthorsAsync_WhenAuthorExists_ShouldReturnEnumerable()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<IAuthorService>();

            var command = new SearchAuthorCommand
            {
                FirstName = "Ant",
                DateOfBirth = "1860-01-29",
                IsActive = false
            };

            var (totalCount, numberOfPages, result) = await service.GetAuthorsAsync(command, 10,1);
            var resultItem = result.FirstOrDefault();

            Assert.Equal(1, totalCount);
            Assert.Equal(1, numberOfPages);
            Assert.Equal(command.DateOfBirth, resultItem!.DateOfBirth);
        }
    }

    [Fact]
    public async Task CreateAuthorAsync_WhenCommandIsCorrect_ShouldAdd()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<IAuthorService>();

            var command = new CreateAuthorCommand
            {
                FirstName = "Andrei",
                LastName = "Platonov",
                Biography = "Was a Soviet Russian novelist, short story writer, philosopher, playwright, and poet. Although Platonov regarded himself as a communist, his principal works remained unpublished in his lifetime because of their skeptical attitude toward collectivization of agriculture (1929-1940) and other Stalinist policies, as well as for their experimental, avant-garde form infused with existentialism which was not in line with the dominant socialist realism doctrine.",
                DateOfBirth = "1899-08-28"
            };

            var newAuthorDto = await service.CreateAuthorAsync(command);

            Assert.Equal(command.FirstName, newAuthorDto.FirstName);
            Assert.Equal(command.DateOfBirth, newAuthorDto.DateOfBirth);
        }
    }

    [Fact]
    public async Task UpdateAuthorAsync_WhenAuthorExists_ShouldUpdate()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var service = _fixture.Container.GetInstance<IAuthorService>();

            var command = new UpdateAuthorCommand
            {
                FirstName = "Jack",
                LastName = "London",
                DateOfBirth = "1876-01-12"
            };

            var updatedAuthorDto = await service.UpdateAuthorAsync(command, 6);

            Assert.Equal(command.FirstName, updatedAuthorDto.FirstName);
            Assert.Equal(command.LastName, updatedAuthorDto.LastName);
            Assert.Equal(command.DateOfBirth, updatedAuthorDto.DateOfBirth);
        }
    }
}
