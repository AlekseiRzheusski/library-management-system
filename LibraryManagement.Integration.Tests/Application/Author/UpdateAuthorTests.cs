using MediatR;
using SimpleInjector.Lifestyles;

using LibraryManagement.Application.Authors.UpdateAuthor;
using LibraryManagement.Integration.Tests.Fixtures;
using LibraryManagement.Application.Services.DTOs.AuthorModels;

namespace LibraryManagement.Integration.Tests.Application.Author;

[Collection("Author collection")]
public class UpdateAuthorTests
{
    private readonly SqliteTestDatabaseFixture _fixture;
    public UpdateAuthorTests(SqliteTestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task UpdateAuthor_WhenAuthorExists_ShouldUpdate()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var mediator = _fixture.Container.GetInstance<IMediator>();

            var command = new UpdateAuthorCommand
            {
                FirstName = "Jack",
                LastName = "London",
                DateOfBirth = "1876-01-12"
            };

            var updatedAuthorDto = await mediator.Send(new UpdateAuthor(command, 6));

            Assert.Equal(command.FirstName, updatedAuthorDto.FirstName);
            Assert.Equal(command.LastName, updatedAuthorDto.LastName);
            Assert.Equal(command.DateOfBirth, updatedAuthorDto.DateOfBirth);
        }
    }
}
