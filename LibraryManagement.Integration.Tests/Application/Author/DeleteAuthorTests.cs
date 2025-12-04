using MediatR;
using SimpleInjector.Lifestyles;

using LibraryManagement.Integration.Tests.Fixtures;
using LibraryManagement.Application.Authors.DeleteAuthor;
using FluentValidation;
using LibraryManagement.Application.Authors.CreateAuthor;
using LibraryManagement.Application.Services.DTOs.AuthorModels;
using LibraryManagement.Infrastructure.Repositories.Interfaces;

namespace LibraryManagement.Integration.Tests.Application.Author;

[Collection("Author collection")]
public class DeleteAuthorTests
{
    private readonly SqliteTestDatabaseFixture _fixture;
    public DeleteAuthorTests(SqliteTestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DeleteAuthor_WhenAuthorHasBooks_ShouldThrow()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var mediator = _fixture.Container.GetInstance<IMediator>();

            await Assert.ThrowsAsync<ValidationException>(async () => 
            {
                await mediator.Send(new DeleteAuthor(1));
            });
        }
    }

    [Fact]
    public async Task DeleteAuthor_WhenAuthorDoesNotHaveBooks_ShouldDelete()
    {
        using (AsyncScopedLifestyle.BeginScope(_fixture.Container))
        {
            var mediator = _fixture.Container.GetInstance<IMediator>();
            var authorRepository = _fixture.Container.GetInstance<IAuthorRepository>();

            var command = new CreateAuthorCommand
            {
                FirstName = "Author",
                LastName = "Author",
                Biography = "Author",
                DateOfBirth = "1899-08-28"
            };

            var newAuthorDto = await mediator.Send(new CreateAuthor(command));

            await mediator.Send(new DeleteAuthor(newAuthorDto.AuthorId));

            var deletedEntity = await authorRepository.ExistsAsync(a=>a.AuthorId == newAuthorDto.AuthorId);
            Assert.False(deletedEntity);
        }
    }
}
